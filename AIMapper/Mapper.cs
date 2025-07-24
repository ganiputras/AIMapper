using System.Linq.Expressions;
using System.Reflection;

namespace AIMapper;

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

    // ⬇ Semua method mapping di bawah ini harus berada di dalam class ⬇

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var key = (typeof(TSource), typeof(TDestination));

        if (_customMappings.TryGetValue(key, out var custom))
            return ((Func<TSource, TDestination>)custom)(source);

        if (_compiledMappingCache.TryGetValue(key, out var cached))
            return ((Func<TSource, TDestination>)cached)(source);

        var compiled = GenerateCompiledMapping<TSource, TDestination>();
        _compiledMappingCache[key] = compiled;
        return compiled(source);
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

            // CustomPath override
            if (options?.CustomPath != null)
            {
                Expression current = sourceParam;
                foreach (var part in options.CustomPath.Split('.'))
                {
                    var prop = current.Type.GetProperty(part);
                    if (prop == null)
                    {
                        current = null!;
                        break;
                    }

                    current = Expression.Property(current, prop);
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

            if (valueExp != null)
                bindings.Add(Expression.Bind(destProp, valueExp));
        }

        Expression body;
        if (destinationType.GetConstructors().Any(c => c.GetParameters().Length == 0))
            body = Expression.MemberInit(Expression.New(destinationType), bindings);
        else
            return src => MapToConstructor<TSource, TDestination>(src);

        var lambda = Expression.Lambda<Func<TSource, TDestination>>(body, sourceParam);
        return lambda.Compile();
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
            .FirstOrDefault() ?? throw new InvalidOperationException("No constructor found");

        var sourceProps = typeof(TSource).GetProperties();
        var args = ctor.GetParameters().Select(p =>
        {
            var match = sourceProps.FirstOrDefault(sp => sp.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)
                                                         && sp.PropertyType == p.ParameterType);
            return match?.GetValue(source) ?? throw new InvalidOperationException($"Cannot map parameter '{p.Name}'");
        }).ToArray();

        return (TDestination)ctor.Invoke(args);
    }
}