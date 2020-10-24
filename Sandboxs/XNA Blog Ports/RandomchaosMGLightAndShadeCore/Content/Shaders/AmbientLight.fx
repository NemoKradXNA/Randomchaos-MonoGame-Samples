float4x4 wvp : WorldViewProjection;
float AmbientIntensity = 1;
float4 AmbientColor : AMBIENT = float4(.5,.5,.5,1);

struct VS_IN
{
	float4 Position : POSITION;
};
struct VS_OUT
{
	float4 Position : POSITION;
};
struct PS_OUT
{
	float4 Color : COLOR;
};

VS_OUT VS_Ambient(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);
	
	return output;
}
PS_OUT PS_Ambient(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	output.Color = AmbientIntensity * AmbientColor;
	
	return output;
}

technique AmbientLight
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_Ambient();
		PixelShader = compile ps_4_0 PS_Ambient();
	}
}
