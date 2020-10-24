#include "PPVertexShader.fxh"

sampler screen : register(s0);

texture depthMap;
sampler DepthMap = sampler_state
{
	Texture = <depthMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


float camMin;
float camMax;
float fogRange;
float fogDistance;
float4 fogColor;
float fogMinThickness = .999f;
float fogMaxThickness = 0.0f;


float4 FogPS(VertexShaderOutput Input) : COLOR0
{
	float4 s = tex2D(screen, Input.TexCoord);
	float depth = 1 - tex2D(DepthMap,Input.TexCoord).r;

	float fSceneZ = ( -camMin * ((camMax /(camMax - camMin))) ) / ( depth - ((camMax /(camMax - camMin))));
	float fFogFactor = clamp(saturate( ( fSceneZ - fogDistance ) / fogRange ),fogMaxThickness,fogMinThickness);

	return lerp(s,fogColor,fFogFactor);

	
}

technique Fog 
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL FogPS();
	}
}