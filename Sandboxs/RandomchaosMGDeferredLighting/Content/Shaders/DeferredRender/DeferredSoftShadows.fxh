#define SAMPLE_COUNT 12
uniform extern float2 Taps[SAMPLE_COUNT];

bool CastShadow;
float DiscRadius = 1;
bool hardShadows = true;

float shadowSample(sampler shadowSampler, float2 lp)
{
	return  1 - tex2D(shadowSampler, lp).r;
}

float rnd(float4 seed)
{
	float dot_product = dot(seed, float4(12.9898,78.233,45.164,94.673));
    return frac(sin(dot_product) * 43758.5453);
}

float SoftShadow(float ss, float depth, float2 lightSamplePos, float shading, float realDistanceToLight, sampler shadowSampler,float4 seed)
{
	float add = 1.0/SAMPLE_COUNT * 1.5;
	float2 texel = float2(1.0 / 1920.0, 1.0 / 1080.0);
 
	for (int b = 0; b < SAMPLE_COUNT && ss <= realDistanceToLight; b++)
	{
		int idx = b;//int(lerp(0,SAMPLE_COUNT,rnd(seed)));

		float2 sp = lightSamplePos - texel * (Taps[idx]) * DiscRadius;
		
		ss = shadowSample(shadowSampler, sp);
		
		if (ss < realDistanceToLight)
			shading -= add;// + (b/SAMPLE_COUNT);			
	}

	return shading * depth;
}