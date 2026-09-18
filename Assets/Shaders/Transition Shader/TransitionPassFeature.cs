using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class TransitionPassFeature : ScriptableRendererFeature
{
    public static TransitionPassFeature Instance { get; private set; }

    [Serializable]
    public class TransitionPassSettings
    {
        [Header("Material")]
        public Material transitionMaterial;

        [Header("Transition")]
        [Range(0f, 1f)]
        public float cutoff = 0f;

        public Color transitionColor = Color.black;

        [Tooltip(
            "Grayscale transition mask. " +
            "Black areas disappear first; white areas disappear last."
        )]
        public Texture2D transitionTexture;

        [Header("Edge")]
        [Range(0f, 0.2f)]
        public float edgeSoftness = 0.01f;
    }

    private class TransitionPass : ScriptableRenderPass
    {
        private readonly TransitionPassSettings settings;

        private static readonly int CutoffID =
            Shader.PropertyToID("_Cutoff");

        private static readonly int ColorID =
            Shader.PropertyToID("_Color");

        private static readonly int TransitionTexID =
            Shader.PropertyToID("_TransitionTex");

        private static readonly int EdgeSoftnessID =
            Shader.PropertyToID("_EdgeSoftness");

        private const string PassName = "Transition Pass";

        public TransitionPass(TransitionPassSettings settings)
        {
            this.settings = settings;
        }

        public override void RecordRenderGraph(
            RenderGraph renderGraph,
            ContextContainer frameData)
        {
            if (settings.transitionMaterial == null)
            {
                return;
            }

            UniversalResourceData resourceData =
                frameData.Get<UniversalResourceData>();

            // The back buffer cannot be sampled as a texture.
            if (resourceData.isActiveTargetBackBuffer)
            {
                return;
            }

            TextureHandle source =
                resourceData.activeColorTexture;

            var destinationDesc =
                renderGraph.GetTextureDesc(source);

            destinationDesc.name =
                "Transition Camera Color";

            destinationDesc.clearBuffer = false;
            destinationDesc.depthBufferBits = 0;

            TextureHandle destination =
                renderGraph.CreateTexture(destinationDesc);

            // Update material properties.
            settings.transitionMaterial.SetFloat(
                CutoffID,
                settings.cutoff
            );

            settings.transitionMaterial.SetColor(
                ColorID,
                settings.transitionColor
            );

            settings.transitionMaterial.SetFloat(
                EdgeSoftnessID,
                settings.edgeSoftness
            );

            if (settings.transitionTexture != null)
            {
                settings.transitionMaterial.SetTexture(
                    TransitionTexID,
                    settings.transitionTexture
                );
            }

            var blitParameters =
                new RenderGraphUtils.BlitMaterialParameters(
                    source,
                    destination,
                    settings.transitionMaterial,
                    0
                );

            renderGraph.AddBlitPass(
                blitParameters,
                PassName
            );

            resourceData.cameraColor = destination;
        }
    }

    [SerializeField]
    public TransitionPassSettings settings =
        new TransitionPassSettings();

    private TransitionPass transitionPass;

    public override void Create()
    {
        Instance = this;

        transitionPass = new TransitionPass(settings);

        transitionPass.renderPassEvent = RenderPassEvent.AfterRendering;
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (settings.transitionMaterial == null)
        {
            return;
        }

        renderer.EnqueuePass(transitionPass);
    }

    protected override void Dispose(bool disposing)
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetCutoff(float value)
    {
        settings.cutoff =
            Mathf.Clamp01(value);
    }

    public float GetCutoff()
    {
        return settings.cutoff;
    }

    public void SetTransitionColor(Color color)
    {
        settings.transitionColor = color;
    }

    public void SetTransitionTexture(Texture2D texture)
    {
        settings.transitionTexture = texture;
    }

    public void SetEdgeSoftness(float softness)
    {
        settings.edgeSoftness =
            Mathf.Clamp(softness, 0f, 0.2f);
    }
}
