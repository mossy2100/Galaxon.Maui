namespace Galaxon.Maui.Utilities;

/// <summary>
/// Extension and utility methods for Microsoft.Maui.Graphics.Color.
/// </summary>
public static class ColorUtility
{
    /// <summary>
    /// Convert a decimal sRGB gamma-encoded color value between 0.0 and 1.0 to a linearized value.
    /// </summary>
    /// <param name="colorChannel"></param>
    /// <returns></returns>
    private static double Linearize(double colorChannel)
    {
        return colorChannel <= 0.04045
            ? colorChannel / 12.92
            : double.Pow((colorChannel + 0.055) / 1.055, 2.4);
    }

    /// <summary>
    /// Calculate the perceived lightness of a color in the range of 0 (black) to 100 (white).
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static double GetPerceivedLightness(this Color color)
    {
        // Convert RGB values to linear values.
        var r = Linearize(color.Red);
        var g = Linearize(color.Green);
        var b = Linearize(color.Blue);

        // Calculate luminance.
        var y = 0.2126 * r + 0.7152 * g + 0.0722 * b;

        // Find perceived lightness.
        return y <= 216.0 / 24389
            ? y * (24389.0 / 27)
            : double.Cbrt(y) * 116 - 16;
    }

    /// <summary>
    /// Returns black for a light background, white for a dark background.
    /// </summary>
    /// <param name="bgColor"></param>
    public static Color GetTextColor(this Color bgColor)
    {
        return bgColor.GetPerceivedLightness() >= 65 ? Colors.Black : Colors.White;
    }
}
