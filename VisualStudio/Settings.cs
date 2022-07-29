using ModSettings;

namespace BetterNightSky;

internal sealed class BetterNightSkySettings : JsonModSettings
{
    [Name("Shooting Stars Frequency")]
    [Description("How often shootings stars will appear in the sky.")]
    [Choice("None", "Low", "Medium", "High")]
    public int ShootingStarsFrequency = 2;

    [Name("Replace Sky")]
    [Description("If enabled, the night sky and moon will be replaced.")]
    public bool Sky = true;
}
internal static class Settings
{
    public static BetterNightSkySettings options;
    public static void OnLoad()
    {
        options = new BetterNightSkySettings();
        options.AddToModSettings("Better Night Sky", MenuType.MainMenuOnly);
    }
}