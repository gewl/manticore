// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BlurShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Amount ("Blur Amount", Range(0,7)) = 1
		_Intensity ("Blur Intensity", Range(0,7)) = 1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	float _Amount;
	float _Intensity;
	float _Threshold;
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	struct v2f_off {
		float4 pos : POSITION;
		float2 uv[8] : TEXCOORD0;
	};
	
	v2f vert (appdata_img v)
	{
		v2f o;
		
		o.pos = UnityObjectToClipPos (v.vertex);

		o.uv = v.texcoord.xy;
		
		return o;
	}
	
	v2f_off vertOff (appdata_img v)
	{
		v2f_off o;
		
		o.pos = UnityObjectToClipPos (v.vertex);

		float2 uv = v.texcoord.xy;
		
		float2 up = float2(0.0, _MainTex_TexelSize.y) * _Amount;
		float2 right = float2(_MainTex_TexelSize.x, 0.0) * _Amount;	
			
		o.uv[0].xy = uv + up;
		o.uv[1].xy = uv - up;
		o.uv[2].xy = uv + right*2;
		o.uv[3].xy = uv - right*2;
		o.uv[4].xy = uv + up*2;
		o.uv[5].xy = uv - up*2;
		o.uv[6].xy = uv + up*1.45;
		o.uv[7].xy = uv - up*1.45;
	//	o.uv[5].xy = uv - right -up;
	//	o.uv[6].xy = uv + right + up;
	//	o.uv[7].xy = uv + right -up;
		
		return o;
	}
	
	half4 frag (v2f i) : COLOR
	{
		fixed4 col = tex2D(_MainTex, i.uv);
			//col.a = col.r + col.b + col.g;
			//	col.a  = 0;
				//col.a -=1;
				return col;
	}
	
	half4 fragOff (v2f_off i) : COLOR
	{
		
		float4 col = tex2D(_MainTex, (i.uv[0] + i.uv[1]) * 0.5);
		float4 newCol;
		newCol.rgb = col.rgb;
		int count = 1;
		for (int pix = 0; pix < 8; pix ++) {
			if (i.uv[pix].x <= 1.0 && i.uv[pix].y <= 1.0 && i.uv[pix].x >= 0.0 && i.uv[pix].y >= 0.0) {
			float4 tempCol = tex2D(_MainTex, i.uv[pix]);
			if (tempCol.r > .2 || tempCol.g > .3 || tempCol.b > .3) {
				newCol += tempCol;
				count ++;
				}
			}
		}
		newCol = ((newCol + col) / (count + 1));
	//	newCol.a =  col.r*col.r + col.b + col.g/2;
		//newCol.a *= newCol.a;
		///newCol.a -= 5;
	//	newCol.r = saturate(col.r*2-.1);
	//	newCol.g = saturate(col.g*2-.1);
	//	newCol.b = saturate(col.b*2-.1);
		//newCol.g = saturate(newCol.g*newCol.g-.25)*25;
	//	newCol.r = saturate(newCol.r*newCol.r-.25)*25;
	//	newCol.b = saturate(newCol.b*newCol.b-.25)*25;
	//newCol.b /= 2;
//	newCol.rgb = saturate((newCol.rgb-.23)*(newCol.rgb-.23)*(newCol.rgb-.24))*250;
	//newCol.b *= 2;
		newCol.a = saturate(newCol.rgb-.1);
		newCol.rgb = saturate(newCol.rgb -.1)*_Intensity;//*5;
	//	newCol.r *= newCol.r;
	//	newCol.b = 0;
	//	newCol.b *= 25;
		return newCol;
	}
	
	ENDCG
	
	SubShader {
	
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
//	Blend OneMinusDstColor One
	Blend SrcAlpha One
//	Blend One OneMinusSrcColor 
		pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
	//Blend SrcColor DstColor
	//Blend SrcAlpha OneMinusSrcAlpha 
		pass {
			CGPROGRAM
			#pragma vertex vertOff
			#pragma fragment fragOff
			#pragma target 3.0
			ENDCG
		}
	}
}
