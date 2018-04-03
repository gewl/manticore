// Adapted from Lindsey Reid (https://github.com/thelindseyreid/Unity-Shader-Tutorials/blob/master/Assets/Materials/Shaders/celEffects.shader)

Shader "Unlit/CelEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RampTex("Ramp", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_OutlineExtrusion("Outline Extrusion", float) = 0
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineDot("Outline Dot", float) = 0.25
	}
	SubShader
	{
		// Color and lighting pass
		Pass
		{
			Tags
				{
					"LightMode" = "ForwardBase" // allows shadows to be received & cast
				}
			// Writes to stencil buffer for outline pass 
			Stencil
			{
				Ref 4
				Comp always
				Pass replace
				ZFail keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			//Properties
			sampler2D _MainTex;
			sampler2D _RampTex;
			float4 _Color;
			float4 _LightColor0; // from Unity

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
				LIGHTING_COORDS(1, 2) // shadows
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				// convert input to world space
				output.pos = UnityObjectToClipPos(input.vertex);
				float4 normal4 = float4(input.normal, 0.0);
				output.normal = normalize(mul(normal4, unity_WorldToObject).xyz);

				output.texCoord = input.texCoord;

				TRANSFER_VERTEX_TO_FRAGMENT(output); // shadows

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				// lighting mode

				// convert light dir to world space & normalize
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				// find loc to sample on ramp texture
				// based on angle b/w surface normal and light dir
				float ramp = clamp(dot(input.normal, lightDir), 0, 1.0);
				float3 lighting = tex2D(_RampTex, float2(ramp, 0.5)).rgb;

				// sample ramp tex based on previous calc
				float4 albedo = tex2D(_MainTex, input.texCoord.xy);

				float attenuation = LIGHT_ATTENUATION(input); // shadow val
				//float3 rgb = albedo.rgb * _LightColor0.rgb * lighting * _Color.rgb * attenuation;
				float3 rgb = albedo.rgb * attenuation * lighting * (_LightColor0.rgb * 2);

				return float4(rgb, 1.0);
			}

			ENDCG
		}

		// Shadow pass
		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f 
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_TARGET
			{
				SHADOW_CASTER_FRAGMENT(i);
			}
				ENDCG
		}

	
			// Outline pass
		Pass
		{
				// won't draw where STENCIL buffer has value 4 @ ref
				Cull OFF
				ZWrite OFF
				ZTest ON
				Stencil
				{
					Ref 4
					Comp notequal
					Fail keep
					Pass replace
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				// properties
				uniform float4 _OutlineColor;
				uniform float _OutlineSize;
				uniform float _OutlineExtrusion;
				uniform float _OutlineDot;

				struct vertexInput
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float4 color : COLOR;
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;
					float4 newPos = input.vertex;
					
					// extruding along normals
					float3 normal = normalize(input.normal);
					newPos += float4(normal, 0.0) * _OutlineExtrusion;

					// convert to world space
					output.pos = UnityObjectToClipPos(newPos);

					output.color = _OutlineColor;
					return output;
				}

				float4 frag(vertexOutput input) : COLOR
				{
					//input.pos.xy = floor(input.pos.xy * _OutlineDot) * 0.5;
					//float checker = -frac(input.pos.r + input.pos.g);

					//clip(checker);
					return input.color;
				}

				ENDCG
		}
	}
}
