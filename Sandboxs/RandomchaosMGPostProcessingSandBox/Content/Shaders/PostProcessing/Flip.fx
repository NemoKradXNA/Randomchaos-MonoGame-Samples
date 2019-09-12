uniform extern texture sceneMap;
sampler screen = sampler_state 
{
    texture = <sceneMap>;    
};

struct PS_INPUT
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};

float4 Invert(PS_INPUT Input) : COLOR0 
{
	Input.TexCoord.y = 1 - Input.TexCoord.y;
	float4 col = tex2D(screen, Input.TexCoord);
	return col;
}

technique PostInvert 
{
	pass P0
	{
		PixelShader = compile ps_4_0_level_9_1 Invert();
	}
}