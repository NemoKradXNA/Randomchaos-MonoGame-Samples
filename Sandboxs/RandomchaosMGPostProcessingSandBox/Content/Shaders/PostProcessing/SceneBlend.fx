#include "PPVertexShader.fxh"

float2 halfPixel;

sampler2D Scene: register(s0){
	AddressU = Mirror;
	AddressV = Mirror;
};

texture OrgScene;
sampler2D orgScene = sampler_state
{
	Texture = <OrgScene>;
	AddressU = CLAMP;
	AddressV = CLAMP;
};


float4 BlendPS(float4 Position : SV_POSITION,
	float4 Color : COLOR0,
	float2 texCoord : TEXCOORD0) : COLOR0
{
	texCoord -= halfPixel;
	float4 col = tex2D(Scene, texCoord) * tex2D(orgScene,texCoord);

	return col;
}

float4 AditivePS(float4 Position : SV_POSITION,
	float4 Color : COLOR0,
	float2 texCoord : TEXCOORD0) : COLOR0
{
	texCoord -= halfPixel;
	float4 col = tex2D(Scene, texCoord) + tex2D(orgScene,texCoord);

	return col;
}

technique Blend
{
	pass p0
	{
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL BlendPS();
	}
}

technique Aditive
{
	pass p0
	{
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL AditivePS();
	}
}