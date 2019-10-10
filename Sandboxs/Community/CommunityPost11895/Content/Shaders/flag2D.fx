
float Time;

float2 amplitude = float2(.01f, .1f);
float2 frequency = float2(12, 8);

Texture2D Texture;
sampler TextureSampler
{
	Texture = <Texture>;
};

struct vsOutput
{
	float4 position : SV_Position;
	float4 color : COLOR0;
	float2 texCoord : TEXCOORD0;
};

float2 CalcUVAnimation(float2 Offset)
{
	return float2 ((sin(Time + Offset.y * frequency.x) * amplitude.x) + Offset.x, (cos(Time + Offset.x * frequency.y) * amplitude.y) + Offset.y);
}

float4 main(vsOutput input) : COLOR0
{
	float2 uv = CalcUVAnimation(input.texCoord);
	float4 color = tex2D(TextureSampler, uv);
	return color;
}

technique Falg
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 main();
	}
}