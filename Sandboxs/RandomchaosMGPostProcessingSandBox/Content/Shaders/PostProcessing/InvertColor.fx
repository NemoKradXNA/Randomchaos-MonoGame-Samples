#include "PPVertexShader.fxh"


sampler screen : register(s0)
{
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 Invert(VertexShaderOutput input) : COLOR0
{

	float4 col = (float4)1 - tex2D(screen, input.TexCoord);

	return col;
}

technique PostInvert
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Invert();
	}
}