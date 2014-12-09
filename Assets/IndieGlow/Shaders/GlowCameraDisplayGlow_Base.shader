Shader "IndieGlow/Base/GlowCameraDisplayGlow_Base" {
    Properties {
        _MSKTex ("Base (RGB)", 2D) = "white" {}
        _GlowColor ("Main Color", Color) = (1,1,1,0.5)
        _GlowStrength ("_Glow Strength", Range(0, 20)) = 1
        _BlurRadius ("Blur Radius", Range(0,10)) = 1
    }
        SubShader {
        Tags { "IgnoreProjector"="True" }
        Lighting Off
        Cull Off
        ZTest Always
        ZWrite Off
        Fog { Mode Off }
        Offset 0, -5
                
                Pass {
                        Blend One Zero
                        CGPROGRAM
                        #pragma vertex vert
                        #pragma fragment frag 
                        #pragma target 3.0
 
                        half4 _GlowColor;
                        sampler2D _MSKTex;
                        half SizeX;
                        half SizeY;
                        half2 _TexelSize;
                        half _GlowStrength;
                        half _BlurRadius;
                
                        struct appdata {
                                half4 vertex : POSITION;
                                half4 texcoord : TEXCOORD0;
                        };
 
                        struct v2f {
                                half4 pos : SV_POSITION;
                                half2 uv : TEXCOORD0;
                        };
                        
                        v2f vert (appdata v) {
                                v2f o;
                                o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
                                o.uv = v.texcoord.xy;
                                return o;
                        }
                        
                        half4 blur(half2 radius, half2 uvOriginal, half2 clampMax)
                        {
                                int Segments = 16;	// Min 4 / Max 20 Segments
                                
                                half PI = 3.141;
                                half4 result = half4(0,0,0,0);
                                
                                half2 offset = half2(0,0);
                                
                                for (int i = 0; i < Segments; ++i)
                                {
                               		half angle = PI*2.0 * ((i+1.0) / Segments);
                             		offset = half2(sin(angle), cos(angle)) * radius;
                              		half2 uv = clamp(uvOriginal + offset, half2(0,0), clampMax);
                                    
                                    half4 BlurRange = tex2D(_MSKTex, uv);
                            		
                            		offset = offset * BlurRange.a;
                            		uv = clamp(uvOriginal + offset, half2(0,0), clampMax);
                            		half4 col = tex2D(_MSKTex, uv);
                            		
                              		result.xyz += col.xyz;
                               	}
                              	result.xyz /= Segments;
                             	return result;
                        }
                
                        half4 frag( v2f v ) : COLOR
                        {
                            half2 maxSize = half2(SizeX, SizeY) - _TexelSize.xy * 2;
                            half2 radius = _TexelSize;
                            half4 result = 0;
                                
                           	result = blur(radius * (1 + _BlurRadius * 0), v.uv, maxSize) * 1;
                            result += blur(radius * (1 + _BlurRadius * 1), v.uv, maxSize) * 1;
                           	result += blur(radius * (1 + _BlurRadius * 2), v.uv, maxSize) * 1;
                            result.xyz /= 3;

                            result.xyz *=_GlowStrength;
                            return result;
                        }
                        ENDCG
                }
        }
        Fallback off
}

