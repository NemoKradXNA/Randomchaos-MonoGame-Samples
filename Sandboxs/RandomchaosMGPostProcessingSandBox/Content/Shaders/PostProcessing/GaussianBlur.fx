#include "PPVertexShader.fxh"
// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

#define SAMPLE_COUNT 11

uniform extern float4 SampleOffsets[SAMPLE_COUNT];
uniform extern float SampleWeights[SAMPLE_COUNT];
//uniform extern texture SceneTex;

float2 halfPixel;

sampler TextureSampler : register(s0);
/* = sampler_state
{
	Texture = <SceneTex>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};*/


float4 GaussianBlurPS(VertexShaderOutput input) : COLOR0
{
	input.TexCoord-=halfPixel;
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(TextureSampler, saturate(input.TexCoord + SampleOffsets[i].xy)) * SampleWeights[i];
    }
    
    return c;
}


technique GaussianBlur
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL GaussianBlurPS();
        
    }
}