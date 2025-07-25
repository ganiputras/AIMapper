using System.Linq.Expressions;
using System.Reflection;

namespace AIMapper.Core;

/// <summary>
///     Implementasi utama AIMapper untuk mapping objek dengan konfigurasi fleksibel,
///     mendukung custom flattening, ignore property, condition, null substitution, value converter, before/after map, dan
///     reverse map.
/// </summary>
public class Mapper : IMapper
{
    private readonly Dictionary<Type, Type> _abstractTypeMappings = new();
    private readonly Dictionary<(Type, Type), Delegate> _compiledMappingCache = new();
    private readonly Dictionary<(Type, Type), object> _configurations = new();
    private readonly Dictionary<(Type, Type), Delegate> _customMappings = new();


    public void Register<TSource, TDestination>(Func<TSource, TDestination> mapFunc)
    {
        _customMappings[(typeof(TSource), typeof(TDestination))] = mapFunc;
    }

    public void RegisterBidirectional<TSource, TDestination>(
        Func<TSource, TDestination> forward,
        Func<TDestination, TSource> reverse)
    {
        Register(forward);
        Register(reverse);
    }

    public void RegisterAbstract<TAbstract, TConcrete>() where TConcrete : TAbstract
    {
        _abstractTypeMappings[typeof(TAbstract)] = typeof(TConcrete);
    }

    public void Configure<TSource, TDestination>(Action<MappingConfiguration<TSource, TDestination>> config)
    {
        var cfg = new MappingConfiguration<TSource, TDestination>();
        config(cfg);
        _configurations[(typeof(TSource), typeof(TDestination))] = cfg;
    }

    public void AssertConfigurationIsValid()
    {
        foreach (var ((source, dest), configObj) in _configurations)
        {
            var configType = typeof(MappingConfiguration<,>).MakeGenericType(source, dest);
            if (!configType.IsInstanceOfType(configObj))
                throw new InvalidOperationException($"Invalid configuration type for {source.Name} → {dest.Name}");
        }
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source), "Source object cannot be null.");

        var key = (typeof(TSource), typeof(TDestination));

        if (_customMappings.TryGetValue(key, out var custom))
            return ((Func<TSource, TDestination>)custom)(source);

        if (_compiledMappingCache.TryGetValue(key, out var cached))
            return ((Func<TSource, TDestination>)cached)(source);

        var compiled = GenerateCompiledMapping<TSource, TDestination>();
        _compiledMappingCache[key] = compiled;
        return compiled(source);
    }

    internal void RegisterReverseConfig<TSource, TDestination>(MappingConfiguration<TSource, TDestination> config)
    {
        _configurations[(typeof(TSource), typeof(TDestination))] = config;
    }

    private Func<TSource, TDestination> GenerateCompiledMapping<TSource, TDestination>()
    {
        var sourceType = typeof(TSource);
        var destinationType = typeof(TDestination);
        var config = GetConfiguration<TSource, TDestination>();

        var sourceParam = Expression.Parameter(sourceType, "src");
        var bindings = new List<MemberBinding>();
        var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destProp in destinationType.GetProperties().Where(p => p.CanWrite))
        {
            if (config.PropertyOptions.TryGetValue(destProp.Name, out var options) && options.Ignore)
                continue;

            Expression? valueExp = null;

            // CustomPath (Override Flattening)
            if (options?.CustomPath != null)
            {
                Expression? current = sourceParam;
                foreach (var part in options.CustomPath.Split('.'))
                {
                    var prop = current.Type.GetProperty(part);
                    if (prop == null)
                    {
                        current = null;
                        break;
                    }

                    // Cek null pada parent (null-safe flattening)
                    current = Expression.Condition(
                        Expression.Equal(current, Expression.Constant(null, current.Type)),
                        Expression.Default(prop.PropertyType),
                        Expression.Property(current, prop)
                    );
                }

                valueExp = current;
            }
            else
            {
                var match = sourceProps.FirstOrDefault(p =>
                    p.Name == destProp.Name && p.PropertyType == destProp.PropertyType);
                if (match != null)
                    valueExp = Expression.Property(sourceParam, match);
                else
                    foreach (var parent in sourceProps)
                    {
                        var nested = parent.PropertyType.GetProperty(destProp.Name);
                        if (nested != null && nested.PropertyType == destProp.PropertyType)
                        {
                            valueExp = Expression.Property(Expression.Property(sourceParam, parent), nested);
                            break;
                        }
                    }
            }

            if (valueExp != null && options?.ConditionFunc != null) continue; // Akan di-handle di runtime

            if (valueExp != null && options?.ValueConverter != null)
            {
                var converterType = options.ValueConverter.GetType();
                var destPropType = destProp.PropertyType;
                var invokeMethod = converterType.GetMethod("Invoke");
                var srcType = invokeMethod.GetParameters()[0].ParameterType;
                var converterConst = Expression.Constant(options.ValueConverter);
                var invokeConverter = Expression.Invoke(converterConst, Expression.Convert(valueExp, srcType));
                var converterExp = Expression.Convert(invokeConverter, destPropType);
                bindings.Add(Expression.Bind(destProp, converterExp));
            }
            else if (valueExp != null && options?.NullSubstitute != null)
            {
                var underlyingType = Nullable.GetUnderlyingType(destProp.PropertyType) ?? destProp.PropertyType;
                var nullConst = Expression.Constant(null, underlyingType);
                var substitute = Expression.Convert(Expression.Constant(options.NullSubstitute), destProp.PropertyType);
                var condition = Expression.Equal(valueExp, nullConst);
                var coalesce = Expression.Condition(condition, substitute, valueExp);
                bindings.Add(Expression.Bind(destProp, coalesce));
            }
            else if (valueExp != null)
            {
                bindings.Add(Expression.Bind(destProp, valueExp));
            }
        }

        Expression body;
        if (destinationType.GetConstructors().Any(c => c.GetParameters().Length == 0))
            body = Expression.MemberInit(Expression.New(destinationType), bindings);
        else
            return src => MapToConstructor<TSource, TDestination>(src);

        var lambda = Expression.Lambda<Func<TSource, TDestination>>(body, sourceParam);
        var compiled = lambda.Compile();

        // Condition/BeforeMap/AfterMap dievaluasi manual di runtime setelah objek terbentuk
        return src =>
        {
            var config = GetConfiguration<TSource, TDestination>();
            var dest = compiled(src);

            // BeforeMap
            config.BeforeMapAction?.Invoke(src, dest);

            // Condition (mapping property dengan predicate)
            var props = config.PropertyOptions;
            foreach (var (propName, opt) in props)
                if (opt.ConditionFunc != null)
                {
                    var destProp = destinationType.GetProperty(propName);
                    if (destProp == null) continue;

                    var predicate = opt.ConditionFunc as Func<TSource, TDestination, bool>;
                    if (predicate != null && predicate(src, dest))
                    {
                        object? value = null;
                        if (opt.CustomPath != null)
                        {
                            object? current = src;
                            foreach (var part in opt.CustomPath.Split('.'))
                            {
                                if (current == null) break; // Perbaikan: pastikan parent null tidak error
                                var prop = current.GetType().GetProperty(part);
                                current = prop?.GetValue(current);
                            }

                            value = current;
                        }
                        else
                        {
                            var match = sourceType.GetProperty(propName);
                            value = match?.GetValue(src);
                        }

                        if (opt.ValueConverter != null && value != null)
                            value = opt.ValueConverter.DynamicInvoke(value);
                        if (value == null && opt.NullSubstitute != null) value = opt.NullSubstitute;
                        destProp.SetValue(dest, value);
                    }
                }

            // AfterMap
            config.AfterMapAction?.Invoke(src, dest);

            return dest;
        };
    }

    private MappingConfiguration<TSource, TDestination> GetConfiguration<TSource, TDestination>()
    {
        if (_configurations.TryGetValue((typeof(TSource), typeof(TDestination)), out var obj) &&
            obj is MappingConfiguration<TSource, TDestination> config)
            return config;
        return new MappingConfiguration<TSource, TDestination>();
    }

    private TDestination MapToConstructor<TSource, TDestination>(TSource source)
    {
        var ctor = typeof(TDestination).GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault() ?? throw new InvalidOperationException("No constructor found for the destination type.");

        var sourceProps = typeof(TSource).GetProperties();
        var args = ctor.GetParameters().Select(p =>
        {
            var match = sourceProps.FirstOrDefault(sp => sp.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)
                                                         && sp.PropertyType == p.ParameterType);
            return match?.GetValue(source) ?? throw new InvalidOperationException($"Cannot map parameter '{p.Name}'.");
        }).ToArray();

        return (TDestination)ctor.Invoke(args);
    }
}