using System.Linq.Expressions;
using System.Reflection;

namespace AIMapper;

/// <summary>
/// Implementasi utama AIMapper untuk mapping objek dengan konfigurasi fleksibel, mendukung custom flattening, ignore property, dan injection ke DI.
/// </summary>
public class Mapper : IMapper
{
    /// <summary>
    /// Mapping tipe abstract ke concrete untuk kebutuhan mapping polymorphic.
    /// </summary>
    private readonly Dictionary<Type, Type> _abstractTypeMappings = new();

    /// <summary>
    /// Cache delegate mapping hasil kompilasi expression agar performa optimal.
    /// </summary>
    private readonly Dictionary<(Type, Type), Delegate> _compiledMappingCache = new();

    /// <summary>
    /// Konfigurasi mapping untuk setiap pasangan type.
    /// </summary>
    private readonly Dictionary<(Type, Type), object> _configurations = new();

    /// <summary>
    /// Custom mapping function yang didaftarkan manual untuk kombinasi source-destination tertentu.
    /// </summary>
    private readonly Dictionary<(Type, Type), Delegate> _customMappings = new();

    /// <summary>
    /// Registrasi custom mapping function untuk tipe tertentu.
    /// </summary>
    public void Register<TSource, TDestination>(Func<TSource, TDestination> mapFunc)
    {
        _customMappings[(typeof(TSource), typeof(TDestination))] = mapFunc;
    }

    /// <summary>
    /// Registrasi mapping dua arah antara dua tipe (forward dan reverse).
    /// </summary>
    public void RegisterBidirectional<TSource, TDestination>(
        Func<TSource, TDestination> forward,
        Func<TDestination, TSource> reverse)
    {
        Register(forward);
        Register(reverse);
    }

    /// <summary>
    /// Registrasi tipe abstract ke concrete, untuk mendukung mapping polymorphic.
    /// </summary>
    public void RegisterAbstract<TAbstract, TConcrete>() where TConcrete : TAbstract
    {
        _abstractTypeMappings[typeof(TAbstract)] = typeof(TConcrete);
    }

    /// <summary>
    /// Konfigurasi mapping secara fluent untuk source dan destination tertentu.
    /// </summary>
    public void Configure<TSource, TDestination>(Action<MappingConfiguration<TSource, TDestination>> config)
    {
        var cfg = new MappingConfiguration<TSource, TDestination>();
        config(cfg);
        _configurations[(typeof(TSource), typeof(TDestination))] = cfg;
    }

    /// <summary>
    /// Validasi seluruh konfigurasi mapping yang sudah didaftarkan.
    /// </summary>
    /// <exception cref="InvalidOperationException">Dilempar jika konfigurasi tidak sesuai tipe.</exception>
    public void AssertConfigurationIsValid()
    {
        foreach (var ((source, dest), configObj) in _configurations)
        {
            var configType = typeof(MappingConfiguration<,>).MakeGenericType(source, dest);
            if (!configType.IsInstanceOfType(configObj))
                throw new InvalidOperationException($"Invalid configuration type for {source.Name} → {dest.Name}");
        }
    }

    /// <summary>
    /// Melakukan mapping objek dari source ke destination type.
    /// </summary>
    /// <param name="source">Objek sumber yang akan dimapping</param>
    /// <typeparam name="TSource">Tipe sumber</typeparam>
    /// <typeparam name="TDestination">Tipe tujuan</typeparam>
    /// <returns>Objek hasil mapping</returns>
    /// <exception cref="ArgumentNullException">Jika source bernilai null</exception>
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

    /// <summary>
    /// Membuat delegate mapping secara dinamis menggunakan expression tree.
    /// </summary>
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
                {
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

    /// <summary>
    /// Mengambil konfigurasi mapping untuk source dan destination type tertentu.
    /// </summary>
    private MappingConfiguration<TSource, TDestination> GetConfiguration<TSource, TDestination>()
    {
        if (_configurations.TryGetValue((typeof(TSource), typeof(TDestination)), out var obj) &&
            obj is MappingConfiguration<TSource, TDestination> config)
            return config;
        return new MappingConfiguration<TSource, TDestination>();
    }

    /// <summary>
    /// Mapping menggunakan constructor (jika destination tidak memiliki parameterless constructor).
    /// </summary>
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
