Shader "ERB/Particles/Premulti"
{
    Properties
    {
        [HDR]_TintColor("Tint Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
        _Opacity("Opacity", Range(0, 1)) = 0.5
        _Emission("Emission", Float) = 2
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "PreviewType" = "Plane" }
        Blend One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _TintColor;
                half _Opacity;
                half _Emission;
            CBUFFER_END

            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 vertexColor : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 vertexColor : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.vertexColor = v.vertexColor;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                half4 texCol = tex * _TintColor;
                half3 emissive = texCol.rgb * i.vertexColor.rgb * _Emission * i.vertexColor.a;
                half4 finalRGBA = half4(emissive, texCol.a * i.vertexColor.a * _Opacity);
                return finalRGBA;
            }
            ENDHLSL
        }
    }
}
