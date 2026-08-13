using _Project.Features.Graphics.Domain;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;
using ShadowQuality = UnityEngine.ShadowQuality;

namespace _Project.Features.Graphics.Presentation
{
    public class SceneSettingsPresenter : MonoBehaviour
    {
        [SerializeField] private Light mainLight;
        [SerializeField] private UnityEngine.Camera mainCamera;
        
        private GraphicsState _graphicsState;

        [Inject]
        public void Construct(GraphicsState state)
        {
            _graphicsState = state;

            ApplyGraphicsSettings();
            
            _graphicsState.GraphicsChanged += ApplyGraphicsSettings;
        }

        private void ApplyGraphicsSettings()
        {
            ApplyShadowSettings();
            ApplyAntiAliasing();
        }

        private void ApplyShadowSettings()
        {
            mainLight.shadows = _graphicsState.ShadowQualityMode.ShadowQuality switch
            {
                ShadowQuality.Disable => LightShadows.None,
                ShadowQuality.HardOnly => LightShadows.Hard,
                ShadowQuality.All => LightShadows.Soft,
                _ => LightShadows.None
            };
        }
        
        private void ApplyAntiAliasing()
        {
            UniversalAdditionalCameraData cameraData =
                mainCamera.GetUniversalAdditionalCameraData();
            
            switch (_graphicsState.AntiAliasingMode)
            {
                case AntiAliasingMode.None:
                    cameraData.antialiasing = AntialiasingMode.None;
                    break;

                case AntiAliasingMode.Fxaa:
                    cameraData.antialiasing =
                        AntialiasingMode.FastApproximateAntialiasing;
                    break;

                case AntiAliasingMode.Taa:
                    cameraData.antialiasing =
                        AntialiasingMode.TemporalAntiAliasing;
                    break;

                case AntiAliasingMode.Msaa4:
                    cameraData.antialiasing = AntialiasingMode.None;
                    break;
            }
        }

        private void OnDestroy()
        {
            _graphicsState.GraphicsChanged -= ApplyGraphicsSettings;
        }
    }
}
