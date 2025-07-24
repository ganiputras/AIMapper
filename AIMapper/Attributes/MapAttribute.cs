using System;

namespace AIMapper.Attributes;
/// <summary>
/// Menandakan bahwa class ini akan otomatis dibuatkan mapping ke tipe target.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class MapAttribute : Attribute
{
    /// <summary>
    /// Tipe target mapping.
    /// </summary>
    public Type TargetType { get; }

    /// <summary>
    /// Membuat attribute Map baru.
    /// </summary>
    /// <param name="targetType">Tipe target mapping.</param>
    public MapAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}