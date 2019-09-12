Texture surfaceTexture;
samplerCUBE TextureSampler = sampler_state 
{ 
    texture = <surfaceTexture> ; 
    magfilter = LINEAR; 
    minfilter = LINEAR; 
    mipfilter = LINEAR; 
    AddressU = Mirror;
    AddressV = Mirror;
};

float4x4 World : World;
float4x4 View : View;
float4x4 Projection : Projection;

float3 EyePosition : CameraPosition;

float alpha = 1;

struct VS_INPUT 
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
    float3 Normal : NORMAL0;    
};

struct VS_OUTPUT 
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCORD0;
    float3 ViewDirection : TEXCOORD1;
        
};

struct PixelShaderOutput
{
	float4 Color	: COLOR0;	// Color	
	float4 Depth	: COLOR1;	// Depth map 
};

float4 CubeMapLookup(float3 CubeTexcoord)
{    
    return texCUBE(TextureSampler, CubeTexcoord);
}

VS_OUTPUT Transform(VS_INPUT Input)
{
    float4x4 WorldViewProjection = mul(mul(World, View), Projection);
    float3 ObjectPosition = mul(Input.Position, World);
    
    VS_OUTPUT Output;
    Output.Position = mul(Input.Position, WorldViewProjection);
    Output.ViewDirection = normalize(ObjectPosition - EyePosition);
    
    return Output;
}

struct PS_INPUT 
{    
    float3 ViewDirection : TEXCOORD2;
};

PixelShaderOutput BasicShader(PS_INPUT Input) : COLOR0
{    
	PixelShaderOutput output = (PixelShaderOutput)0;

    float3 ViewDirection = normalize(Input.ViewDirection);    
    ViewDirection.x *= -1;
	output.Color = float4(ViewDirection,1) + CubeMapLookup(ViewDirection) * alpha;
	output.Depth = 0;

	return output;
}

technique Sky 
{
    pass P0
    {
        VertexShader = compile vs_4_0_level_9_1 Transform();
        PixelShader  = compile ps_4_0_level_9_1 BasicShader();
    }
}
