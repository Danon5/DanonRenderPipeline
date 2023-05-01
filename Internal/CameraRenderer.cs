using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline.Internal {
    internal sealed class CameraRenderer {
        private const string c_buffer_name = "Render Camera";
        private static readonly ShaderTagId s_unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
        private readonly CommandBuffer m_buffer;
        private ScriptableRenderContext m_context;
        private Camera m_camera;

        internal CameraRenderer() {
            m_buffer = new CommandBuffer {
                name = c_buffer_name
            };
        }

        public void Render(in ScriptableRenderContext context, in Camera camera) {
            m_context = context;
            m_camera = camera;

            if (!Cull(out var cullingResults)) return;
            
            Setup();
            DrawVisibleGeometry(cullingResults);
            Submit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteBuffer() {
            m_context.ExecuteCommandBuffer(m_buffer);
            m_buffer.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Setup() {
            m_context.SetupCameraProperties(m_camera);
            m_buffer.ClearRenderTarget(true, true, Color.clear);
            m_buffer.BeginSample(c_buffer_name);
            ExecuteBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Cull(out CullingResults results) {
            if (m_camera.TryGetCullingParameters(out var cullParams)) {
                results = m_context.Cull(ref cullParams);
                return true;
            }

            results = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawVisibleGeometry(in CullingResults cullingResults) {
            var sortingSettings = new SortingSettings(m_camera) {
                criteria = SortingCriteria.CommonOpaque
            };
            var drawingSettings = new DrawingSettings(s_unlitShaderTagId, sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.all);
            m_context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
            m_context.DrawSkybox(m_camera);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Submit() {
            m_buffer.EndSample(c_buffer_name);
            ExecuteBuffer();
            m_context.Submit();
        }
    }
}