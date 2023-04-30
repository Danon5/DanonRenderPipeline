using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline {
    [CreateAssetMenu(
        fileName = nameof(DanonRenderPipelineAsset),
        menuName = "Rendering/" + nameof(DanonRenderPipelineAsset))]
    public sealed class DanonRenderPipelineAsset : RenderPipelineAsset {
        protected override RenderPipeline CreatePipeline() {
            return new DanonRenderPipelineInstance(this);
        }
    }
}