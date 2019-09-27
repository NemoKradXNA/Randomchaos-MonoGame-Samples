/*
Volumetric flame effect
based on Yury Uralsky's "Volumetric Fire"
http://www.cgshaders.org/shaders/show.php?id=39
$Id: //sw/devtools/SDK/9.5/SDK/MEDIA/HLSL/Flame.fx#2 $
This revolves a cross section of a flame image around the Y axis to
produce a cylindrical volume, and then perturbs the texture coordinates
with 4 octaves of animated 3D procedural noise to produce the flame effect.
Mod History: [Oriional shader from the NVIDIA 9.5 SDK]
23//03/2007 C. Humphrey charles.humphrey53@yahoo.co.uk http://www.randomchaos.co.uk
Modified so that the shader can be placed on an object that has
its position other than at 0,0,0 and made compatible with XNA.
*/

// Added by C.Humphrey
float Index;
float ticks : Time;
/************* TWEAKABLES **************/
float noiseFreq= .10;
float noiseStrength= 1.0;
float timeScale= 1.0;
float3 noiseScale = { 1.0, 1.0, 1.0 };
float3 noiseAnim = { 0.0, -0.1, 0.0 };
float4 flameColor  = { 0.2, 0.2, 0.2, 1.0 };
float3 flameScale  = { 1.0, -1.0, 1.0 };
float3 flameTrans  = { 0.0, 0.0, 0.0 };
// Textures /////////////////
#define VOLUME_SIZE 32
texture noiseTexture;
texture flameTexture;

/****************************************************/
/********** SAMPLERS ********************************/
/****************************************************/
sampler3D noiseTextureSampler = sampler_state
{
	Texture = <noiseTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};
sampler2D flameTextureSampler = sampler_state
{
	Texture = <flameTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

// Vector-valued noise
float4 GenerateNoise4f(float3 Pos : POSITION) : COLOR
{
float4 c;
float3 P = Pos * VOLUME_SIZE;
c.r = noise(P);
c.g = noise(P + float3(11, 17, 23));
c.b = noise(P + float3(57, 93, 65));
c.a = noise(P + float3(77, 15, 111));
// return c*0.5+0.5;
return abs(c);
}
// Scalar noise
float GenerateNoise1f(float3 Pos : POSITION) : COLOR
{
float3 P = Pos * VOLUME_SIZE;
// return noise(P)*0.5+0.5;
return abs(noise(P));
}
// Tracked matricies
float4x4 wvp : WorldViewProjection  ;
float4x4 world : World;
//////////////////////////////
// Structures
struct appdata {
	float3 Position : SV_POSITION0;
	float4 UV : TEXCOORD0;
	float4 Normal : NORMAL;
};
struct vertexOutput {
	float4 HPosition : SV_POSITION0;
	float3 NoisePos : TEXCOORD0;
	float3 FlamePos : TEXCOORD1;
	float2 UV : TEXCOORD2;
};
// Vertex shader
vertexOutput flameVS(appdata IN)
{
	vertexOutput OUT;
	float4 objPos = float4(IN.Position.x, IN.Position.y, IN.Position.z, 1.0);
	float3 worldPos = mul(objPos, world).xyz;
	OUT.HPosition = mul(objPos, wvp);
	float time = fmod(ticks, 10.0); // avoid large texcoords
	OUT.NoisePos = worldPos * noiseScale*noiseFreq + time * timeScale*noiseAnim;
	OUT.FlamePos = worldPos * flameScale + flameTrans;
	// Mod by C.Humphrey so flame can be anywhere in the scene.
	IN.Position.y += Index;
	OUT.FlamePos.xz = IN.Position.xy;
	OUT.FlamePos.y = IN.Position.z;
	// End Mod
	OUT.UV = IN.UV;
	return OUT;
}
// Pixel shaders
half4 noise3D(uniform sampler3D NoiseMap, float3 P)
{
	//return tex3D(NoiseMap, P)*2-1;
	return tex3D(NoiseMap, P);
}
half4 turbulence4(uniform sampler3D NoiseMap, float3 P)
{
	half4 sum = noise3D(NoiseMap, P)*0.5 +
		noise3D(NoiseMap, P * 2)*0.25 +
		noise3D(NoiseMap, P * 4)*0.125 +
		noise3D(NoiseMap, P * 8)*0.0625;
	return sum;
}
half4 flamePS(vertexOutput IN) : COLOR
{
	// return tex3D(NoiseMap,IN.NoisePos) * flameColor;
	// return turbulence4(NoiseMap, IN.NoisePos) * flameColor;
	half2 uv;
	uv.x = length(IN.FlamePos.xz); // radial distance in XZ plane
	uv.y = IN.FlamePos.y;
	//uv.y += turbulence4(NoiseMap, IN.NoisePos) * noiseStrength;
	uv.y += turbulence4(noiseTextureSampler, IN.NoisePos) * noiseStrength / uv.x;
	return tex2D(flameTextureSampler, uv) + flameColor;
}

/****************************************************/
/********** TECHNIQUES ******************************/
/****************************************************/
technique ps20 
{
	pass p1d
	{
		VertexShader = compile vs_4_0 flameVS();
		/*
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = None;
		AlphaBlendEnable = true;
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = One;
		*/
		PixelShader = compile ps_4_0 flamePS();
	}
}
/***************************** eof ***/