float4x4 world : World;
float4x4 wvp : WorldViewProjection;
float4x4 itw : WorldInverseTranspose;
float AmbientIntensity = 1;
float4 AmbientColor : Ambient = float4(.5,.5,.5,1);
float3 LightDirection : Direction = float3(0,50,10);

struct VS_IN
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;
};

struct VS_OUT
{
	float4 Position : POSITION;
	float3 Light : TEXCOORD0;
	float3 Normal : TEXCOORD1;
};

struct PS_OUT
{
	float4 Color : COLOR;
};

VS_OUT VS_AmbientDiffuse(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);
	output.Light = normalize(LightDirection);
	output.Normal = normalize(mul(itw,input.Normal));
	
	return output;
}

PS_OUT PS_AmbientDiffuse(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	output.Color = (AmbientIntensity * AmbientColor) + saturate(dot(input.Light,input.Normal));
	
	return output;
}

technique AmbientDiffuseLight
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_AmbientDiffuse();
		PixelShader = compile ps_4_0 PS_AmbientDiffuse();
	}
}