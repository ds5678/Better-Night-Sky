using Il2Cpp;
using UnityEngine;
using HarmonyLib;

namespace BetterNightSky;

[HarmonyPatch(typeof(TODStateData), nameof(TODStateData.SetBlended))]
internal static class TODStateConfic_SetBlended
{
    public static void Postfix(TODStateData __instance, int nightStates)
    {
        if (nightStates > 0)
        {
            //__instance.m_SkyBloomIntensity *= 0.3f;
            __instance.m_BloomIntensity *= 0.3f;
        }
    }
}

[HarmonyPatch(typeof(UniStormWeatherSystem), nameof(UniStormWeatherSystem.Init))]
internal static class UniStormWeatherSystem_Init
{
    public static void Prefix()
    {
        Implementation.Log("Init");
        Implementation.Install();
    }
}

[HarmonyPatch(typeof(UniStormWeatherSystem), nameof(UniStormWeatherSystem.SetMoonPhaseIndex))]
internal static class UniStormWeatherSystem_SetMoonPhaseIndex
{
    public static void Postfix()
    {
        Implementation.Log("SetMoonPhaseIndex");
        Implementation.UpdateMoonPhase();
    }
}

[HarmonyPatch(typeof(UniStormWeatherSystem), nameof(UniStormWeatherSystem.SetMoonPhase))]
internal static class UniStormWeatherSystem_SetMoonPhase
{
    public static void Postfix()
    {
        Implementation.UpdateMoonPhase();
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.Awake))]
internal static class GameManager_Awake
{
    private static bool wasMainMenu = false;

    public static void Postfix()
    {
        bool isMainMenu = UpdateShootingStar.IsMainMenu();

        if (wasMainMenu != isMainMenu)
        {
            Implementation.RescheduleShootingStars();
            wasMainMenu = isMainMenu;
        }
    }
}