float4x4 wvp : WorldViewProjection;
float4x4 world : World;

float3 EyePosition : CAMERAPOSITION;
float4 tint = float4(1,1,1,1);

struct VS_IN
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;	
};
struct VS_OUT
{
	float4 Position : POSITION;
	float3 Reflect: TEXCOORD0;    
};
struct PS_OUT
{
	float4 Color : COLOR;
};

texture cubeMap;
samplerCUBE CubeMap = sampler_state 
{ 
    texture = <cubeMap> ;     
};

float4 CubeMapLookup(float3 CubeTexcoord)
{    
    return texCUBE(CubeMap, CubeTexcoord);
}

VS_OUT VS_Reflection(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);	
	
	float3 Normal = mul(normalize(input.Normal), world);   
    float3 PosWorldr  = (mul(input.Position, world));
    float3 ViewDirection = normalize(PosWorldr - EyePosition);
	
	output.Reflect = reflect(ViewDirection, Normal);	
	
	return output;
}
PS_OUT PS_Reflection(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	output.Color = CubeMapLookup(input.Reflect) * tint;
	
	return output;
}

technique Reflection
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_Reflection();
		PixelShader = compile ps_4_0 PS_Reflection();
	}
}