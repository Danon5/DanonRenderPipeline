using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

#if UNITY_EDITOR
        private const string c_material_path = "Assets/Plugins/DanonRenderPipeline/Materials/";
        private const string c_unlit_path = c_material_path + "DRP-Unlit-Default.mat";
        
        public override Material defaultMaterial => AssetDatabase.LoadAssetAtPath<Material>(c_unlit_path);
        public override Material defaultParticleMaterial => AssetDatabase.LoadAssetAtPath<Material>(c_unlit_path);
        public override Material defaultLineMaterial => AssetDatabase.LoadAssetAtPath<Material>(c_unlit_path);
#endif
    }
}