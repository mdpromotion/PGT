using System.Collections.Generic;
using UnityEngine;

namespace MGeLabs.DayNightSystem
{
    /// <summary>
    /// Controls one or more ParticleSystems as part of the day-night visualizer.
    /// This module can modify emission rate, particle lifetime and particle color
    /// based on a normalized time value (0..1) provided by the visualizer.
    /// </summary>
    public class DayNightParticleSystemModule : DayNightVisualModule
    {
        [Header("Setup")]
        [Tooltip("Particle systems that will be updated by this module.")]
        [SerializeField] protected List<ParticleSystem> particleSystems;

        [Header("Emission")]
        [Tooltip("Enable or disable emission rate updates for the particle systems.")]
        [SerializeField] protected bool useEmission;
        [Tooltip("Emission rate values sampled across the day cycle.")]
        [SerializeField] protected List<float> emissionValues = new() { 100, 200, 200, 100 };

        [Header("Lifetime")]
        [Tooltip("Enable or disable particle lifetime updates for the particle systems.")]
        [SerializeField] protected bool useLifetime;
        [Tooltip("Particle lifetime values sampled across the day cycle.")]
        [SerializeField] protected List<float> lifetimeValues = new() { 3, 5, 5, 3 };

        [Header("Color")]
        [Tooltip("Enable or disable particle color updates for the particle systems.")]
        [SerializeField] protected bool useColor = true;
        [Tooltip("Gradient used to sample particle color over the day cycle (0..1).")]
        [SerializeField] protected Gradient colorValues = new()
        {
            colorKeys = new GradientColorKey[]
            {
                new(Color.white, 0f),
                new(Color.white, 1f),
            },
            alphaKeys = new GradientAlphaKey[]
            {
                new(1f, 0f),
                new(1f, 1f),
            },
        };

        /// <summary>
        /// Update the configured particle systems to reflect the given time of day.
        /// </summary>
        /// <param name="timePercentage">
        /// Normalized time value in the range (0,1) representing the current point
        /// in the day-night cycle. Used to sample lists via <see cref="DayNightVisualizer.Lerp"/>
        /// and to evaluate the color gradient.
        /// </param>
        public override void UpdateVisual(float timePercentage)
        {
            foreach (ParticleSystem system in particleSystems)
            {
                ParticleSystem.MainModule main = system.main;
                if (useLifetime) main.startLifetime = DayNightVisualizer.Lerp(lifetimeValues, timePercentage);
                if (useColor) main.startColor = colorValues.Evaluate(timePercentage);

                if (useEmission)
                {
                    ParticleSystem.EmissionModule emission = system.emission;
                    emission.rateOverTime = DayNightVisualizer.Lerp(emissionValues, timePercentage);
                }
            }
        }
    }
}