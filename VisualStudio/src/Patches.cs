using HarmonyLib;

namespace BetterNightSky
{
    [HarmonyPatch(typeof(TODStateConfig), "SetBlended")]
    internal class TODStateConfic_SetBlended
    {
        public static void Postfix(TODStateConfig __instance, int nightStates)
        {
            if (nightStates > 0)
            {
                //__instance.m_SkyBloomIntensity *= 0.3f;
                __instance.m_BloomIntensity *= 0.3f;
            }
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), "Init")]
    internal class UniStormWeatherSystem_Init
    {
        public static void Prefix()
        {
            Implementation.Log("Init");
            Implementation.Install();
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), "SetMoonPhaseIndex")]
    internal class UniStormWeatherSystem_SetMoonPhaseIndex
    {
        public static void Postfix()
        {
            Implementation.Log("SetMoonPhaseIndex");
            Implementation.UpdateMoonPhase();
        }
    }

    [HarmonyPatch(typeof(UniStormWeatherSystem), "SetMoonPhase")]
    internal class UniStormWeatherSystem_SetMoonPhase
    {
        public static void Postfix()
        {
            Implementation.UpdateMoonPhase();
        }
    }

    [HarmonyPatch(typeof(GameManager), "Awake")]
    internal class GameManager_Awake
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
}