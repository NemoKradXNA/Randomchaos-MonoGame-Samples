#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float Time;

float2 amplitude = float2(.01f, .1f);
float2 frequency = float2(12,8);

float3 lightDirection = float3(0, 0, -1);
float3 lightColor = float3(1, 1, 1);
float lightPower =1;

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

	// Calculate the normals - REALY don't think I am doing this right here, but it's better than nothing lol
	// Get the surrounding normals.
	float3 uv1 = float3(CalcUVAnimation(input.texCoord + float2(-.5,0)), 1);
	float3 uv2 = float3(CalcUVAnimation(input.texCoord + float2(-1, 0)), 1);
	float3 uv3 = float3(CalcUVAnimation(input.texCoord + float2(0, .5)), 1);

	// Calculate the normals
	float3 side1 = uv1 - uv3;
	float3 side2 = uv1 - uv2;

	// Find the cross product to get the normal..
	float3 normal = cross(side1, side2);

	// Calculate the diffuse term
	float3 diffuse = (lightPower * lightColor * saturate(dot(-lightDirection, normal))) * 2;

	float4 color = tex2D(TextureSampler, uv);

	// Apply lighting
	color.rgb *= diffuse;

	return color;
}

technique Falg
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL main();
	}
}