using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;


public class GrassBendingRTPrePass : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        static readonly int GrassBendingRT_Id = Shader.PropertyToID("_GrassBendingRT");
        RTHandle GrassBendingRT;
        ShaderTagId GrassBendingTag = new ShaderTagId("GrassBending");

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Allocate an RTHandle for the grass bending render target
            GrassBendingRT = RTHandles.Alloc(
                width: 512,
                height: 512,
                colorFormat: GraphicsFormat.R8_UNorm,
                depthBufferBits: DepthBits.None,
                name: "_GrassBendingRT"
            );

            ConfigureTarget(GrassBendingRT);
            ConfigureClear(ClearFlag.All, Color.white);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!InstancedIndirectGrassRenderer.instance)
            {
                Debug.LogWarning("InstancedIndirectGrassRenderer not found. Skipping GrassBendingRTPrePass.");
                return;
            }

            var cmd = CommandBufferPool.Get("GrassBendingRTPass");

            // Compute the view matrix for a top-down orthographic projection
            Vector3 grassCenter = InstancedIndirectGrassRenderer.instance.transform.position;
            Vector3 cameraPosition = grassCenter + Vector3.up; // 1 unit above the grass center
            Quaternion cameraRotation = Quaternion.LookRotation(-Vector3.up); // Looking straight down
            Matrix4x4 viewMatrix = Matrix4x4.TRS(cameraPosition, cameraRotation, new Vector3(1, 1, -1)).inverse;

            // Compute the orthographic projection matrix based on the grass area
            float sizeX = InstancedIndirectGrassRenderer.instance.transform.localScale.x;
            float sizeZ = InstancedIndirectGrassRenderer.instance.transform.localScale.z;
            Matrix4x4 projectionMatrix = Matrix4x4.Ortho(-sizeX, sizeX, -sizeZ, sizeZ, 0.5f, 1.5f);

            // Set the custom view and projection matrices
            cmd.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
            context.ExecuteCommandBuffer(cmd);

            // Draw all objects tagged with the GrassBending shader pass
            var drawingSettings = CreateDrawingSettings(GrassBendingTag, ref renderingData, SortingCriteria.CommonTransparent);
            var filteringSettings = new FilteringSettings(RenderQueueRange.all);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            // Restore the original camera matrices
            cmd.Clear();
            cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

            // Set the render target globally for further use
            cmd.SetGlobalTexture(GrassBendingRT_Id, GrassBendingRT);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Release the allocated RTHandle
            GrassBendingRT?.Release();
        }
    }

    CustomRenderPass customRenderPass;

    public override void Create()
    {
        customRenderPass = new CustomRenderPass
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPrePasses
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(customRenderPass);
    }
}
