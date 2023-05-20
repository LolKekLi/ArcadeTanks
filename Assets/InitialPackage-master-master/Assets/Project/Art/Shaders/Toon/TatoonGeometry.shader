// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TetraArts/TatoonGeometry"
{
	Properties
	{
		_RimColor("Rim Color", Color) = (0,1,0.8758622,0)
		_DiffuseColor("Diffuse Color", Color) = (1,1,1,0)
		_RimBlend("Rim Blend", Range( 0 , 10)) = 0
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		[Toggle]_UseNormalMap("UseNormalMap", Float) = 0
		[NoScaleOffset]_TextureDiffuse("Texture Diffuse", 2D) = "white" {}
		_NormalStrength("Normal Strength", Float) = 1
		_RimSize("Rim Size", Range( 0 , 2)) = 0.5679239
		_ShadowColor("Shadow Color", Color) = (1,0,0,1)
		_ShadowSize("Shadow Size", Range( 0 , 2)) = 0
		[NoScaleOffset]_ShadowTexture("Shadow Texture", 2D) = "white" {}
		_ShadowBlend("ShadowBlend", Range( 0 , 1)) = 0
		[Toggle]_UseRim("UseRim", Float) = 0
		[Toggle]_ShadowTextureViewProjection("Shadow Texture View Projection", Float) = 0
		_ShadowTextureTiling("Shadow Texture Tiling", Float) = 0
		_ShadowTextureRotation("Shadow Texture Rotation", Float) = 0
		[Toggle]_UseShadow("UseShadow", Float) = 0
		[Toggle]_UseGradient("Use Gradient", Float) = 0
		[NoScaleOffset]_RimTexture("Rim Texture", 2D) = "white" {}
		[Toggle]_RimTextureViewProjection("Rim Texture View Projection", Float) = 0
		_ColorB("Color B", Color) = (0,0.1264467,1,0)
		_ColorA("Color A", Color) = (1,0,0,0)
		_RimTextureTiling("Rim Texture Tiling", Float) = 0
		_GradientSize("Gradient Size", Range( 0 , 10)) = 0
		_RimTextureRotation("Rim Texture Rotation", Float) = 0
		_GradientPosition("Gradient Position", Float) = 0
		_GradientRotation("Gradient Rotation", Float) = 0
		[Toggle]_UseSpecular("UseSpecular", Float) = 0
		[HideInInspector][NoScaleOffset][Normal]_Texture0("Texture 0", 2D) = "bump" {}
		[NoScaleOffset]_SpecularMap("Specular Map", 2D) = "gray" {}
		[Toggle]_SpecularTextureViewProjection("Specular Texture View Projection", Float) = 0
		_SpecularTextureTiling("Specular Texture Tiling", Float) = 0
		_SpecularTextureRotation("Specular Texture Rotation", Float) = 0
		_SpecularColor("Specular Color", Color) = (1,0.9575656,0.75,0)
		[Toggle]_SpecLightColor("Spec Light Color", Float) = 0
		_SpecularLightIntensity("Specular Light Intensity", Range( 0 , 10)) = 1
		_SpecularSize("Specular Size", Range( 0 , 1)) = 0.005
		_SpecularBlend("Specular Blend", Range( 0 , 1)) = 0
		[HDR]_OutlineColor("Outline Color", Color) = (0,0,0,0)
		[Toggle]_RimLightColor("Rim Light Color", Float) = 0
		_RimLightIntensity("Rim Light Intensity", Range( 0 , 10)) = 0
		_OutlineSize("Outline Size", Float) = 0.1
		[Toggle]_UseOutline("UseOutline", Float) = 0
		[HDR]_OutlineColor2("OutlineColor2", Color) = (1,0.06464233,0,0)
		[HDR]_OutlineColor1("OutlineColor1", Color) = (0,0.9740796,1,0)
		_Speed("Speed", Range( -1 , 1)) = 0
		_PowerColor1("PowerColor1", Float) = 1
		_PowerColor2("PowerColor2", Float) = 1
		[NoScaleOffset]_Texture("Texture", 2D) = "white" {}
		_NoiseTextureScale("NoiseTextureScale", Float) = 2
		[Toggle]_UseOutlineFire("UseOutlineFire", Float) = 0
		_AttenuationPower("AttenuationPower", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		ZWrite On
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Lighting.cginc"
		#pragma target 4.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _UseShadow;
		uniform float4 _ShadowColor;
		uniform sampler2D _ShadowTexture;
		uniform float _ShadowTextureViewProjection;
		uniform float _ShadowTextureTiling;
		uniform float _ShadowTextureRotation;
		uniform float _UseGradient;
		uniform float4 _DiffuseColor;
		uniform float4 _ColorA;
		uniform float4 _ColorB;
		uniform float _GradientPosition;
		uniform float _GradientSize;
		uniform float _GradientRotation;
		uniform sampler2D _TextureDiffuse;
		uniform float _ShadowSize;
		uniform float _ShadowBlend;
		uniform float _AttenuationPower;
		uniform float _UseRim;
		uniform float _RimSize;
		uniform float _RimBlend;
		uniform float _RimLightColor;
		uniform float4 _RimColor;
		uniform float _RimLightIntensity;
		uniform sampler2D _RimTexture;
		uniform float _RimTextureViewProjection;
		uniform float _RimTextureTiling;
		uniform float _RimTextureRotation;
		uniform float _UseSpecular;
		uniform sampler2D _SpecularMap;
		uniform float _SpecularTextureViewProjection;
		uniform float _SpecularTextureTiling;
		uniform float _SpecularTextureRotation;
		uniform float _SpecLightColor;
		uniform float4 _SpecularColor;
		uniform float _SpecularLightIntensity;
		uniform float _SpecularSize;
		uniform float _SpecularBlend;
		uniform float _UseNormalMap;
		uniform sampler2D _Texture0;
		uniform float _NormalStrength;
		uniform sampler2D _Normal;
		uniform float _UseOutline;
		uniform float _OutlineSize;
		uniform float _UseOutlineFire;
		uniform float4 _OutlineColor;
		uniform float4 _OutlineColor1;
		uniform float _PowerColor1;
		uniform sampler2D _Texture;
		uniform float _Speed;
		uniform float _NoiseTextureScale;
		uniform float _PowerColor2;
		uniform float4 _OutlineColor2;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 Outline219 = 0;
			v.vertex.xyz += Outline219;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float4 color74 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
			float2 temp_cast_0 = (_ShadowTextureTiling).xx;
			float2 uv_TexCoord65 = i.uv_texcoord * temp_cast_0;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float cos70 = cos( radians( _ShadowTextureRotation ) );
			float sin70 = sin( radians( _ShadowTextureRotation ) );
			float2 rotator70 = mul( (( _ShadowTextureViewProjection )?( ( ( _ShadowTextureTiling * 1 ) * mul( UNITY_MATRIX_VP, float4( ase_worldViewDir , 0.0 ) ).xyz ) ):( float3( uv_TexCoord65 ,  0.0 ) )).xy - float2( 0,0 ) , float2x2( cos70 , -sin70 , sin70 , cos70 )) + float2( 0,0 );
			float4 ShadowColor80 = (( _UseShadow )?( ( _ShadowColor * tex2D( _ShadowTexture, rotator70 ) ) ):( color74 ));
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			UnityGI gi11 = gi;
			float3 diffNorm11 = ase_worldNormal;
			gi11 = UnityGI_Base( data, 1, diffNorm11 );
			float3 indirectDiffuse11 = gi11.indirect.diffuse + diffNorm11 * 0.0001;
			float4 Ambient181 = ( ase_lightColor * float4( indirectDiffuse11 , 0.0 ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 appendResult93 = (float4(ase_vertex3Pos.x , ase_vertex3Pos.y , 0.0 , 0.0));
			float cos95 = cos( radians( _GradientRotation ) );
			float sin95 = sin( radians( _GradientRotation ) );
			float2 rotator95 = mul( appendResult93.xy - float2( 0,0 ) , float2x2( cos95 , -sin95 , sin95 , cos95 )) + float2( 0,0 );
			float smoothstepResult101 = smoothstep( _GradientPosition , ( _GradientPosition + _GradientSize ) , rotator95.x);
			float4 lerpResult102 = lerp( _ColorA , _ColorB , smoothstepResult101);
			float2 uv_TextureDiffuse108 = i.uv_texcoord;
			float4 tex2DNode108 = tex2D( _TextureDiffuse, uv_TextureDiffuse108 );
			float4 Color110 = ( ( Ambient181 + (( _UseGradient )?( lerpResult102 ):( ( ase_lightColor * _DiffuseColor ) )) ) * tex2DNode108 );
			float Atten328 = pow( ase_lightAtten , _AttenuationPower );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult3 = dot( ase_normWorldNormal , ase_worldlightDir );
			float NdotL175 = dotResult3;
			float smoothstepResult58 = smoothstep( _ShadowSize , ( _ShadowSize + _ShadowBlend ) , ( Atten328 + NdotL175 ));
			float temp_output_410_0 = ( smoothstepResult58 * Atten328 );
			float dotResult38 = dot( ase_worldNormal , ase_worldViewDir );
			float2 temp_cast_8 = (_RimTextureTiling).xx;
			float2 uv_TexCoord193 = i.uv_texcoord * temp_cast_8;
			float cos198 = cos( radians( _RimTextureRotation ) );
			float sin198 = sin( radians( _RimTextureRotation ) );
			float2 rotator198 = mul( (( _RimTextureViewProjection )?( ( ( _RimTextureTiling * 1 ) * mul( float4( ase_worldViewDir , 0.0 ), UNITY_MATRIX_VP ).xyz ) ):( float3( uv_TexCoord193 ,  0.0 ) )).xy - float2( 0,0 ) , float2x2( cos198 , -sin198 , sin198 , cos198 )) + float2( 0,0 );
			float4 Rim185 = (( _UseRim )?( ( saturate( ( NdotL175 * pow( ( 1.0 - saturate( ( dotResult38 + _RimSize ) ) ) , _RimBlend ) ) ) * ( (( _RimLightColor )?( float4( ( _RimLightIntensity * ase_lightColor.rgb ) , 0.0 ) ):( _RimColor )) * tex2D( _RimTexture, rotator198 ) ) ) ):( float4( 0,0,0,0 ) ));
			float2 temp_cast_13 = (_SpecularTextureTiling).xx;
			float2 uv_TexCoord143 = i.uv_texcoord * temp_cast_13;
			float cos156 = cos( radians( _SpecularTextureRotation ) );
			float sin156 = sin( radians( _SpecularTextureRotation ) );
			float2 rotator156 = mul( (( _SpecularTextureViewProjection )?( ( ( _SpecularTextureTiling * 1 ) * mul( float4( ase_worldViewDir , 0.0 ), UNITY_MATRIX_VP ).xyz ) ):( float3( uv_TexCoord143 ,  0.0 ) )).xy - float2( 0,0 ) , float2x2( cos156 , -sin156 , sin156 , cos156 )) + float2( 0,0 );
			float temp_output_161_0 = ( 1.0 - _SpecularSize );
			float3 normalizeResult148 = normalize( ase_worldlightDir );
			float3 normalizeResult147 = normalize( ase_worldViewDir );
			float3 normalizeResult162 = normalize( ( normalizeResult148 + normalizeResult147 ) );
			float3 normalizeResult157 = normalize( ase_worldNormal );
			float dotResult165 = dot( normalizeResult162 , normalizeResult157 );
			float smoothstepResult169 = smoothstep( temp_output_161_0 , ( temp_output_161_0 + _SpecularBlend ) , dotResult165);
			float4 Specular174 = (( _UseSpecular )?( ( ( ( ( 1.0 - tex2D( _SpecularMap, rotator156 ) ) * (( _SpecLightColor )?( ( ase_lightColor * _SpecularLightIntensity ) ):( _SpecularColor )) ) * smoothstepResult169 ) * NdotL175 ) ):( float4( 0,0,0,0 ) ));
			float4 Result135 = ( ( ( ShadowColor80 * Color110 ) * ( 1.0 - smoothstepResult58 ) ) + ( Color110 * smoothstepResult58 ) + ( temp_output_410_0 * Rim185 ) + ( temp_output_410_0 * Specular174 ) );
			float2 uv_Texture0119 = i.uv_texcoord;
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float3 normalizeResult123 = normalize( ase_worldlightDir );
			float dotResult128 = dot( BlendNormals( UnpackNormal( tex2D( _Texture0, uv_Texture0119 ) ) , ase_objectScale ) , normalizeResult123 );
			float2 uv_Normal120 = i.uv_texcoord;
			float dotResult127 = dot( BlendNormals( UnpackScaleNormal( tex2D( _Normal, uv_Normal120 ), _NormalStrength ) , ase_objectScale ) , ase_worldlightDir );
			float4 NormalMap132 = (( _UseNormalMap )?( ( dotResult127 * Result135 ) ):( ( dotResult128 * Result135 ) ));
			c.rgb = ( Result135 + NormalMap132 ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "TatoonEditor"
}