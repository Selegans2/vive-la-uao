//https://forum.unity3d.com/threads/playing-360-videos-with-the-videoplayer.461290/
Shader "360/Equirectangular"
{
	Properties
	{
        [NoScaleOffset] _MainTex ("Panorama (HDR)", 2D) = "grey" {}
	}

	SubShader
	{
		Pass
		{
			Tags {"LightMode" = "Always"}

			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
			};

			v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
			{
				v2f outCoords = { UnityObjectToClipPos(vertex), normal };
				return outCoords;
			}

			#define ONE_OVER_PI .31830988618379067154F

			inline float2 ToRadialCoords(float3 coords)
			{
				float3 normalizedCoords = normalize(coords);
				float latitude = acos(normalizedCoords.y);
				float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
				float2 sphereCoords = float2(longitude, latitude) * ONE_OVER_PI;
				return float2(0.5F - sphereCoords.x * 0.5F, 1.0F - sphereCoords.y);
			}

			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
			    float2 equirectangularUV = ToRadialCoords(i.normal);
			    return tex2D(_MainTex, equirectangularUV);
			}
			ENDCG
		}
	}
	FallBack "VertexLit"
}
