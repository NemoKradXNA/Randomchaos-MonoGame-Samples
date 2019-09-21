float4x4 wvp : WorldViewProjection;
float4x4 itw : WorldInverseTranspose;
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

VS_OUT VS_Diffuse(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);
	output.Light = normalize(LightDirection);
	output.Normal = normalize(mul(itw,input.Normal));
	
	return output;
}

PS_OUT PS_Diffuse(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
		
	output.Color = saturate(dot(input.Light,input.Normal));	
	
	return output;
}

technique DiffuseLight
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_Diffuse();
		PixelShader = compile ps_4_0 PS_Diffuse();
	}
}