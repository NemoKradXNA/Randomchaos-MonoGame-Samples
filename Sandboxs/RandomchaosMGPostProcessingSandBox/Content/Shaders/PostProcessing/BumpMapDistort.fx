#include "PPVertexShader.fxh"
// This distorts the image in the direction defined by the 
// bumpmap. Used to create a gravitation field effect.
// Includes two methods of distorting the image, 
// one "high" and one "low".

//uniform extern texture	SceneTex;
uniform extern texture  Bumpmap;

uniform extern float Offset;

float2 halfPixel;

sampler BackGroundSampler : register(s0);
/* = sampler_state
{
	Texture = <SceneTex>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = CLAMP;
    AddressV  = CLAMP;
};*/

sampler FractalSampler = sampler_state
{
	Texture = <Bumpmap>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = WRAP;
    AddressV  = WRAP;
};

//Costly distort - 8 texture reads
float4 HighPS(VertexShaderOutput input) : COLOR0
{
	float4 bc = tex2D(BackGroundSampler, input.TexCoord);

	input.TexCoord -= halfPixel;
	float2 bgTexC = input.TexCoord;
	
	input.TexCoord.y += Offset;
	float2 offset0 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	input.TexCoord.y -= Offset;
	
	input.TexCoord.y -= Offset;
	float2 offset1 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	input.TexCoord.y += Offset;
	
	input.TexCoord.x += Offset;
	float2 offset2 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	input.TexCoord.x -= Offset;
	
	input.TexCoord.x -= Offset;
	float2 offset3 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	
	float4 c0 = tex2D(BackGroundSampler, bgTexC + (offset0 - .025f));
	float4 c1 = tex2D(BackGroundSampler, bgTexC + (offset1 - .025f));
	float4 c2 = tex2D(BackGroundSampler, bgTexC + (offset2 - .025f));
	float4 c3 = tex2D(BackGroundSampler, bgTexC + (offset3 - .025f));
	
	return (c0 + c1 + c2 + c3) * .25f;
}

//Similar distort, not quite as blurry - 5 texture reads
float4 LowPS(VertexShaderOutput input) : COLOR0
{
	float4 bc = tex2D(BackGroundSampler, input.TexCoord);

	input.TexCoord -= halfPixel;
	float2 bgTexC = input.TexCoord;
	
	input.TexCoord.y += Offset;
	float2 offset0 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	input.TexCoord.y -= Offset;
	
	input.TexCoord.y -= Offset;
	float2 offset1 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	input.TexCoord.y == Offset;
	
	input.TexCoord.x += Offset;
	float2 offset2 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	input.TexCoord.x -= Offset;
	
	input.TexCoord.x -= Offset;
	float2 offset3 = tex2D(FractalSampler, input.TexCoord).xy * .05f;
	
	offset0 = offset0 + offset1 + offset2 + offset3;
	offset0 *= .25f;
	
	//if(input.TexCoord.y > .5)
		return tex2D(BackGroundSampler, bgTexC + (offset0 - .025f));
	//else
		//return tex2D(BackGroundSampler, input.TexCoord);
}

technique High
{
	pass P0
    {
        pixelShader  = compile PS_SHADERMODEL HighPS();
    }
}

technique Low
{
	pass P0
    {
        pixelShader  = compile PS_SHADERMODEL LowPS();
    }
}
