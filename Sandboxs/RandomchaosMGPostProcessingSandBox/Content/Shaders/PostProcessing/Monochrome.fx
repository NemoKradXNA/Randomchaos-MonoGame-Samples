uniform extern texture sceneMap;
sampler screen = sampler_state 
{
    texture = <sceneMap>;    
};

struct PS_INPUT 
{
	float2 TexCoord	: TEXCOORD0;
};

float4 Invert(PS_INPUT Input) : COLOR0 
{
	float4 col = tex2D(screen, Input.TexCoord).r;
	return col;
}

technique PostInvert 
{
	pass P0
	{
		PixelShader = compile ps_4_0_level_9_1 Invert();
	}
}