using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private float radius = 3f;   // campo de apoio, aparece no Inspector

    public float Radius
    {
        get => radius;
        set
        {
            radius = value;
            ApplyToLight();      // roda toda vez que alguém muda o raio
        }
    }

    private Light2D light;

    void Awake()
    {
        light = GetComponentInChildren<Light2D>();
        ApplyToLight();
    }

    private void ApplyToLight()
    {
        if (light == null) return;

        light.pointLightOuterRadius = radius;
    }
}
