using System.Collections.Generic;
using UnityEngine;

namespace MGeLabs.DayNightSystem
{
    /// <summary>
    /// Handles the visualization of the day-night cycle, including updating lighting, fog, and skybox settings
    /// based on the current time of day.
    /// </summary>
    public class DayNightVisualizer
    {
        // Shader property IDs
        protected static readonly int FogColor = Shader.PropertyToID("_FogColor");
        protected static readonly int FogDensity = Shader.PropertyToID("_FogDensity");
        protected static readonly int Transition = Shader.PropertyToID("_Transition");
        private static readonly int StarIntensity = Shader.PropertyToID("_StarIntensity");

        protected readonly DayNightVisualConfig config;
        protected readonly Light directionalLight;
        protected readonly List<DayNightVisualModule> visualModules;

        protected Material skyMaterial;

        /// <summary>
        /// Initializes a new instance of the <see cref="DayNightVisualizer"/> class.
        /// Sets up the skybox material based on the provided material.
        /// </summary>
        /// <param name="config">The configuration for the day-night cycle visuals.</param>
        /// <param name="directionalLight">The directional light representing the sun.</param>
        /// <param name="visualModules">A list of visual modules that can be used to extend the visualizer's functionality.</param>
        public DayNightVisualizer(
            DayNightVisualConfig config,
            Light directionalLight,
            List<DayNightVisualModule> visualModules
        )
        {
            skyMaterial = new Material(config.skyMaterial);

            this.config = config;
            this.directionalLight = directionalLight;
            this.visualModules = visualModules;
        }

        /// <summary>
        /// Initializes the visualizer by setting up the skybox, ambient mode, and sun.
        /// </summary>
        public virtual void Init()
        {
            RenderSettings.skybox = skyMaterial;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.sun = directionalLight;

            if (!config.enableSkyFog)
            {
                skyMaterial.SetFloat(FogDensity, 0f);
            }

            if (!config.enableStars)
            {
                skyMaterial.SetFloat(StarIntensity, 0f);
            }
        }

        /// <summary>
        /// Updates the lighting, skybox and modules based on the current time of day.
        /// </summary>
        /// <param name="timePercentage">Current time as a percentage (0-1).</param>
        public virtual void Update(float timePercentage)
        {
            ApplyColors(timePercentage);
            ApplyFog(timePercentage);
            ApplySkyFog(timePercentage);
            ApplySkyAndSunTransition(timePercentage);
            ApplyUpdatesToModules(timePercentage);
        }

        /// <summary>
        /// Applies ambient light and directional light colors based on the time of day.
        /// </summary>
        /// <param name="timePercentage">Current time as a percentage (0-1).</param>
        protected virtual void ApplyColors(float timePercentage)
        {
            if (!config.enableColors) return;

            // Set ambient light and directional light colors
            RenderSettings.ambientLight = config.ambientLightColor.Evaluate(timePercentage);
            directionalLight.color = config.directionalLightColor.Evaluate(timePercentage);

            if (config.enableFog)
            {
                // Set fog color
                RenderSettings.fogColor = config.fogColor.Evaluate(timePercentage);
            }
        }

        /// <summary>
        /// Applies fog distance settings based on the time of day.
        /// </summary>
        /// <param name="timePercentage">Current time as a percentage (0-1).</param>
        protected virtual void ApplyFog(float timePercentage)
        {
            if (!config.enableFog) return;

            // Set fog end distance
            RenderSettings.fogEndDistance = Lerp(config.fogDistances, timePercentage);
        }

        /// <summary>
        /// Applies skybox fog color and density based on the time of day.
        /// </summary>
        /// <param name="timePercentage">Current time as a percentage (0-1).</param>
        protected virtual void ApplySkyFog(float timePercentage)
        {
            if (!config.enableSkyFog || !skyMaterial) return;

            // Set skybox fog color
            Color materialFogColor = config.skyMaterialFogColor.Evaluate(timePercentage);
            skyMaterial.SetColor(FogColor, materialFogColor);

            // Set skybox fog density
            skyMaterial.SetFloat(FogDensity, Lerp(config.skyFogDensities, timePercentage));
        }

        /// <summary>
        /// Applies skybox transition and rotates the sun based on the time of day.
        /// </summary>
        /// <param name="timePercentage">Current time as a percentage (0-1).</param>
        protected virtual void ApplySkyAndSunTransition(float timePercentage)
        {
            if (skyMaterial)
            {
                // Set skybox transition value
                float skyTransitionValue = Lerp(config.skyMaterialTransitionValues, timePercentage);
                skyMaterial.SetFloat(Transition, skyTransitionValue);
            }

            // Rotate the sun based on the time of day
            float sunRotationX = timePercentage * 360f - 90f;
            float sunRotationY = config.sunDirection;
            directionalLight.transform.localRotation = Quaternion.Euler(sunRotationX, sunRotationY, 0f);
        }

        /// <summary>
        /// Forwards an update tick to all configured visual modules so they can update their
        /// visuals for the current time of day.
        /// </summary>
        /// <param name="timePercentage">Current time as a percentage (0-1), where 0 and 1 represent the same point in the cycle.</param>
        /// <remarks>
        /// This method is safe to call when there are no modules configured; it will simply return.
        /// Each module's <see cref="DayNightVisualModule.UpdateVisual"/> method is invoked with the
        /// provided time percentage.
        /// </remarks>
        protected virtual void ApplyUpdatesToModules(float timePercentage)
        {
            foreach (DayNightVisualModule dayNightVisualModule in visualModules)
            {
                dayNightVisualModule.UpdateVisual(timePercentage);
            }
        }

        /// <summary>
        /// Linearly interpolates between elements in a list of floats based on a parameter t.
        /// </summary>
        /// <param name="list">The list of floats to interpolate.</param>
        /// <param name="t">The interpolation parameter, typically between 0 and 1.</param>
        /// <returns>The interpolated float value.</returns>
        public static float Lerp(List<float> list, float t)
        {
            // Determine the indices of the start and end values
            int startIndex = (int)(t * (list.Count - 1));
            int endIndex = startIndex + 1;

            // Get the start and end values
            float startValue = list[startIndex];
            float endValue = list[endIndex];

            // Calculate the interpolation fraction
            float fraction = (t - (float)startIndex / (list.Count - 1)) * (list.Count - 1);

            // Return the interpolated value
            return Mathf.Lerp(startValue, endValue, fraction);
        }

        /// <summary>
        /// Cleans up resources by destroying the skybox material.
        /// </summary>
        public virtual void Cleanup()
        {
            if (skyMaterial)
            {
                Object.Destroy(skyMaterial);
                skyMaterial = null;
            }
        }
    }
}