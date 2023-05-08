using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline.Internal {
    internal sealed class DRPCameraRenderer {
        private const string c_buffer_name = "Render Camera";
        private static readonly ShaderTagId s_unlitShaderTagId;
        private static readonly ShaderTagId s_litShaderTagId;
        private static readonly ShaderTagId[] s_legacyShaderTagIds;
#if UNITY_EDITOR
        private static readonly Material s_errorMaterial;
#endif
        private readonly DRPLighting m_lighting;
        private readonly CommandBuffer m_buffer;
        private ScriptableRenderContext m_context;
        private Camera m_camera;
        private string m_sampleName;

        static DRPCameraRenderer() {
            s_unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
            s_litShaderTagId = new ShaderTagId("DRPLit");
            s_legacyShaderTagIds = new ShaderTagId[] {
                new("Always"),
                new("ForwardBase"),
                new("PrepassBase"),
                new("Vertex"),
                new("VertexLMRGBM"),
                new("VertexLM")
            };
#if UNITY_EDITOR
            s_errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
#endif
        }

        internal DRPCameraRenderer() {
            m_lighting = new DRPLighting();
            m_buffer = new CommandBuffer {
                name = c_buffer_name
            };
        }

        public void Start(ScriptableRenderContext context, in Camera camera) {
            m_context = context;
            m_camera = camera;
            
#if UNITY_EDITOR
            PrepareBuffer();
            PrepareForSceneWindow();
#endif
        }
        
        public void Render(bool useDynamicBatching, bool useGPUInstancing) {
            if (!Cull(out var cullingResults)) return;

            Setup();
            m_lighting.Setup(m_context);
            DrawVisibleGeometry(cullingResults, useDynamicBatching, useGPUInstancing);
#if UNITY_EDITOR
            DrawUnsupportedShaders(cullingResults);
            DrawGizmos();
#endif
        }

        public void End() {
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
            var flags = m_camera.clearFlags;
            var flagsAreColor = flags == CameraClearFlags.Color; 
            m_buffer.ClearRenderTarget(
                flags <= CameraClearFlags.Depth, 
                flagsAreColor, 
                flagsAreColor ? m_camera.backgroundColor.linear : Color.clear);
#if UNITY_EDITOR
            m_buffer.BeginSample(m_sampleName);
#else
            m_buffer.BeginSample(c_buffer_name);
#endif
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
        private void DrawVisibleGeometry(in CullingResults cullingResults, bool useDynamicBatching, bool useGPUInstancing) {
            var sortingSettings = new SortingSettings(m_camera) {
                criteria = SortingCriteria.CommonOpaque
            };
            var drawingSettings = new DrawingSettings(s_unlitShaderTagId, sortingSettings) {
                enableDynamicBatching = useDynamicBatching,
                enableInstancing = useGPUInstancing
            };
            drawingSettings.SetShaderPassName(1, s_litShaderTagId);
            var filteringSettings = new FilteringSettings(RenderQueueRange.all);
            m_context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
            m_context.DrawSkybox(m_camera);
        }

#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawUnsupportedShaders(in CullingResults cullingResults) {
            var sortingSettings = new SortingSettings(m_camera);
            var drawingSettings = new DrawingSettings(s_legacyShaderTagIds[0], sortingSettings) {
                overrideMaterial = s_errorMaterial
            };
            for (var i = 1; i < s_legacyShaderTagIds.Length; i++)
                drawingSettings.SetShaderPassName(i, s_legacyShaderTagIds[i]);
            var filteringSettings = FilteringSettings.defaultValue;
            m_context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawGizmos() {
            if (m_camera.cameraType == CameraType.SceneView)
                m_context.DrawWireOverlay(m_camera);
            
            if (Handles.ShouldRenderGizmos()) {
                m_context.DrawGizmos(m_camera, GizmoSubset.PreImageEffects);
                m_context.DrawGizmos(m_camera, GizmoSubset.PostImageEffects);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PrepareBuffer() {
            var name = m_camera.name;
            m_buffer.name = name;
            m_sampleName = name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PrepareForSceneWindow() {
            if (m_camera.cameraType == CameraType.SceneView)
                ScriptableRenderContext.EmitWorldGeometryForSceneView(m_camera);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Submit() {
#if UNITY_EDITOR
            m_buffer.EndSample(m_sampleName);
#else
            m_buffer.EndSample(c_buffer_name);
#endif
            ExecuteBuffer();
            m_context.Submit();
        }
    }
}