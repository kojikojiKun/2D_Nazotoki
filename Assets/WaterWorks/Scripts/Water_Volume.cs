using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source;
        private Material _material;

        private RTHandle tempRT;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            // Allocate temporary RT
            RenderingUtils.ReAllocateIfNeeded(ref tempRT, desc, name: "_WaterTempRT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            var cmd = CommandBufferPool.Get("Water_Volume");

            // New API replacement for Blit
            Blitter.BlitCameraTexture(cmd, source, tempRT, _material, 0);
            Blitter.BlitCameraTexture(cmd, tempRT, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Release temp RT
            if (tempRT != null)
                tempRT.Release();
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingTransparents;
    }

    public _Settings settings = new _Settings();
    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = Resources.Load<Material>("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.source = renderer.cameraColorTargetHandle;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
