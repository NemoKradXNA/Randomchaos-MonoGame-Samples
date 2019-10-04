#include "DeferredHeader.fxh"

// Global Variables
float4x4 wvp : WorldViewProjection;


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, wvp);
	output.Color = input.Color;

	return output;
}

PixelShaderOutput PSBasicTexture(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	output.Color = input.Color;

	output.Tangent.rgb = 0;
	output.Tangent.a = 1;

	output.SGR.r = 0;
	output.SGR.g = 1;
	output.SGR.b = 0;
	output.SGR.w = 0;

	output.Depth.r = 0;
	output.Depth.a = 1;

	return output;
}

technique Deferred
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PSBasicTexture();
	}
}
