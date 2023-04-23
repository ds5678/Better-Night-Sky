using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace BetterNightSky;

[MelonLoader.RegisterTypeInIl2Cpp]
internal sealed class UpdateMoon : MonoBehaviour
{
    public const int MOON_CYCLE_DAYS = 29;

    private Texture2D[] MoonPhaseTextures = System.Array.Empty<Texture2D>();

    private Color baseColor;
    private int forcedPhase = -1;
    private float lastAlpha = -1;
    private int lastPhaseTextureIndex = -1;
    private Material material;

    public UpdateMoon(System.IntPtr intPtr) : base(intPtr) { }

    [HideFromIl2Cpp]
    public void SetForcedPhase(int forcedPhase)
    {
        this.forcedPhase = Mathf.Clamp(forcedPhase, 0, MoonPhaseTextures.Length);
        UpdatePhase();
    }

    public void Start()
    {
        MoonPhaseTextures = GetMoonPhaseTextures();
        material = GetComponentInChildren<Renderer>().material;
        baseColor = material.GetColor("_TintColor");
        UpdatePhase();
    }

    public void Update()
    {
        UpdateAlpha();
        UpdateDirection();
    }

    public void UpdatePhase()
    {
        if (MoonPhaseTextures == null || material == null)
        {
            return;
        }

        int phaseTextureIndex = GetPhaseTextureIndex();
        if (lastPhaseTextureIndex == phaseTextureIndex)
        {
            return;
        }

        lastPhaseTextureIndex = phaseTextureIndex;
        //material.mainTexture = MoonPhaseTextures[lastPhaseTextureIndex];
        material.mainTexture = Implementation.GetMoonPhaseTexture(lastPhaseTextureIndex);
    }

    [HideFromIl2Cpp]
    private int GetPhaseTextureIndex()
    {
        if (forcedPhase >= 0)
        {
            return forcedPhase;
        }

        UniStormWeatherSystem uniStormWeatherSystem = GameManager.GetUniStorm();
        int day = uniStormWeatherSystem.GetDayNumber() + uniStormWeatherSystem.m_MoonCycleStartDay;
        return day % MOON_CYCLE_DAYS * MoonPhaseTextures.Length / MOON_CYCLE_DAYS;
    }

    private void UpdateAlpha()
    {
        float currentAlpha = GameManager.GetUniStorm().GetActiveTODState().m_MoonAlpha;
        if (Mathf.Approximately(lastAlpha, currentAlpha))
        {
            return;
        }

        lastAlpha = currentAlpha;
        material.SetColor("_TintColor", baseColor * lastAlpha);
    }

    private void UpdateDirection()
    {
        transform.forward = -GameManager.GetUniStorm().m_MoonLight.transform.forward;
	}

	private static Texture2D[] GetMoonPhaseTextures()
	{
		Texture2D[] result = new Texture2D[24];
		for (int i = 0; i < result.Length; i++)
		{
			result[i] = Implementation.GetMoonPhaseTexture(i);
		}

		return result;
	}
}