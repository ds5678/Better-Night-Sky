extern alias Hinterland;
using Hinterland;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BetterNightSky;

internal class Implementation : MelonLoader.MelonMod
{
    private const string NAME = "Better-Night-Sky";

    private static AssetBundle assetBundle;

    private static GameObject? moon;
    private static UpdateMoon updateMoon;

    private static GameObject? starSphere;

    private static GameObject? shootingStar;
    private static UpdateShootingStar updateShootingStar;

    public static int ShootingStarsFrequency
    {
        get => Settings.options.ShootingStarsFrequency;
    }

    public override void OnApplicationStart()
    {
        Settings.OnLoad();

        Initialize();
    }

    private static void Initialize()
    {
        LoadEmbeddedAssetBundle();

        uConsole.RegisterCommand("toggle-night-sky", new System.Action(ToggleNightSky));
        uConsole.RegisterCommand("moon-phase", new System.Action(MoonPhase));
        uConsole.RegisterCommand("shooting-star", new System.Action(ShootingStar));
    }

    private static void LoadEmbeddedAssetBundle()
    {
        MemoryStream memoryStream;
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BetterNightSky.res.better-night-sky"))
        {
            memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);
        }
        if (memoryStream.Length == 0)
        {
            throw new System.Exception("No data loaded!");
        }
        assetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
    }

    internal static void ForcePhase(int phase)
    {
        if (updateMoon != null)
        {
            updateMoon.SetForcedPhase(phase);
        }
    }

    internal static void Install()
    {
        if (Settings.options.Sky && starSphere == null)
        {
            starSphere = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/StarSphere.prefab"));
            if (starSphere == null)
            {
                MelonLoader.MelonLogger.Error("starSphere was instantiated null");
            }

            starSphere.transform.parent = GameManager.GetUniStorm()?.m_StarSphere?.transform?.parent;
            starSphere.transform.localEulerAngles = new Vector3(0, 90, 0);
            starSphere.layer = GameManager.GetUniStorm().m_StarSphere.layer;
            starSphere?.AddComponent<UpdateStars>();

            moon = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/Moon.prefab"));
            if (moon == null)
            {
                MelonLoader.MelonLogger.Error("moon was instantiated null");
            }

            moon.transform.parent = GameManager.GetUniStorm()?.m_StarSphere?.transform?.parent?.parent;
            moon.layer = GameManager.GetUniStorm().m_StarSphere.layer;
            updateMoon = moon?.AddComponent<UpdateMoon>();
            updateMoon.MoonPhaseTextures = GetMoonPhaseTextures();

            GameManager.GetUniStorm()?.m_StarSphere?.SetActive(false);
        }

        if (!Settings.options.Sky && starSphere != null)
        {
            Object.Destroy(starSphere);
            Object.Destroy(moon);
            GameManager.GetUniStorm().m_StarSphere.SetActive(true);
        }

        if (Settings.options.ShootingStarsFrequency > 0 && shootingStar == null)
        {
            shootingStar = Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/ShootingStar.prefab"));
            shootingStar.transform.parent = GameManager.GetUniStorm().m_StarSphere.transform.parent.parent;
            updateShootingStar = shootingStar.AddComponent<UpdateShootingStar>();
        }

        if (Settings.options.ShootingStarsFrequency == 0 && shootingStar != null)
        {
            Object.Destroy(shootingStar);
        }
    }

    internal static void RescheduleShootingStars()
    {
        if (shootingStar != null)
        {
            updateShootingStar.Reschedule();
        }
    }

    internal static void Log(string message) => MelonLoader.MelonLogger.Msg(message);

    internal static void UpdateMoonPhase()
    {
        if (updateMoon != null)
        {
            updateMoon.UpdatePhase();
        }
    }

    internal static Texture2D GetMoonPhaseTexture(int i)
    {
        return assetBundle.LoadAsset<Texture2D>("assets/MoonPhase/Moon_" + i + ".png");
    }

    private static Texture2D[] GetMoonPhaseTextures()
    {
        Texture2D[] result = new Texture2D[24];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = GetMoonPhaseTexture(i);
        }

        return result;
    }

    private static void MoonPhase()
    {
        int numParameter = uConsole.GetNumParameters();
        if (numParameter != 1)
        {
            uConsole.Log("Expected one parameter: Moon Phase Index");
            return;
        }

        ForcePhase(uConsole.GetInt());
    }

    private static void ShootingStar()
    {
        if (shootingStar == null)
        {
            uConsole.Log("Shooting Stars are disabled");
            return;
        }

        int duration = 5;
        if (uConsole.GetNumParameters() == 1)
        {
            duration = uConsole.GetInt();
        }

        updateShootingStar.Trigger(duration);
    }

    private static void ToggleNightSky()
    {
        GameObject originalStarSphere = GameManager.GetUniStorm().m_StarSphere;

        starSphere.SetActive(originalStarSphere.activeSelf);
        moon.SetActive(originalStarSphere.activeSelf);
        originalStarSphere.SetActive(!originalStarSphere.activeSelf);
    }
}