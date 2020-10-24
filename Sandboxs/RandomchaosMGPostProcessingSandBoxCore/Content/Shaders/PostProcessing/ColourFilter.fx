// Shader donated by https://twitter.com/RisingSunGames

// ===========================================
//  COMMON
// ===========================================

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0     //_level_9_1
#define PS_SHADERMODEL ps_4_0     //_level_9_1
#endif

struct VsInputQuad
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexureCoordinateA : TEXCOORD0;
};
struct VsOutputQuad
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
    float2 TexureCoordinateA : TEXCOORD0;
};
struct PsOutputQuad
{
    float4 Color : COLOR0;
};

// ===========================================
//  TECHNIQUES
// ===========================================


Texture2D InputTexture;
sampler InputSampler
{
    Texture = <InputTexture>;
};

float burn = 0.01f;
float saturation = 1.0f;
float3 color;
float bright = 0.0f;

PsOutputQuad FilterT(VsOutputQuad input)
{
    float2 tex = input.TexureCoordinateA;
	float4 col = tex2D(InputSampler, tex);
	
	float d = sqrt(pow((tex.x), 2) + pow((tex.y), 2));

	col.rgb -= d * burn;
	

	float a = col.r + col.g + col.b;
	a /= 3.0f;
	a *= 1.0f - saturation;

	col.r = (col.r * saturation + a) * color.r;
	col.g = (col.g * saturation + a) * color.g;
	col.b = (col.b * saturation + a) * color.b;
	col.rgb += bright;

    PsOutputQuad output;
	output.Color = col;
	return output;
}

technique Filter
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL FilterT();
    }
}