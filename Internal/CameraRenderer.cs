using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace DanonRenderPipeline.Internal {
    internal sealed class CameraRenderer {
        private ScriptableRenderContext m_context;
        private Camera m_camera;
        
        public void Render(in ScriptableRenderContext context, in Camera camera) {
            m_context = context;
            m_camera = camera;
            
            Setup();
            DrawGeometry();
            Submit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Setup() {
            m_context.SetupCameraProperties(m_camera);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawGeometry() {
            m_context.DrawSkybox(m_camera);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Submit() {
            m_context.Submit();
        }
    }
}