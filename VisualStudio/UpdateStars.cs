using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace BetterNightSky;

[MelonLoader.RegisterTypeInIl2Cpp]
internal sealed class UpdateStars : MonoBehaviour
{
    private Color baseColor;
    private float lastAlpha = -1;
    private Material? _material;

	private Material Material
	{
		get
		{
			if (_material == null)
			{
				_material = GetComponent<Renderer>().material;
			}
			return _material;
		}
	}

	public UpdateStars(System.IntPtr intPtr) : base(intPtr) { }

    public void Start()
    {
        baseColor = Material.GetColor("_TintColor");
    }

    public void Update()
    {
        float currentAlpha = GameManager.GetUniStorm().GetActiveTODState().m_StarsAlpha;
        if (Mathf.Approximately(lastAlpha, currentAlpha))
        {
            return;
        }

        lastAlpha = currentAlpha;
        Material.SetColor("_TintColor", baseColor * lastAlpha * lastAlpha);
    }
}