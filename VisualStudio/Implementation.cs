using System.IO;
using System.Reflection;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace BetterNightSky;

internal sealed class Implementation : MelonLoader.MelonMod
{
    private const string NAME = "Better-Night-Sky";

    private static AssetBundle assetBundle = LoadEmbeddedAssetBundle();

	private static GameObject? moon;
    private static UpdateMoon? updateMoon;
    private static GameObject? starSphere;
    private static GameObject? shootingStar;
    private static UpdateShootingStar? updateShootingStar;

    public static int ShootingStarsFrequency
    {
        get => Settings.options.ShootingStarsFrequency;
    }

    public override void OnInitializeMelon()
    {
        Settings.OnLoad();

        uConsole.RegisterCommand("toggle-night-sky", new System.Action(ToggleNightSky));
        uConsole.RegisterCommand("moon-phase", new System.Action(MoonPhase));
        uConsole.RegisterCommand("shooting-star", new System.Action(ShootingStar));

        Debug.Log($"[{Info.Name}] version {Info.Version} loaded");
        new MelonLoader.MelonLogger.Instance($"{Info.Name}").Msg($"Version {Info.Version} loaded");
    }

    private static AssetBundle LoadEmbeddedAssetBundle()
    {
        MemoryStream memoryStream;
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Better-Night-Sky.res.better-night-sky"))
        {
            memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);
        }
        if (memoryStream.Length == 0)
        {
            throw new System.Exception("No data loaded!");
        }
        return AssetBundle.LoadFromMemory(memoryStream.ToArray());
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
            starSphere = UnityEngine.Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/StarSphere.prefab"));
            if (starSphere == null)
            {
                MelonLoader.MelonLogger.Error("starSphere was instantiated null");
                return;
            }

            starSphere.transform.parent = GameManager.GetUniStorm()?.m_StarSphere?.transform?.parent;
            starSphere.transform.localEulerAngles = new Vector3(0, 90, 0);
            starSphere.layer = GameManager.GetUniStorm().m_StarSphere.layer;
            starSphere?.AddComponent<UpdateStars>();

            moon = UnityEngine.Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/Moon.prefab"));
            if (moon == null)
            {
                MelonLoader.MelonLogger.Error("moon was instantiated null");
                return;
            }

            moon.transform.parent = GameManager.GetUniStorm()?.m_StarSphere?.transform?.parent?.parent;
            moon.layer = GameManager.GetUniStorm().m_StarSphere.layer;
            updateMoon = moon?.AddComponent<UpdateMoon>();

            GameManager.GetUniStorm()?.m_StarSphere?.SetActive(false);
        }

        if (!Settings.options.Sky && starSphere != null)
        {
            UnityEngine.Object.Destroy(starSphere);
            UnityEngine.Object.Destroy(moon);
            GameManager.GetUniStorm().m_StarSphere.SetActive(true);
        }

        if (Settings.options.ShootingStarsFrequency > 0 && shootingStar == null)
        {
            shootingStar = UnityEngine.Object.Instantiate(assetBundle.LoadAsset<GameObject>("assets/ShootingStar.prefab"));
            shootingStar.transform.parent = GameManager.GetUniStorm().m_StarSphere.transform.parent.parent;
            updateShootingStar = shootingStar.AddComponent<UpdateShootingStar>();
        }

        if (Settings.options.ShootingStarsFrequency == 0 && shootingStar != null)
        {
            UnityEngine.Object.Destroy(shootingStar);
        }
    }

    internal static void RescheduleShootingStars()
    {
        if (updateShootingStar != null)
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
        if (updateShootingStar == null)
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