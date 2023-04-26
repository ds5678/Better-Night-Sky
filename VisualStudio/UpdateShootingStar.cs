using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace BetterNightSky;

[MelonLoader.RegisterTypeInIl2Cpp]
internal sealed class UpdateShootingStar : MonoBehaviour
{
    private const float ALPHA_MAX = 0.75f;
    private const float COLOR_MAX = 0.90f;
    private const float COLOR_MIN = 0.60f;

    private const int DELAY_MAX = 1800;
    private const int DELAY_MIN = 900;
    private const int DURATION_MAX = 30;
    private const int DURATION_MIN = 2;

    private const float HEIGHT = 2000;
    private const float POSITION_MAX = 2000;
    private const float POSITION_MIN = -2000;

    private const int PARTICLES_MAX = 30;
    private const int PARTICLES_MIN = 1;
    private ParticleSystem? _particleSystem;

    private ParticleSystem ParticleSystem 
    { 
        get
        {
			if (_particleSystem == null)
            {
				_particleSystem = GetComponentInChildren<ParticleSystem>();
				if (_particleSystem == null)
				{
					gameObject.SetActive(false);
					throw new System.NullReferenceException("Particle system not found!");
				}
			}
            return _particleSystem;
		}
        set => _particleSystem = value; 
    }

    public UpdateShootingStar(System.IntPtr intPtr) : base(intPtr) { }

    [HideFromIl2Cpp]
    internal void Trigger(int duration = 0)
    {
        CancelInvoke();

        int actualDuration = duration < 1 ? UnityEngine.Random.Range(DURATION_MIN, DURATION_MAX) : duration;
        Invoke("StopEmitting", actualDuration);

        StartEmitting();
    }

    private static int GetNextDelay()
    {
        int result = UnityEngine.Random.Range(DELAY_MIN, DELAY_MAX) / Implementation.ShootingStarsFrequency;

        if (IsMainMenu())
        {
            result /= 10;
        }

        return result;
    }

    private static int GetNextDuration()
    {
        return UnityEngine.Random.Range(DURATION_MIN, DURATION_MAX);
    }

    internal static bool IsMainMenu()
    {
        return "MainMenu" == GameManager.m_ActiveScene;
    }

    [HideFromIl2Cpp]
    private bool CanEmit()
    {
        return !GameManager.GetWeatherComponent().IsIndoorScene() 
            && GameManager.GetUniStorm().GetActiveTODState().m_MoonAlpha >= 0.05;
    }

    private void Start()
    {
        StopEmitting();
    }

    private void StartEmitting()
    {
        if (!CanEmit())
        {
            return;
        }

        UpdatePosition();
        UpdateColor();

        ParticleSystem.EmissionModule emissionModule = ParticleSystem.emission;
        emissionModule.enabled = true;
    }

    private void StopEmitting()
    {
        ParticleSystem.EmissionModule emissionModule = ParticleSystem.emission;
        emissionModule.enabled = false;
    }

    internal void Reschedule()
    {
        CancelInvoke();
        StopEmitting();

        int delay = GetNextDelay();
        Invoke("StartEmitting", delay);

        int duration = GetNextDuration();
        Invoke("Reschedule", delay + duration);

        Implementation.Log("Scheduled next shooting stars in " + delay + " seconds for " + duration + " seconds.");
    }

    private void UpdateColor()
    {
        float currentAlpha = Mathf.Clamp(GameManager.GetUniStorm().GetActiveTODState().m_MoonAlpha, 0, ALPHA_MAX);        

        Color minColor = new Color(UnityEngine.Random.Range(COLOR_MIN, COLOR_MAX), UnityEngine.Random.Range(COLOR_MIN, COLOR_MAX), UnityEngine.Random.Range(COLOR_MIN, COLOR_MAX), currentAlpha);
        Color maxColor = new Color(COLOR_MAX, COLOR_MAX, COLOR_MAX, currentAlpha);
        
        var gradient = new ParticleSystem.MinMaxGradient() { m_ColorMin = minColor, m_ColorMax = maxColor };        
        ParticleSystem.MainModule mainModule = ParticleSystem.main;

        // Commented as setting color currently causing a crash to desktop under some conditions
        //mainModule.startColor = gradient;
        mainModule.maxParticles = UnityEngine.Random.Range(PARTICLES_MIN, PARTICLES_MAX);
    }

    private void UpdatePosition()
    {
        if (IsMainMenu())
        {
            ParticleSystem.transform.position = new Vector3(-2500, 2000, -2000);
            ParticleSystem.transform.rotation = Quaternion.identity;
            return;
        }

        ParticleSystem.transform.position = new Vector3(UnityEngine.Random.Range(POSITION_MIN, POSITION_MAX), HEIGHT, UnityEngine.Random.Range(POSITION_MIN, POSITION_MAX));
        ParticleSystem.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.value * 360, 0);
    }
}