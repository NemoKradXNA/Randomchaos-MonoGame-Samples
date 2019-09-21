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
	float3 Refract: TEXCOORD1;
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

VS_OUT VS_ReflectionAndRefraction(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);	
	
	float3 Normal = mul(normalize(input.Normal), world);   
    float3 PosWorldr  = (mul(input.Position, world));
    float3 ViewDirection = normalize(PosWorldr - EyePosition);
	
	output.Reflect = reflect(ViewDirection, Normal);	
	output.Refract = refract(ViewDirection, Normal, 0.99);
	
	return output;
}
PS_OUT PS_ReflectionAndRefraction(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	output.Color = lerp(CubeMapLookup(input.Refract),CubeMapLookup(input.Reflect),.5f) * tint;
	
	return output;
}

technique ReflectionAndRefraction
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_ReflectionAndRefraction();
		PixelShader = compile ps_4_0 PS_ReflectionAndRefraction();
	}
}