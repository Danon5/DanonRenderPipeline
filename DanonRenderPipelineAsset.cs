using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline {
    [CreateAssetMenu(
        fileName = nameof(DanonRenderPipelineAsset),
        menuName = "Rendering/" + nameof(DanonRenderPipelineAsset))]
    public sealed class DanonRenderPipelineAsset : RenderPipelineAsset {
        private const string c_batch_settings = "Batch Settings";
        [SerializeField, BoxGroup(c_batch_settings)] private bool m_useDynamicBatching;
        [SerializeField, BoxGroup(c_batch_settings)] private bool m_useGPUInstancing;
        [SerializeField, BoxGroup(c_batch_settings)] private bool m_useSRPBatcher;
        
        protected override RenderPipeline CreatePipeline() {
            return new DanonRenderPipelineInstance(this, m_useDynamicBatching, m_useGPUInstancing, m_useSRPBatcher);
        }
    }
}