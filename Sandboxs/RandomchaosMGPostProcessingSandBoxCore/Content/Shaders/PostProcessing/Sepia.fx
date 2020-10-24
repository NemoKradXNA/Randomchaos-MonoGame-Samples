#include "PPVertexShader.fxh"


sampler screen : register(s0)
{
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 Sepia(VertexShaderOutput input) : COLOR0
{

	float4 col = tex2D(screen, input.TexCoord);

	float4 outputColor = col;
	outputColor.r = (col.r * 0.393) + (col.g * 0.769) + (col.b * 0.189);
	outputColor.g = (col.r * 0.349) + (col.g * 0.686) + (col.b * 0.168);
	outputColor.b = (col.r * 0.272) + (col.g * 0.534) + (col.b * 0.131);

	return outputColor;
}

technique PostInvert
{
	pass P0
	{		
		PixelShader = compile PS_SHADERMODEL Sepia();
	}
}