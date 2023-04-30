using DanonRenderPipeline.Internal;
using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline {
    public sealed class DanonRenderPipelineInstance : RenderPipeline {
        private readonly DanonRenderPipelineAsset m_asset;
        private readonly CameraRenderer m_cameraRenderer;

        public DanonRenderPipelineInstance(in DanonRenderPipelineAsset asset) {
            m_asset = asset;
            m_cameraRenderer = new CameraRenderer();
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            foreach (var camera in cameras)
                m_cameraRenderer.Render(context, camera);
        }
    }
}