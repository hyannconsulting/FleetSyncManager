namespace Laroche.FleetManager.Domain.Extensions;

/// <summary>
/// Extension methods for enum operations
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts a string value to the specified enum type
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="value">The string value to convert</param>
    /// <param name="ignoreCase">Whether to ignore case during conversion</param>
    /// <returns>The enum value if conversion succeeds, otherwise the default value of the enum</returns>
    public static TEnum ToEnum<TEnum>(this string value, bool ignoreCase = true)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        return Enum.TryParse<TEnum>(value, ignoreCase, out var result)
            ? result
            : default;
    }

    /// <summary>
    /// Converts a string value to the specified enum type with a fallback default value
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="value">The string value to convert</param>
    /// <param name="defaultValue">The default value to return if conversion fails</param>
    /// <param name="ignoreCase">Whether to ignore case during conversion</param>
    /// <returns>The enum value if conversion succeeds, otherwise the specified default value</returns>
    public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue, bool ignoreCase = true)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        return Enum.TryParse<TEnum>(value, ignoreCase, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// Tries to convert a string value to the specified enum type
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="value">The string value to convert</param>
    /// <param name="result">The converted enum value</param>
    /// <param name="ignoreCase">Whether to ignore case during conversion</param>
    /// <returns>True if conversion succeeds, otherwise false</returns>
    public static bool TryToEnum<TEnum>(this string value, out TEnum result, bool ignoreCase = true)
        where TEnum : struct, Enum
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Enum.TryParse(value, ignoreCase, out result);
    }
}