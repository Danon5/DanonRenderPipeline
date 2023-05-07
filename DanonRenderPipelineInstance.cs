using DanonRenderPipeline.Internal;
using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline {
    public sealed class DanonRenderPipelineInstance : RenderPipeline {
        private readonly DanonRenderPipelineAsset m_asset;
        private readonly CameraRenderer m_cameraRenderer;
        private readonly bool m_useDynamicBatching;
        private readonly bool m_useGPUInstancing;

        public DanonRenderPipelineInstance(in DanonRenderPipelineAsset asset, bool useDynamicBatching, 
            bool useGPUInstancing, bool useSRPBatcher) {
            m_asset = asset;
            m_cameraRenderer = new CameraRenderer();
            m_useDynamicBatching = useDynamicBatching;
            m_useGPUInstancing = useGPUInstancing;
            
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
            
           
            
            if (QualitySettings.activeColorSpace != ColorSpace.Linear)
                Debug.LogWarning($"The active color space is currently set to {QualitySettings.activeColorSpace}, " +
                                 "but it should be set to Linear. Rendering might not work properly.");
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            BeginFrameRendering(context, cameras);
            foreach (var camera in cameras) {
                m_cameraRenderer.Start(context, camera);
                BeginCameraRendering(context, camera);
                m_cameraRenderer.Render(m_useDynamicBatching, m_useGPUInstancing);
                EndCameraRendering(context, camera);
                m_cameraRenderer.End();
            }
            EndFrameRendering(context, cameras);
        }
    }
}