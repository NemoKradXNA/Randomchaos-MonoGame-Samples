#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Pixel shader combines the bloom image with the original
// scene, using tweakable intensity levels and saturation.
// This is the final step in applying a bloom postprocess.

uniform extern float BloomIntensity;
uniform extern float BaseIntensity;

uniform extern float BloomSaturation;
uniform extern float BaseSaturation;

uniform extern texture SceneTex;
//uniform extern texture BloomTex;

uniform extern float powFactor = 1;

float2 halfPixel;

sampler BaseSampler = sampler_state
{
	Texture = <SceneTex>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};

sampler BloomSampler :register(s0);
/*= sampler_state
{
	Texture = <BloomTex>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};*/


// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}		

float4 BloomPS(float4 Position : SV_POSITION,
float4 Color : COLOR0,
float2 texCoord : TEXCOORD0) : COLOR0
{
	texCoord -= halfPixel;
    // Look up the bloom and original base image colors.
    float4 bloom = tex2D(BloomSampler, texCoord);
    float4 base = tex2D(BaseSampler, texCoord);
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1 - saturate(bloom));
    
    // Combine the two images.
    return pow(base + bloom,powFactor);
    
}

float4 GlarePixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
	texCoord -= halfPixel;
    // Look up the bloom and original base image colors.
    float4 bloom = tex2D(BloomSampler, texCoord) * BloomIntensity;
    float4 base = tex2D(BaseSampler, texCoord) * BaseIntensity;
    
     // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    base *= (1 - saturate(bloom));
    
	//Produces more of a glare effect rather than just a bloom.
    float3 final = saturate(base.rgb + bloom.rgb - base.rgb * bloom.rgb);
    return float4(final, 1.0);
}

technique BloomComposite
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BloomPS();
        
        //ZWriteEnable = false;
    }
}

technique GlareComposite
{
	pass P0
    {
        PixelShader = compile PS_SHADERMODEL GlarePixelShader();
        
        //ZWriteEnable = false;
    }
}
