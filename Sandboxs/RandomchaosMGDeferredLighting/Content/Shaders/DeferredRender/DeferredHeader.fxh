#define MaxBones 59

float2 halfPixel;

struct VertexShaderOutputToPS
{
	float4 Position : Position0;
	float4 color : COLOR0;
    float2 texCoord : TEXCOORD0;
    float4 ScreenPosition : TEXCOORD1;
};

struct PixelShaderOutput
{
	float4 Color	: COLOR0;	// Color
	float4 SGR		: COLOR1;	// Specular, Glow and Reflection values
	float4 Tangent	: COLOR2;	// Normal / Tangent map
	float4 Depth	: COLOR3;	// Depth map (R) Specular (G) Glow (B) and Reflection (A)
};