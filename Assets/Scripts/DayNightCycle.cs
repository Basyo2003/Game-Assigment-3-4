using System;
using UnityEngine;

/// <summary>
/// Simple day / night cycle for Unity.
/// - Rotates a directional light to simulate sun movement.
/// - Supports a full cycle duration in minutes (default 24 minutes -> 1 minute = 1 hour of game-time when using 24h mapping).
/// - Exposes color Gradient and intensity AnimationCurve so you can tweak dawn/day/dusk/night looks.
/// - If no Light is assigned, will try to use RenderSettings.sun or find the first Directional light in scene.
/// </summary>
[ExecuteAlways]
public class DayNightCycle : MonoBehaviour
{
    [Tooltip("Duration of a full day in minutes. Default 24 minutes -> a full 24-hour cycle.")]
    public float dayDurationMinutes = 24f;

    [Tooltip("Start hour in 24h (0-24). 0 = midnight, 12 = noon")]
    [Range(0f, 24f)]
    public float startHour = 12f;

    [Tooltip("Optional directional light used as the sun. If null the script will try RenderSettings.sun or find one in scene.")]
    public Light sunLight;

    [Tooltip("Gradient that defines sun light color over the normalized day [0..1]. 0=midnight, 0.5=noon, 1=midnight")] 
    public Gradient sunColor = new Gradient();

    [Tooltip("Sun intensity over the normalized day [0..1].")]
    public AnimationCurve sunIntensity = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    [Tooltip("If true the cycle will progress while in Edit mode.")]
    public bool runInEditMode = false;

    [Tooltip("Whether time progresses automatically.")]
    public bool autoProgress = true;

    [Tooltip("Game time multiplier (1.0 = normal speed). Useful for debugging.")]
    public float timeScale = 1f;

    [SerializeField, HideInInspector]
    private float _timeOfDayNormalized = 0f; // 0..1 where 0 = midnight

    private float _secondsPerDay;

    void OnValidate()
    {
        dayDurationMinutes = Mathf.Max(0.01f, dayDurationMinutes);
        startHour = Mathf.Clamp(startHour, 0f, 24f);
        _secondsPerDay = dayDurationMinutes * 60f;
        // default gradient and curve sensible setup
        if (sunColor == null)
        {
            sunColor = new Gradient();
        }
        if (sunIntensity == null || sunIntensity.keys.Length == 0)
        {
            sunIntensity = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        }
    }

    void Reset()
    {
        // sensible defaults: bright white at noon, warm at dawn/dusk, dark at night
        sunColor = new Gradient();
        var keys = new GradientColorKey[3];
        keys[0].color = new Color(0.05f, 0.06f, 0.15f); // night
        keys[0].time = 0f;
        keys[1].color = new Color(1f, 0.95f, 0.85f); // day
        keys[1].time = 0.5f;
        keys[2].color = keys[0].color; // night
        keys[2].time = 1f;
        var alpha = new GradientAlphaKey[3];
        alpha[0].alpha = 1f; alpha[0].time = 0f;
        alpha[1].alpha = 1f; alpha[1].time = 0.5f;
        alpha[2].alpha = 1f; alpha[2].time = 1f;
        sunColor.SetKeys(keys, alpha);

        sunIntensity = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 0.6f),
            new Keyframe(0.5f, 1f),
            new Keyframe(0.75f, 0.6f),
            new Keyframe(1f, 0f)
        );

        dayDurationMinutes = 24f;
        startHour = 12f;
        runInEditMode = false;
        autoProgress = true;
        timeScale = 1f;
    }

    void Awake()
    {
        _secondsPerDay = Mathf.Max(0.01f, dayDurationMinutes) * 60f;
        // initialize time of day normalized by startHour
        _timeOfDayNormalized = Mathf.Repeat(startHour / 24f, 1f);
        if (sunLight == null)
            FindSunLight();
    }

    void Update()
    {
        if (!Application.isPlaying && !runInEditMode)
            return;

        if (sunLight == null)
            FindSunLight();

        if (autoProgress)
        {
            var delta = Time.deltaTime * timeScale;
            _timeOfDayNormalized += delta / _secondsPerDay;
            _timeOfDayNormalized = Mathf.Repeat(_timeOfDayNormalized, 1f);
        }

        ApplySunTransformAndLighting(_timeOfDayNormalized);
    }

    private void ApplySunTransformAndLighting(float normalized)
    {
        if (sunLight == null) return;

        // Map normalized [0..1] -> angle 0..360, where 0.25 = sunrise east horizon, 0.5 = noon (zenith), 0.75 = sunset west horizon
        float sunAngle = normalized * 360f - 90f; // offset so 0 = midnight (-90 puts midnight below horizon)
        // rotate around X axis to simulate daily arc
        sunLight.transform.localRotation = Quaternion.Euler(new Vector3(sunAngle, 170f, 0f));

        // color & intensity
        sunLight.color = sunColor.Evaluate(normalized);
        sunLight.intensity = sunIntensity.Evaluate(normalized);

        // toggle shadows and enabled when below horizon (simple threshold)
        bool aboveHorizon = Mathf.Sin(normalized * Mathf.PI * 2f) > 0f; // simple check
        sunLight.enabled = true; // keep enabled so color/intensity can be 0 — user controls preferred

        // Optionally update ambient lighting (basic)
        RenderSettings.ambientLight = sunLight.color * Mathf.Clamp01(sunLight.intensity * 0.25f);
    }

    private void FindSunLight()
    {
        if (RenderSettings.sun != null)
        {
            sunLight = RenderSettings.sun;
            return;
        }

        // find a directional Light in scene
        var lights = GameObject.FindObjectsOfType<Light>();
        foreach (var l in lights)
        {
            if (l.type == LightType.Directional)
            {
                sunLight = l;
                return;
            }
        }
    }

    /// Public helper to set time of day by hour (0..24)
    public void SetHour(float hour)
    {
        _timeOfDayNormalized = Mathf.Repeat(hour / 24f, 1f);
        ApplySunTransformAndLighting(_timeOfDayNormalized);
    }

    /// Returns current hour in 0..24
    public float GetHour() => _timeOfDayNormalized * 24f;
}
