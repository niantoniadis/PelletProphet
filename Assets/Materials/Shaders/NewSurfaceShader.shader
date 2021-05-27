Shader "Tutorial/028_voronoi_noise/2d" {
	Properties{
		_CellSize("Cell Size", Range(0, 2)) = 2
	}
		SubShader{
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}

			CGPROGRAM

			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			#include "Random.cginc"

			float _CellSize;

			struct Input {
				float3 worldPos;
			};

			float voronoiNoise(float2 value) {
				float2 cell = floor(value);
				float2 cellPosition = cell + rand2dTo2d(cell);
				float2 toCell = cellPosition - value;
				float distToCell = length(toCell);
				return distToCell;
			}

			void surf(Input i, inout SurfaceOutputStandard o) {
				float2 value = i.worldPos.xz / _CellSize;
				float noise = voronoiNoise(value);

				o.Albedo = noise;
			}
			ENDCG
	}
		FallBack "Standard"
}