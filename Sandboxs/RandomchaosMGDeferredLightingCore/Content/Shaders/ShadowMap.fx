//http://www.opengl-tutorial.org/intermediate-tutorials/tutorial-16-shadow-mapping/

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 world : WorldViewProjection;
float4x4 vp : ViewProjection;

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 ScreenPosition : TEXCOORD0;
};

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
};


VertexShaderOutput VS_Shadows(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(mul(input.Position, world), vp);
	output.ScreenPosition = output.Position;

	return output;
}

struct outputPS
{
	float4 channel0 : COLOR0;
};

outputPS PS_Shadows(VertexShaderOutput input)
{
	outputPS output = (outputPS)0;
	output.channel0.r = 1 - ((input.ScreenPosition.z / input.ScreenPosition.w) * 1);
	output.channel0.a = 1;

	//output.channel0 = 1;
	return output;
}

technique ForwardShadowMap
{
	pass Pass1
	{
		//CullMode = None;
		VertexShader = compile VS_SHADERMODEL VS_Shadows();
		PixelShader = compile PS_SHADERMODEL PS_Shadows();
	}
}