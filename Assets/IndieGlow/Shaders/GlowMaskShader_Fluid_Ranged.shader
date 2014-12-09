Shader "IndieGlow/ObjectShader/GlowMaskShader_Fluid_Ranged" {
	Properties {
		_Layer_1_Color ("Layer 1 Color", Color) = (1,1,1,1)
		_Layer_1_Tex ("Layer 1 (RGB)", 2D) = "white" {}
 		_Layer_1_TileX ("X Tiling", Float) = 1
		_Layer_1_TileY ("Y Tiling", Float) = 1
		_Layer_1_Intensity ("Layer 1 Intesity", Range(0.000, 0.025)) = 0.003
		_Layer_1_Rate ("Layer 1 Rate", Range(0.0, 100)) = 100
		_Layer_1_ScrollX ("Layer 1 UV X Scroll", Range(-2, 2)) = 0
		_Layer_1_ScrollY ("Layer 1 UV Y Scroll", Range(-2, 2)) = 0
		
		_Layer_2_Color ("Layer 2 Color", Color) = (0,0,0,1)
		_Layer_2_Tex ("Layer 2 (RGB)", 2D) = "white" {}
 		_Layer_2_TileX ("X Tiling", Float) = 1
		_Layer_2_TileY ("Y Tiling", Float) = 1
		_Layer_2_Intensity ("Layer 2 Intesity", Range(0.000, 0.025)) = 0.003
		_Layer_2_Rate ("Layer 2 Rate", Range(0.0, 100)) = 100
		_Layer_2_ScrollX ("Layer 2 UV X Scroll", Range(-2, 2)) = 0
		_Layer_2_ScrollY ("Layer 2 UV Y Scroll", Range(-2, 2)) = 0
		
		_Layer_3_Color ("Layer 3 Color", Color) = (0,0,0,1)
		_Layer_3_Tex ("Layer 3 (RGB)", 2D) = "white" {}
 		_Layer_3_TileX ("X Tiling", Float) = 1
		_Layer_3_TileY ("Y Tiling", Float) = 1
		_Layer_3_Intensity ("Layer 3 Intesity", Range(0.000, 0.025)) = 0.003
		_Layer_3_Rate ("Layer 3 Rate", Range(0.0, 100)) = 100
		_Layer_3_ScrollX ("Layer 3 UV X Scroll", Range(-2, 2)) = 0
		_Layer_3_ScrollY ("Layer 3 UV Y Scroll", Range(-2, 2)) = 0
		
		_GlowStrength ("Glow Intesity", Range(0.0, 1)) = 1
		_GlowRange ("Glow Range", float) = 100
	}
	SubShader {
        Blend SrcAlpha One
        Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
        Lighting Off
        Cull Off
        Fog { Mode Off }
        Offset 0, -10
        ZWrite Off
		
		Pass {
			Tags {"LightMode"="Always"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			#pragma target 3.0
			#include "UnityCG.cginc"

			fixed4 _Layer_1_Color;
			sampler2D _Layer_1_Tex;
			half _Layer_1_TileX;
			half _Layer_1_TileY;
			half _Layer_1_Intensity;
			half _Layer_1_Rate;
			half _Layer_1_ScrollX;
			half _Layer_1_ScrollY;
			
			fixed4 _Layer_2_Color;
			sampler2D _Layer_2_Tex;
			half _Layer_2_TileX;
			half _Layer_2_TileY;
			half _Layer_2_Intensity;
			half _Layer_2_Rate;
			half _Layer_2_ScrollX;
			half _Layer_2_ScrollY;
			
			fixed4 _Layer_3_Color;
			sampler2D _Layer_3_Tex;
			half _Layer_3_TileX;
			half _Layer_3_TileY;
			half _Layer_3_Intensity;
			half _Layer_3_Rate;
			half _Layer_3_ScrollX;
			half _Layer_3_ScrollY;
			
			half _GlowStrength;
			half _GlowRange;
		
			struct appdata {
    			half4 vertex : POSITION;
    			half4 texcoord : TEXCOORD0;
    			half4 projPos : TEXCOORD1;
			};
 
			struct v2f {
    			half4 pos : SV_POSITION;
    			half4 uv : TEXCOORD0;
    			half4 projPos : TEXCOORD1;
			};
			
			v2f vert (appdata v) {
    			v2f o;
    			o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
    			o.uv = half4( v.texcoord.xy, 0, 0 );
    			o.projPos = ComputeScreenPos (v.vertex);
    			UNITY_TRANSFER_DEPTH(o.projPos.z);
    			//COMPUTE_EYEDEPTH(o.projPos.z);
    			return o;
			}
    		
    		half4 frag( v2f v ) : COLOR
			{
				half4 result = half4(0,0,0,0);
				half rate = _Layer_1_Rate;
				half intensity = _Layer_1_Intensity;
				half2 tiling = half2(_Layer_1_TileX, _Layer_1_TileY);
        		half2 Offset = half2(_Layer_1_ScrollX, _Layer_1_ScrollY);
        		half2 uv = half2(sin(_Time.w + v.uv.x * rate), cos(_Time.w + v.uv.y * rate));
        		result = tex2D(_Layer_1_Tex, ((v.uv.xy * tiling.xy) + _Time.x * Offset.xy) + intensity * uv) * _Layer_1_Color;
				
				rate = _Layer_2_Rate;
				intensity = _Layer_2_Intensity;
				tiling = half2(_Layer_2_TileX, _Layer_2_TileY);
        		Offset = half2(_Layer_2_ScrollX, _Layer_2_ScrollY);
        		uv = half2(sin(_Time.w + v.uv.x * rate), cos(_Time.w + v.uv.y * rate));
        		result += tex2D(_Layer_2_Tex, ((v.uv.xy * tiling.xy) + _Time.x * Offset.xy) + intensity * uv) * _Layer_2_Color;
        		
        		rate = _Layer_3_Rate;
				intensity = _Layer_3_Intensity;
				tiling = half2(_Layer_3_TileX, _Layer_3_TileY);
        		Offset = half2(_Layer_3_ScrollX, _Layer_3_ScrollY);
        		uv = half2(sin(_Time.w + v.uv.x * rate), cos(_Time.w + v.uv.y * rate));
        		result += tex2D(_Layer_3_Tex, ((v.uv.xy * tiling.xy) + _Time.x * Offset.xy) + intensity * uv) * _Layer_3_Color;
				half depth = v.projPos.z / _GlowRange;
				depth = 1.0 - depth;
				result.rgb *= _GlowStrength;
				result.a = depth;
				return result;
			}
			ENDCG
		}
	}
	Fallback off
}