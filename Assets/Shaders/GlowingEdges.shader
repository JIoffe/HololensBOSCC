//Simple 'Glowing Edges' effect based on fresnel term

Shader "Custom/GlowingEdges"
{
	Properties
	{
        [Header(Main Color)]
        [Toggle] _UseColor("Enabled?", Float) = 0
        _Color("Main Color", Color) = (1,1,1,1)
        [Space(20)]

        [Header(Base(RGB))]
        [Toggle] _UseMainTex("Enabled?", Float) = 1
		[NoScaleOffset] _MainTex("Base (RGB)", 2D) = "white" {}
		[Space(20)]

		[Header(Glow Color)]
		_GlowColor("Glow Color", Color) = (1,1,1,1)

		[Header(Misc.)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Cull[_Cull]

		Pass{
			CGPROGRAM
			// We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
			#pragma target 5.0
			#pragma only_renderers d3d11

			#pragma shader_feature _USECOLOR_ON
			#pragma shader_feature _USEMAINTEX_ON
			#pragma shader_feature IS_SPRITE

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#if _USEMAINTEX_ON
				UNITY_DECLARE_TEX2D(_MainTex);
			#endif

			#if _USECOLOR_ON
				float4 _Color;
			#endif

			float4 _GlowColor;
			const fixed Texel_Size = 0.01;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal: NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				half3 normal : TEXCOORD1;
				half3 viewDir : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv;
				
				o.normal = UnityObjectToWorldNormal(v.normal);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.viewDir =  _WorldSpaceCameraPos - worldPos.xyz;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				#if _USEMAINTEX_ON
					float4 color = UNITY_SAMPLE_TEX2D(_MainTex, i.uv);

					if (color.a < 0.5)
						discard;
				#else
					float4 color = 1;
				#endif

				#if _USECOLOR_ON
					color *= _Color;
				#endif

				half fresnel;

				#if IS_SPRITE
					#if _USEMAINTEX_ON
						//Scan for edges in alpha-mapped cutouts
						fresnel = 1. - UNITY_SAMPLE_TEX2D(_MainTex, i.uv + fixed2(0, Texel_Size)).a *
							UNITY_SAMPLE_TEX2D(_MainTex, i.uv - fixed2(0, Texel_Size)).a *
							UNITY_SAMPLE_TEX2D(_MainTex, i.uv + fixed2(Texel_Size, 0)).a *
							UNITY_SAMPLE_TEX2D(_MainTex, i.uv - fixed2(Texel_Size, 0)).a;

					#endif
				#else
					fresnel = 1. - max(0., dot(normalize(i.viewDir), normalize(i.normal)));
				#endif

				return lerp(color, _GlowColor, fresnel);
			}

			ENDCG
		}
	}
}
