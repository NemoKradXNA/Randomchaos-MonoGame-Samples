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
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

//Global Variables 
float Timer;

//noise effect intensity value (0 = no effect, 1 = full effect) 
float fNintensity = 0.15;
//scanlines effect intensity value (0 = no effect, 1 = full effect) 
float fSintensity = 1.0;
//scanlines effect count value (0 = no effect, 4096 = full effect) 
float fScount = 1700;
//#define RGB 
//#define MONOCHROME 

// Pixel Shader Output 
PsOutputQuad ps_main(VsOutputQuad input)
{
	// sample the source 
    float4 cTextureScreen = tex2D(InputSampler, input.TexureCoordinateA.xy);

	// make some noise 
    float x = input.TexureCoordinateA.x * input.TexureCoordinateA.y * Timer  *1000 ;
	x = fmod(x, 13) * fmod(x, 123);
	float dx = fmod(x, 0.01);

	// add noise 
	float3 cResult = cTextureScreen.rgb + cTextureScreen.rgb * saturate(0.1f + dx.xxx * 100);

		// get us a sine and cosine 
    float2 sc;
    sincos(input.TexureCoordinateA.y * fScount, sc.x, sc.y);

	// add scanlines 
	cResult += cTextureScreen.rgb * float3(sc.x, sc.y, sc.x) * fSintensity;

	// interpolate between source and result by intensity 
	cResult = lerp(cTextureScreen.xyz, cResult, saturate(fNintensity));

	// convert to grayscale if desired 
#ifdef MONOCHROME 
	cResult.rgb = dot(cResult.rgb, float3(0.3, 0.59, 0.11));
#endif 

	// return with source alpha 
    PsOutputQuad output;
    output.Color = float4(cResult, cTextureScreen.a);
    return output;
}

//Technique Calls 
technique PostProcess
{
	pass Pass_0
	{
        PixelShader = compile PS_SHADERMODEL ps_main();
    }
}