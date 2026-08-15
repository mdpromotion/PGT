using System.Collections.Generic;
using _Project.Features.Graphics.Domain;
using _Project.Features.UI.Infrastructure;
using _Project.Features.UI.Menus.SettingsMenu;
using UnityEngine;
using VContainer;

namespace _Project.Features.Graphics.Infrastucture
{
    public interface IGraphicsConfigResolver
    {
        void ConvertFromGraphicData(
            GraphicsData data,
            Dictionary<SettingsMenuMode, int> cachedValues);

        void ConvertFromGraphicState(
            GraphicsState data,
            Dictionary<SettingsMenuMode, int> cachedValues);
        
        GraphicsData ConvertToGraphicData(
            IReadOnlyDictionary<SettingsMenuMode, int> cachedValues);

        GraphicsData? GetDefaultGraphicsData(GraphicsType graphicsType);
    }

    [CreateAssetMenu(menuName = "Settings/GraphicsConfigResolver")]
    public class GraphicsConfigResolver : ScriptableObject, IGraphicsConfigResolver
    {
        [SerializeField] private List<GraphicsPreset> presets;
        
        private GraphicsQualityConfig _qualityConfig;

        [Inject]
        public void Construct(GraphicsQualityConfig qualityConfig)
        {
            _qualityConfig = qualityConfig;
        }
        
        public void ConvertFromGraphicData(
            GraphicsData data,
            Dictionary<SettingsMenuMode, int> cachedValues)
        {
            var shadowDistanceType = _qualityConfig
                .GetShadowDistanceEntry(data.ShadowQualityMode.ShadowDistance)
                .graphicsType;

            Debug.Log(data.ViewDistance);
            
            var viewDistanceType = _qualityConfig
                .GetViewDistanceEntry(data.ViewDistance)
                .graphicsType;

            cachedValues[SettingsMenuMode.Quality] = (int)data.QualityMode;
            cachedValues[SettingsMenuMode.ShadowQuality] = (int)data.ShadowQualityMode.ShadowQuality;
            cachedValues[SettingsMenuMode.ShadowDistance] = (int)shadowDistanceType;
            cachedValues[SettingsMenuMode.AntiAliasing] = (int)data.AntiAliasingMode;
            cachedValues[SettingsMenuMode.WindowMode] = (int)data.WindowMode;
            cachedValues[SettingsMenuMode.VSync] = data.VSync ? 1 : 0;
            cachedValues[SettingsMenuMode.ViewDistance] = (int)viewDistanceType;
        }
        
        public void ConvertFromGraphicState(
            GraphicsState data,
            Dictionary<SettingsMenuMode, int> cachedValues)
        {
            var shadowDistanceType = _qualityConfig
                .GetShadowDistanceEntry(data.ShadowQualityMode.ShadowDistance)
                .graphicsType;

            var viewDistanceType = _qualityConfig
                .GetViewDistanceEntry(data.ViewDistance)
                .graphicsType;

            cachedValues[SettingsMenuMode.Quality] = (int)data.QualityMode;
            cachedValues[SettingsMenuMode.ShadowQuality] = (int)data.ShadowQualityMode.ShadowQuality;
            cachedValues[SettingsMenuMode.ShadowDistance] = (int)shadowDistanceType;
            cachedValues[SettingsMenuMode.AntiAliasing] = (int)data.AntiAliasingMode;
            cachedValues[SettingsMenuMode.WindowMode] = (int)data.WindowMode;
            cachedValues[SettingsMenuMode.VSync] = data.VSync ? 1 : 0;
            cachedValues[SettingsMenuMode.ViewDistance] = (int)viewDistanceType;
        }

        public GraphicsData ConvertToGraphicData(
            IReadOnlyDictionary<SettingsMenuMode, int> cachedValues)
        {
            var shadowDistanceType =
                (GraphicsType)GetValue(
                    cachedValues,
                    SettingsMenuMode.ShadowDistance);

            var viewDistanceType =
                (GraphicsType)GetValue(
                    cachedValues,
                    SettingsMenuMode.ViewDistance);

            var shadowQualityMode = new ShadowQualityMode(
                (ShadowQuality)GetValue(
                    cachedValues,
                    SettingsMenuMode.ShadowQuality),
                _qualityConfig
                    .GetShadowDistanceEntry(shadowDistanceType)
                    .shadowDistance
            );

            return new GraphicsData(
                (GraphicsType)GetValue(
                    cachedValues,
                    SettingsMenuMode.Quality),

                shadowQualityMode,

                (AntiAliasingMode)GetValue(
                    cachedValues,
                    SettingsMenuMode.AntiAliasing),

                (WindowMode)GetValue(
                    cachedValues,
                    SettingsMenuMode.WindowMode),

                GetValue(
                    cachedValues,
                    SettingsMenuMode.VSync) != 0,

                _qualityConfig
                    .GetViewDistanceEntry(viewDistanceType)
                    .viewDistance
            );
        }

        public GraphicsData? GetDefaultGraphicsData(GraphicsType graphicsType)
        {
            GraphicsPreset preset = null;

            foreach (var item in presets)
            {
                if (item.Category == graphicsType)
                {
                    preset = item;
                    break;
                }
            }

            if (!preset)
                return null;

            var shadowDistance = _qualityConfig
                .GetShadowDistanceEntry(graphicsType)
                .shadowDistance;

            var viewDistance = _qualityConfig
                .GetViewDistanceEntry(graphicsType)
                .viewDistance;

            var shadowData = new ShadowQualityMode(
                preset.ShadowQuality.quality,
                shadowDistance);

            return new GraphicsData(
                preset.Category,
                shadowData,
                preset.AntiAliasingMode,
                preset.WindowMode,
                preset.VSync,
                viewDistance);
        }

        private static int GetValue(
            IReadOnlyDictionary<SettingsMenuMode, int> values,
            SettingsMenuMode mode)
        {
            return values[mode];
        }
    }
}