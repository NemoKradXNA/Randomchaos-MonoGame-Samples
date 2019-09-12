// Pixel shader that applies a Poisson disc blur filter.
// Samples pixels within a circle. The good thing about this 
// blur filter is that it is dynamic. You can grow/shrink the
// circle however you like to achieve the desired effect.

#define SAMPLE_COUNT 12

uniform extern texture SceneTex;
uniform extern float DiscRadius;
uniform extern float2 TexelSize;
uniform extern float2 Taps[SAMPLE_COUNT];

// Tap locations for unit disc
//Hmm, seems to peform better when we set the Taps from outside.....odd.
/*float2 Taps[12] = { float2(-0.326212,-0.40581),float2(-0.840144,-0.07358),
					float2(-0.695914,0.457137),float2(-0.203345,0.620716),
					float2(0.96234,-0.194983),float2(0.473434,-0.480026),
					float2(0.519456,0.767022),float2(0.185461,-0.893124),
					float2(0.507431,0.064425),float2(0.89642,0.412458),
					float2(-0.32194,-0.932615),float2(-0.791559,-0.59771)};*/

sampler2D TextureSampler = sampler_state
{
	Texture = <SceneTex>;
	MinFilter = Anisotropic;
    MagFilter = Anisotropic;
    MaxAnisotropy = 8;
    MipFilter = Linear;
    MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
    
    AddressU  = CLAMP;
    AddressV  = CLAMP;
};

float4 PoissonDiscBlurPS(float4 Position : SV_POSITION,
	float4 Color : COLOR0,
	float2 texCoord : TEXCOORD0) : COLOR0
{
	// Take a sample at the disc’s center
	float4 base = tex2D( TextureSampler, texCoord );
	float4 sampleAccum = base;
	
	// Take 12 samples in disc
	for ( int nTapIndex = 0; nTapIndex < SAMPLE_COUNT; nTapIndex++ )
	{
		// Compute new texture coord inside disc
		float2 vTapCoord = texCoord - TexelSize * Taps[nTapIndex] * DiscRadius;
		
		// Accumulate samples
		sampleAccum += tex2D( TextureSampler, saturate(vTapCoord) );
	}
	
	return sampleAccum * 0.0769f; // Return average, divide by 13
}


technique GaussianBlur
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 PoissonDiscBlurPS();
    }
}