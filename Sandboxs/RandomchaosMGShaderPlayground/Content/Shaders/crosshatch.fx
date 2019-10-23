// Shader source: http://kylehalladay.com/blog/tutorial/2017/02/21/Pencil-Sketch-Effect.html

float4x4 world : WORLD;
float4x4 wvp : WorldViewProjection;

float3 lightDirection;
float hatchingDensity = 10;
texture textureMat;

sampler textureSample = sampler_state
{
	texture = <textureMat>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture BumpMap;
sampler BumpMapSampler = sampler_state
{
	Texture = <BumpMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture hatch0Map;
sampler hatch0Sampler = sampler_state
{
	Texture = <hatch0Map>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture hatch1Map;
sampler hatch1Sampler = sampler_state
{
	Texture = <hatch1Map>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
	float3 Normal	: Normal0;
	float3 Tangent	: Tangent0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
	float3 Normal : Normal0;
	float4 SPos : TexCoord1;
	float3x3 Tangent: Tangent0;
};

struct PixelShaderOutput
{
	float4 Color	: COLOR0;	// Color	
	float4 Depth	: COLOR1;	// Depth map 
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, wvp);
	output.TexCoord = input.TexCoord;

	output.Normal = mul(input.Normal, world);

	output.Tangent[0] = normalize(mul(input.Tangent, world));
	output.Tangent[1] = normalize(mul(cross(input.Tangent, input.Normal), world));
	output.Tangent[2] = normalize(output.Normal);

	output.SPos = output.Position;

	return output;
}

float3 Hatching(float2 _uv, float _intensity)
{
	float3 hatch0 = tex2D(hatch0Sampler, _uv).rgb;
	float3 hatch1 = tex2D(hatch1Sampler, _uv).rgb;

	float3 overbright = max(0, _intensity - 1.0);

	float3 weightsA = saturate((_intensity * 6.0) + float3(-0, -1, -2));
	float3 weightsB = saturate((_intensity * 6.0) + float3(-3, -4, -5));

	weightsA.xy -= weightsA.yz;
	weightsA.z -= weightsB.x;
	weightsB.xy -= weightsB.zy;

	hatch0 = hatch0 * weightsA;
	hatch1 = hatch1 * weightsB;

	float3 hatching = overbright + hatch0.r +
		hatch0.g + hatch0.b +
		hatch1.r + hatch1.g +
		hatch1.b;

	return hatching;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	float4 Color = tex2D(textureSample,input.TexCoord);

	// Get value in the range of -1 to 1
	float3 n = 2.0f * tex2D(BumpMapSampler, input.TexCoord) - 1.0f;
	n = mul(n, input.Tangent);

	float3 lightVector = normalize(-lightDirection);

	float NdL = saturate(dot(lightVector, n));

	output.Color = Color;// *NdL;

	float intensity = dot(NdL, float3(0.2326, 0.7152, 0.0722));
	output.Color.rgb *= Hatching(input.TexCoord * hatchingDensity, intensity);
	//output.Color *= NdL;

	

	//rgb += hatch.r * weights0;
	//rgb += hatch, g * weights1;
	//rgb += hatch.b * weights2;

	// Depth
	output.Depth.r = 1 - (input.SPos.z / input.SPos.w);
	output.Depth.a = 1;

	return output;
}

technique Technique1
{
	pass Pass1
	{
		// TODO: set renderstates here.

		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
	}
}

