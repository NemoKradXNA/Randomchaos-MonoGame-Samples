#define SAMPLE_COUNT 12
uniform extern float2 Taps[SAMPLE_COUNT];

bool CastShadow;
float DiscRadius = 1;
bool hardShadows = true;

float shadowSample(sampler shadowSampler, float2 lp)
{
	return  1 - tex2D(shadowSampler, lp).r;// (r / 3);
}

float SoftShadow(float ss, float depth, float2 lightSamplePos, float shading, float realDistanceToLight, sampler shadowSampler)
{
	float2 texelSize = texelSize = float2(1.0 / 1920.0, 1.0 / 1080.0);
	
	float add = (1.0 / 26.0) *  saturate((depth) * 30);
	shading -= add;

	float2 lsp = lightSamplePos;
	float ld = SAMPLE_COUNT * DiscRadius * depth;

	for (int b = 0; b < SAMPLE_COUNT && ss <= realDistanceToLight; b++)
	{
		float2 sp = lightSamplePos + texelSize * Taps[b] * DiscRadius;
		
		add = dot(lsp, sp) / ld;

		ss = shadowSample(shadowSampler, sp);
		if (ss <= realDistanceToLight)
			shading -= add;
	}

	return shading * depth;
}