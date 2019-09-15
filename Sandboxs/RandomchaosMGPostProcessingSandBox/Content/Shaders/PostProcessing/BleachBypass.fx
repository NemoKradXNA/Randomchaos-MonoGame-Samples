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

float Opacity;

PsOutputQuad BleachBypassPs(VsOutputQuad input)
{
    float4 base = tex2D(InputSampler, input.TexureCoordinateA);
    float3 lumCoeff = float3(0.25,0.65,0.1);
    float lum = dot(lumCoeff,base.rgb);
    float3 blend = lum.rrr;
    float L = min(1,max(0,10*(lum- 0.45)));
    float3 result1 = 2.0f * base.rgb * blend;
    float3 result2 = 1.0f - 2.0f*(1.0f-blend)*(1.0f-base.rgb);
    float3 newColor = lerp(result1,result2,L);
    float A2 = Opacity * base.a;
    float3 mixRGB = A2 * newColor.rgb;
    mixRGB += ((1.0f-A2) * base.rgb);

    PsOutputQuad output;
    output.Color = float4(mixRGB, base.a);
    return output;
}

technique BleachBypass
{
	pass Pass1
	{
        PixelShader = compile PS_SHADERMODEL BleachBypassPs();
    }
}
