#include "PPVertexShader.fxh"

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


float4 BlendPS(VertexShaderOutput input) : COLOR0
{
	float4 col = tex2D(Scene, input.TexCoord) * tex2D(orgScene, input.TexCoord);

	return col;
}

float4 AditivePS(VertexShaderOutput input) : COLOR0
{
	float4 col = tex2D(Scene, input.TexCoord) + tex2D(orgScene, input.TexCoord);

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