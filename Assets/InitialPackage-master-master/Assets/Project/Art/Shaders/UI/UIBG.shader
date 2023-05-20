Shader "UI/UIBG"
{
   Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TopColor ("TopColor", Color) = (1,1,1,1)
		_BottomColor ("BottomColor", Color) = (1,1,1,1)
		[Header (Pattern)]
		_Size ("Size", Range(0.0, 10.0)) = 3
		_Offset ("Offset", Range(0.0, 1.0)) = 0.15
		_ScrollSpeed ("Scroll Speed", Range(0.0, 5.0)) = 0.5
		[IntRange]_RotateSpeed ("Rotate Speed", Range(0.0, 50)) = 5
		[Header (Circle 1)]
		_CircleColor1 ("Circle Color 1", Color) = (.5,.5,.5,1)
		_CircleRad1 ("Circle Rad 1", float) = 1
        _CirclePos1 ("Circle Pos 1", Range (0.0, 1)) = 0.5
		[Header (Circle 2)]
		_CircleColor2 ("Circle Color 2", Color) = (.5,.5,.5,1)
		_CircleRad2 ("Circle Rad 2", float) = 1
		_CirclePos2 ("Circle Pos 2", Range (0.0, 1)) = 0.5
		[PowerSlider(3.0)] _Shininess ("Shininess", Range (0.01, 1)) = 0.08
	}
	SubShader
	{
		Tags
		{ 
			"Queue"="Geometry" 
		}

		Cull Off
		Lighting Off
		ZWrite Off

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
				float2 texcoord1  : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _BottomColor;
			fixed4 _TopColor;
			//Pattern
			fixed _Size;
            fixed _Offset;
			fixed _ScrollSpeed;
			fixed _RotateSpeed;

			fixed4 _CircleColor1;
			fixed _CircleRad1;
			fixed4 _CircleColor2;
			fixed _CirclePos1;
			fixed _CircleRad2;
			fixed _CirclePos2;

			float _Shininess;
		
			float2x2 Rotation(fixed speed){
				float s = sin ( speed * _Time );
				float c = cos ( speed * _Time );
				return float2x2( c, -s, s, c);
            }

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				float s = 0.707; // ±sin 45 deg
				o.texcoord1 = mul(v.texcoord, float2x2(s, -s, s, s));
                o.texcoord1.x -= _Time.x * _ScrollSpeed;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 Lighten (fixed4 a, fixed4 b)
			{ 
			    return max(a, b);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = lerp (_BottomColor,_TopColor, i.texcoord.y);

				float distFromCenter = distance(i.texcoord, float2(0.5, _CirclePos1));
                float circle1 = smoothstep(_CircleRad1, _CircleRad1 - _CircleColor1.a, distFromCenter);
				float circle2 = smoothstep(_CircleRad2, _CircleRad2 - _CircleColor2.a, distFromCenter);
                color =  lerp(color,_CircleColor1, circle1);
				color =  lerp(color,_CircleColor2, circle2);

				half2 uvPattern = frac(i.texcoord1 / _Offset) * _Size;				
				half2 uvRotated = mul(uvPattern - 0.7, Rotation(-_RotateSpeed)) + 0.5;

				fixed4 tile = tex2D(_MainTex, uvRotated);
				tile.a *= 1;
				color = lerp (color,tile,  tile.a);

				return color ;
			}
		ENDCG
		}
	}
}
