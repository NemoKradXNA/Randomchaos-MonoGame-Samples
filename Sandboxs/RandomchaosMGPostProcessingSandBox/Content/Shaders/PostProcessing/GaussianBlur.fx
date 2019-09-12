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


float4 GaussianBlurPS(float4 Position : SV_POSITION,
	float4 Color : COLOR0,
	float2 texCoord : TEXCOORD0) : COLOR0
{
	texCoord-=halfPixel;
    float4 c = 0;
    
    // Combine a number of weighted image filter taps.
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(TextureSampler, saturate(texCoord + SampleOffsets[i].xy)) * SampleWeights[i];
    }
    
    return c;
}


technique GaussianBlur
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 GaussianBlurPS();
        
    }
}