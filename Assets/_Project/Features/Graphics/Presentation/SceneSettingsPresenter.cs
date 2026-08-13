using _Project.Features.Core.Domain;
using _Project.Features.Graphics.Domain;
using UnityEngine;
using UnityEngine.Rendering;
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
        public void Construct()
        {
            GraphicsData data = new GraphicsData(GraphicsType.High, new ShadowQualityMode(ShadowQuality.All, 500), AntiAliasingMode.Msaa4, 10);
            
            _graphicsState = new GraphicsState(data);
        }


        private void Awake()
        {
            // temporary stub, I'll refactor this soon using di injection
            Construct();
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

            
            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpAsset)
            {
                urpAsset.shadowDistance = _graphicsState.ShadowQualityMode.ShadowDistance;
                
                QualitySettings.shadows = _graphicsState.ShadowQualityMode.ShadowQuality;
            }
        }
        
        private void ApplyAntiAliasing()
        {
            UniversalAdditionalCameraData cameraData =
                mainCamera.GetUniversalAdditionalCameraData();

            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpAsset)
            {
                switch (_graphicsState.AntiAliasingMode)
                {
                    case AntiAliasingMode.None:
                        cameraData.antialiasing = AntialiasingMode.None;
                        urpAsset.msaaSampleCount = 1;
                        break;

                    case AntiAliasingMode.Fxaa:
                        cameraData.antialiasing =
                            AntialiasingMode.FastApproximateAntialiasing;
                        urpAsset.msaaSampleCount = 1;
                        break;

                    case AntiAliasingMode.Taa:
                        cameraData.antialiasing =
                            AntialiasingMode.TemporalAntiAliasing;
                        urpAsset.msaaSampleCount = 1;
                        break;

                    case AntiAliasingMode.Msaa4:
                        cameraData.antialiasing = AntialiasingMode.None;
                        urpAsset.msaaSampleCount = 4;
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            _graphicsState.GraphicsChanged -= ApplyGraphicsSettings;
        }
    }
}
