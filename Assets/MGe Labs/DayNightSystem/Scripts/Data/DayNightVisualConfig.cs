using System.Collections.Generic;
using UnityEngine;

namespace MGeLabs.DayNightSystem
{
    /// <summary>
    /// Configuration for visual aspects of the day-night cycle, including skybox, fog, and lighting settings.
    /// </summary>
    [CreateAssetMenu(fileName = "DayNightVisualConfig", menuName = "MGe Labs/DayNightSystem/Visual Config")]
    public class DayNightVisualConfig : ScriptableObject
    {
        [Header("Skybox")]
        [Tooltip(
            "The skybox material used throughout the day-night cycle. This material will be dynamically modified during the cycle.")]
        public Material skyMaterial;
        [Tooltip(
            "Controls how the skybox transitions visually over the course of the day. Values correspond to points in the day (0 = midnight, 1 = noon).")]
        public List<float> skyMaterialTransitionValues = new() { 1, 0, 0, 1 };

        [Header("Fog")]
        [Tooltip(
            "Enable or disable fog in the scene. When enabled, fog settings will be applied based on the time of day.")]
        public bool enableFog = true;
        [Tooltip(
            "Controls how far the fog extends at different times of day. Use 4 values for morning, noon, evening, and night. Will only work with Fog mode set to 'Linear'.")]
        public List<float> fogDistances = new() { 75, 200, 200, 75 };
        [Tooltip(
            "Color of the fog as it changes throughout the day. This affects the scene's atmosphere and visibility.")]
        public Gradient fogColor;

        [Header("Sky Fog / Horizon")]
        [Tooltip("Enable or disable additional fog effect applied to the skybox material.")]
        public bool enableSkyFog = true;
        [Tooltip(
            "Fog color specifically applied to the skybox material. Use this to fine-tune visual blending between the sky and fog.")]
        public Gradient skyMaterialFogColor;
        [Tooltip(
            "Controls the density of the sky fog at different times of day. Use 4 values for morning, noon, evening, and night.")]
        public List<float> skyFogDensities = new() { 0.008f, 0.01f, 0.01f, 0.008f };

        [Header("Sun & Stars")]
        [Tooltip("The direction of the sun.")]
        public float sunDirection = 170f;
        [Tooltip("Enable or disable star visibility in the skybox material.")]
        public bool enableStars = true;

        [Header("Color Gradients")]
        [Tooltip("Enable or disable color gradients for ambient light and directional light.")]
        public bool enableColors = true;
        [Tooltip("The ambient light color used at different times of day. Affects overall scene lighting.")]
        public Gradient ambientLightColor;
        [Tooltip("Color of the main directional light (sun) throughout the day. Controls light tint and tone.")]
        public Gradient directionalLightColor;
    }
}