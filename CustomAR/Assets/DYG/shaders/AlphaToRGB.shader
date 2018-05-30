Shader "Hidden/AlphaToRGB"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				const float redSensitivity = 0.3;
				const float greenSensitivity = 0.59;
				const float blueSensitivity = 0.11;

				fixed4 col = tex2D(_MainTex, i.uv);

                const float alphaVal = col.a;

				col.rgb = float3(alphaVal / redSensitivity, alphaVal / greenSensitivity, alphaVal / blueSensitivity);
                    
                //Make alpha opaque
                col.a = 1.0;

				return col;
			}
			ENDCG
		}
	}
}
