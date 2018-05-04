Shader "Hidden/Threshold"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _ThresholdPoint ("Threshold point", Range(0.0, 1.0)) = 0.5
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
			float _ThresholdPoint;

			fixed4 frag (v2f i) : SV_Target
			{
				const float redSensitivity = 0.3;
				const float greenSensitivity = 0.59;
				const float blueSensitivity = 0.11;
				const float3 perceptionSensitivities = float3(redSensitivity, greenSensitivity, blueSensitivity);
				
				const float3 black = float3(0, 0, 0);
				const float3 white = float3(1, 1, 1);
				
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float rgbDotProd = dot(col.rgb, perceptionSensitivities);
				
                col.rgb = rgbDotProd >= _ThresholdPoint ? (1, 1, 1): (0, 0, 0); 

				return col;
			}
			ENDCG
		}
	}
}
