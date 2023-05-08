using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline.Internal {
    internal sealed class DRPLighting {
        private const string c_buffer_name = "Lighting";
        private static readonly int s_dirLightDir;
        private static readonly int s_dirLightCol;
        private readonly CommandBuffer m_buffer;

        static DRPLighting() {
            s_dirLightDir = Shader.PropertyToID("_DirLightDir");
            s_dirLightCol = Shader.PropertyToID("_DirLightColor");
        }

        public DRPLighting() {
            m_buffer = new CommandBuffer {
                name = c_buffer_name
            };
        }

        public void Setup(ScriptableRenderContext context) {
            m_buffer.BeginSample(c_buffer_name);
            SetupDirectionalLight();
            m_buffer.EndSample(c_buffer_name);
            context.ExecuteCommandBuffer(m_buffer);
            m_buffer.Clear();
        }

        private void SetupDirectionalLight() {
            var light = RenderSettings.sun;
            if (light == null) return;
            m_buffer.SetGlobalVector(s_dirLightDir, light.transform.forward);
            m_buffer.SetGlobalVector(s_dirLightCol, light.color * light.intensity);
        }
    }
}