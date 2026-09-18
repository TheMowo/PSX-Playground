Shader "Custom/TransitionUI"
{
    Properties
    {
        [PerRendererData] _MainTex ("UI Texture", 2D) = "white" {}
        _TransitionTex ("Transition Texture", 2D) = "white" {}
        _Color ("Transition Color", Color) = (0, 0, 0, 1)
        _Cutoff ("Cutoff", Range(0, 1)) = 1.0
        _EdgeSoftness ("Edge Softness", Range(0, 0.2)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "TransitionUI"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _TransitionTex;
            float4 _Color;
            float _Cutoff;
            float _EdgeSoftness;
            float4 _TransitionTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.uv, _TransitionTex);
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float transition = tex2D(_TransitionTex, IN.uv).r;
                float maskValue = 1.0 - transition;

                float softness = max(_EdgeSoftness, 0.0001);

                float sceneMask = smoothstep(
                    _Cutoff - softness,
                    _Cutoff + softness,
                    maskValue
                );

                // Clamp extremes: cutoff=0 -> fully transparent, cutoff=1 -> fully black
                if (_Cutoff <= 0.0)
                {
                    return fixed4(0,0,0,0); // fully transparent
                }
                else if (_Cutoff >= 1.0)
                {
                    return fixed4(_Color.rgb, 1.0); // fully opaque black (or chosen color)
                }

                // Normal blended transition
                float alpha = 1.0 - sceneMask;
                fixed4 result = _Color;
                result.a = alpha * IN.color.a;
                return result;
            }
            ENDHLSL
        }
    }
}
