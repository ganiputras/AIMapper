namespace AIMapper;

/// <summary>
///     Antarmuka untuk AIMapper, menyediakan method mapping objek.
/// </summary>
public interface IMapper
{
    TDestination Map<TSource, TDestination>(TSource source);
    void Register<TSource, TDestination>(Func<TSource, TDestination> mapFunc);

    void RegisterBidirectional<TSource, TDestination>(
        Func<TSource, TDestination> forward,
        Func<TDestination, TSource> reverse);

    void RegisterAbstract<TAbstract, TConcrete>() where TConcrete : TAbstract;
    void Configure<TSource, TDestination>(Action<MappingConfiguration<TSource, TDestination>> config);
    void AssertConfigurationIsValid();
}