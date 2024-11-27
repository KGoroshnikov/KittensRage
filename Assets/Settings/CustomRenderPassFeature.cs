using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class FastMobileBloomRenderFeature : ScriptableRendererFeature
{
    class FastMobileBloomPass : ScriptableRenderPass
    {
        private class PassData
        {
            public Material bloomMaterial;
            public float threshold;
            public float intensity;
            public float blurSize;
            public int blurIterations;

            public TextureHandle sourceTexture;
            public TextureHandle destinationTexture;
        }

        private Material _bloomMaterial;
        private float _threshold;
        private float _intensity;
        private float _blurSize;
        private int _blurIterations;

        public FastMobileBloomPass(Material bloomMaterial, float threshold, float intensity, float blurSize, int blurIterations)
        {
            _bloomMaterial = bloomMaterial;
            _threshold = threshold;
            _intensity = intensity;
            _blurSize = blurSize;
            _blurIterations = blurIterations;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            requiresIntermediateTexture = true;
        }

        /*static void ExecutePass(PassData data, RasterGraphContext context)
        {
            CommandBuffer cmd = context.cmd;

            int rtW = context.resourceProvider.GetTextureDescriptor(data.sourceTexture).width / 4;
            int rtH = context.resourceProvider.GetTextureDescriptor(data.sourceTexture).height / 4;

            RenderTextureDescriptor rtDesc = new RenderTextureDescriptor(rtW, rtH, RenderTextureFormat.Default);
            var downscaleRt = RenderTexture.GetTemporary(rtDesc);
            var currentRt = downscaleRt;

            // Initial Downsample
            data.bloomMaterial.SetFloat("_Spread", data.blurSize);
            data.bloomMaterial.SetVector("_ThresholdParams", new Vector2(1.0f, -data.threshold));
            Blit(cmd, data.sourceTexture, currentRt, data.bloomMaterial, 0);

            // Downscale loop
            for (int i = 0; i < data.blurIterations - 1; i++)
            {
                rtDesc.width /= 2;
                rtDesc.height /= 2;
                var rt = RenderTexture.GetTemporary(rtDesc);
                Blit(cmd, currentRt, rt, data.bloomMaterial, 1);
                RenderTexture.ReleaseTemporary(currentRt);
                currentRt = rt;
            }

            // Upscale loop
            for (int i = 0; i < data.blurIterations - 1; i++)
            {
                rtDesc.width *= 2;
                rtDesc.height *= 2;
                var rt = RenderTexture.GetTemporary(rtDesc);
                Blit(cmd, currentRt, rt, data.bloomMaterial, 2);
                RenderTexture.ReleaseTemporary(currentRt);
                currentRt = rt;
            }

            // Final Blit
            data.bloomMaterial.SetFloat("_BloomIntensity", data.intensity);
            data.bloomMaterial.SetTexture("_BloomTex", currentRt);
            Blit(cmd, data.sourceTexture, data.destinationTexture, data.bloomMaterial, 3);

            RenderTexture.ReleaseTemporary(currentRt);
        }*/

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "Fast Mobile Bloom";

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                var resourceData = frameData.Get<UniversalResourceData>();
                var source = resourceData.activeColorTexture;

                var destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = $"CameraColor-{passName}";
                destinationDesc.clearBuffer = false;

                TextureHandle destination = renderGraph.CreateTexture(destinationDesc);
        
                passData.bloomMaterial = _bloomMaterial;
                passData.threshold = _threshold;
                passData.intensity = _intensity;
                passData.blurSize = _blurSize;
                passData.blurIterations = _blurIterations;


                RenderGraphUtils.BlitMaterialParameters para = new(source, destination, _bloomMaterial, 0);
                renderGraph.AddBlitPass(para, passName: passName);

                resourceData.cameraColor = destination;
            }
            
        }
    }

    [SerializeField] private Material bloomMaterial;
    [Range(0.0f, 1.5f)] public float threshold = 0.25f;
    [Range(0.00f, 4.0f)] public float intensity = 1.0f;
    [Range(0.25f, 5.5f)] public float blurSize = 1.0f;
    [Range(1, 4)] public int blurIterations = 2;

    private FastMobileBloomPass _renderPass;

    public override void Create()
    {
        if (bloomMaterial == null)
        {
            Debug.LogError("Bloom Material not assigned.");
            return;
        }

        _renderPass = new FastMobileBloomPass(bloomMaterial, threshold, intensity, blurSize, blurIterations);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_renderPass);
    }
}
