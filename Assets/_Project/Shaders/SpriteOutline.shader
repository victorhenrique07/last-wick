// Desenha só o contorno de 1 pixel em volta de um sprite, sem iluminação, pulsando devagar.
// Usado num SpriteRenderer filho, por cima do sprite original, para marcar o que é clicável.
// O sprite precisa de pelo menos 1 pixel transparente em volta e Mesh Type = Full Rect.
Shader "Last Wick/Sprite Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        [HDR] _OutlineColor ("Cor do contorno", Color) = (1, 0.82, 0.5, 1)
        _PulseSpeed ("Velocidade do pulso", Float) = 3
        _PulseMin ("Brilho mínimo do pulso", Range(0, 1)) = 0.55
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float3 positionOS : POSITION;
            float4 color : COLOR;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 color : COLOR;
            float2 uv : TEXCOORD0;
        };

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize; // fora do buffer do material: o SpriteRenderer preenche com a textura de cada sprite

        CBUFFER_START(UnityPerMaterial)
            half4 _OutlineColor;
            float _PulseSpeed;
            half _PulseMin;
        CBUFFER_END

        Varyings vert(Attributes input)
        {
            Varyings output;
            output.positionCS = TransformObjectToHClip(input.positionOS);
            output.color = input.color;
            output.uv = input.uv;
            return output;
        }

        half Alpha(float2 uv)
        {
            return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
        }

        half4 frag(Varyings input) : SV_Target
        {
            // Dentro do sprite não desenha nada: o sprite original já está lá
            if (Alpha(input.uv) > 0.5) discard;

            // Fora do sprite, só pinta se algum vizinho (cima, baixo, lados) for opaco
            float2 texel = _MainTex_TexelSize.xy;
            half neighbors = max(max(Alpha(input.uv + float2(texel.x, 0)), Alpha(input.uv - float2(texel.x, 0))),
                                 max(Alpha(input.uv + float2(0, texel.y)), Alpha(input.uv - float2(0, texel.y))));
            if (neighbors < 0.5) discard;

            half pulse = lerp(_PulseMin, 1, 0.5 + 0.5 * sin(_Time.y * _PulseSpeed));
            half4 color = _OutlineColor * input.color;
            color.a *= pulse;
            return color;
        }
        ENDHLSL

        // Renderer 2D do URP
        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        // Renderer padrão do URP (caso a câmera não use o Renderer 2D)
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
}
