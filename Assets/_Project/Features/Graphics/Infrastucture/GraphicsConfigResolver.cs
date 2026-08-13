using System.Collections.Generic;
using _Project.Features.Graphics.Domain;
using _Project.Features.UI.Infrastructure;
using _Project.Features.UI.Menus.SettingsMenu;
using UnityEngine;

namespace _Project.Features.Graphics.Infrastucture
{
    public interface IGraphicsConfigResolver
    {
        List<SettingsEntry> ConvertFromGraphicData(
            GraphicsData data,
            List<SettingsEntry> cachedValues);

        GraphicsData? GetDefaultGraphicsData(GraphicsType graphicsType);
    }

    [CreateAssetMenu(menuName = "Settings/GraphicsConfigResolver")]
    public class GraphicsConfigResolver : ScriptableObject, IGraphicsConfigResolver
    {
        [SerializeField] private List<GraphicsPreset> presets;
        [SerializeField] private GraphicsQualityConfig qualityConfig;

        public List<SettingsEntry> ConvertFromGraphicData(
            GraphicsData data,
            List<SettingsEntry> cachedValues)
        {
            var result = cachedValues ?? new List<SettingsEntry>();

            var shadowDistanceType = qualityConfig
                .GetShadowDistanceEntry(data.ShadowQualityMode.ShadowDistance)
                .graphicsType;

            var viewDistanceType = qualityConfig
                .GetViewDistanceEntry(data.ViewDistance)
                .graphicsType;

            SetOrAdd(result, SettingsMenuMode.Quality, (int)data.QualityMode);
            SetOrAdd(result, SettingsMenuMode.ShadowQuality, (int)data.ShadowQualityMode.ShadowQuality);
            SetOrAdd(result, SettingsMenuMode.ShadowDistance, (int)shadowDistanceType);
            SetOrAdd(result, SettingsMenuMode.AntiAliasing, (int)data.AntiAliasingMode);
            SetOrAdd(result, SettingsMenuMode.WindowMode, (int)data.WindowMode);
            SetOrAdd(result, SettingsMenuMode.VSync, data.VSync ? 1 : 0);
            SetOrAdd(result, SettingsMenuMode.ViewDistance, (int)viewDistanceType);

            return result;
        }

        public GraphicsData? GetDefaultGraphicsData(GraphicsType graphicsType)
        {
            GraphicsPreset item = null;

            foreach (var t in presets)
            {
                if (t.Category == graphicsType)
                {
                    item = t;
                    break;
                }
            }

            if (!item)
                return null;
            
            var shadowDistance = qualityConfig
                .GetShadowDistanceEntry(graphicsType)
                .shadowDistance;

            var viewDistance = qualityConfig
                .GetViewDistanceEntry(graphicsType)
                .viewDistance;
            
            var shadowData = new ShadowQualityMode(item.ShadowQuality.quality, shadowDistance);
            var data = new GraphicsData(item.Category, shadowData, item.AntiAliasingMode, item.WindowMode, item.VSync, viewDistance);       

            return data;
        }

        private static void SetOrAdd(List<SettingsEntry> entries, SettingsMenuMode mode, int value)
        {
            var newEntry = new SettingsEntry(mode, value);

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Mode == mode)
                {
                    entries[i] = newEntry;
                    return;
                }
            }

            entries.Add(newEntry);
        }
    }
}