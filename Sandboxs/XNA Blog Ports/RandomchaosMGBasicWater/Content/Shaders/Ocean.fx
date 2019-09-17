
/*********************************************************************NVMH3****
Path: $Id: //sw/devtools/ShaderLibrary/1.0/HLSL/Ocean.fx#1 $
File:  ocean.fx

Copyright NVIDIA Corporation 2003-2007
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY LOSS)
ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF NVIDIA HAS
BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.


% Simple ocean shader with animated bump map and geometric waves
% Based partly on "Effective Water Simulation From Physical Models", GPU Gems

keywords: material animation environment bumpmap

Sparkle Mod: C.Humphrey
Altered the shader so that the "HALO" sparkle effect could be switched on and off
rather than having either or.

******************************************************************************/


///////// UNTWEAKABLES //////////////////////

float4x4 world : World ;                   // World or Model matrix
float4x4 wvp : WorldViewProjection;    // Model*View*Projection
float4x4 viewI : ViewInverse  ;

float time : Time  ;

//////////////// TEXTURES ///////////////////

texture normalMap;

texture cubeMap;

sampler2D normalMapSampler = sampler_state
{
	Texture = <normalMap>;
	MagFilter = Linear;
	MinFilter = Linear;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

samplerCUBE envMapSampler = sampler_state
{
	Texture = <cubeMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

///////// TWEAKABLE PARAMETERS //////////////////

float bumpHeight = 0.1;

float2 textureScale = float2(8.0, 4.0);

float2 bumpSpeed  = float2 (-0.05, 0.0 );

float fresnelBias = 0.1;

float fresnelPower = 4.0;

float hdrMultiplier = 3.0;

float4 deepColor : Diffuse = float4(0.0f, 0.0f, 0.1f, 1.0f) ;

float4 shallowColor : Diffuse = float4( 0.0f, 0.5f, 0.5f, 1.0f );

float4 reflectionColor : Specular = float4( 1.0f, 1.0f, 1.0f, 1.0f );

// these are redundant, but makes the ui easier:
float reflectionAmount = 1.0f;

float waterAmount = 1.0f;

float waveAmp = 1.0;

float waveFreq = 0.1;

//////////// CONNECTOR STRUCTS //////////////////

struct AppData {
	float4 Position : POSITION0;   // in object space
	float2 TexCoord : TEXCOORD0;
	float3 Normal   : NORMAL0;
	
};

struct VertexOutput {
	float4 Position  : SV_Position;  // in clip space
	float2 TexCoord  : TEXCOORD0;
	float3 Normal    : NORMAL0;
	float3 TexCoord1 : TEXCOORD1; // first row of the 3x3 transform from tangent to cube space
	float3 TexCoord2 : TEXCOORD2; // second row of the 3x3 transform from tangent to cube space
	float3 TexCoord3 : TEXCOORD3; // third row of the 3x3 transform from tangent to cube space

	float2 bumpCoord0 : TEXCOORD4;
	float2 bumpCoord1 : TEXCOORD5;
	float2 bumpCoord2 : TEXCOORD6;

	float3 eyeVector  : TEXCOORD7;
};

// wave functions ///////////////////////
//
#define NWAVES 2


float waveFrequency[NWAVES];
float waveAmplitude[NWAVES];
float wavePhase[NWAVES];
float2 waveDirection[NWAVES];


float evaluateWave(float freq, float amp, float phase, float2 dir, float2 pos, float t)
{
	return amp * sin(dot(dir, pos) * freq + t * phase);
}

// derivative of wave function
float evaluateWaveDeriv(float freq, float amp, float phase, float2 dir, float2 pos, float t)
{
	return freq * amp * cos(dot(dir, pos) * freq + t * phase);
}

// sharp wave functions
float evaluateWaveSharp(float freq, float amp, float phase, float2 dir, float2 pos, float t, float k)
{
	return amp * pow(sin(dot(dir, pos)*freq + t * phase)* 0.5 + 0.5, k);
}

float evaluateWaveDerivSharp(float freq, float amp, float phase, float2 dir, float2 pos, float t, float k)
{
	return k * freq*amp * pow(sin(dot(dir, pos)*freq + t * phase)* 0.5 + 0.5, k - 1) * cos(dot(dir, pos)*freq + t * phase);
}



///////// SHADER FUNCTIONS ///////////////

VertexOutput BumpReflectWaveVS(AppData IN)
{
	VertexOutput OUT;

	waveFrequency[0] = waveFreq;
	waveAmplitude[0] = waveAmp;

	waveFrequency[1] = waveFreq * 2.0;
	waveAmplitude[1] = waveAmp * 0.5;

	float4 P = IN.Position;

	// sum waves    
	P.y = 0.0;
	float ddx = 0.0, ddy = 0.0;
	for (int i = 0; i < NWAVES; i++) 
	{
		P.y += evaluateWave(waveFrequency[i], waveAmplitude[i], wavePhase[i], waveDirection[i], P.xz, time);
		float deriv = evaluateWaveDeriv(waveFrequency[i], waveAmplitude[i], wavePhase[i], waveDirection[i], P.xz, time);
		ddx += deriv * waveDirection[i].x;
		ddy += deriv * waveDirection[i].y;
	}

	// compute tangent basis
	float3 B = float3(1, ddx, 0);
	float3 T = float3(0, ddy, 1);
	float3 N = float3(-ddx, 1, -ddy);

	OUT.Position = mul(P, wvp);

	// pass texture coordinates for fetching the normal map
	OUT.TexCoord.xy = IN.TexCoord * textureScale;

	time = fmod(time, 100.0);
	OUT.bumpCoord0.xy = IN.TexCoord*textureScale + time * bumpSpeed;
	OUT.bumpCoord1.xy = IN.TexCoord*textureScale*2.0 + time * bumpSpeed*4.0;
	OUT.bumpCoord2.xy = IN.TexCoord*textureScale*4.0 + time * bumpSpeed*8.0;

	// compute the 3x3 tranform from tangent space to object space
	float3x3 objToTangentSpace;
	// first rows are the tangent and binormal scaled by the bump scale
	objToTangentSpace[0] = bumpHeight * normalize(T);
	objToTangentSpace[1] = bumpHeight * normalize(B);
	objToTangentSpace[2] = normalize(N);

	OUT.TexCoord1.xyz = mul(objToTangentSpace, world[0].xyz);
	OUT.TexCoord2.xyz = mul(objToTangentSpace, world[1].xyz);
	OUT.TexCoord3.xyz = mul(objToTangentSpace, world[2].xyz);

	// compute the eye vector (going from shaded point to eye) in cube space
	float4 worldPos = mul(P, world);
	OUT.eyeVector = viewI[3] - worldPos; // view inv. transpose contains eye position in world space in last row
	return OUT;
}


// Pixel Shaders

//float4 BumpReflectMain(VertexOutput IN) : COLOR
//{
//	// fetch the bump normal from the normal map
//	float4 N;
//
//	if (!sparkle)
//		N = tex2D(normalMapSamplerHALO, IN.TexCoord.xy)*2.0 - 1.0;
//	else
//		N = tex2D(normalMapSampler, IN.TexCoord.xy)*2.0 - 1.0;
//
//
//	float3x3 m; // tangent to world matrix
//	m[0] = IN.TexCoord1;
//	m[1] = IN.TexCoord2;
//	m[2] = IN.TexCoord3;
//	float3 Nw = mul(m, N.xyz);
//
//	//    float3 E = float3(IN.TexCoord1.w, IN.TexCoord2.w, IN.TexCoord3.w);
//	   float3 E = IN.eyeVector;
//	   float3 R = reflect(-E, Nw);
//
//	   return texCUBE(envMapSampler, R);
//}



float4 OceanMain(VertexOutput IN) : COLOR0
{
	// sum normal maps
	half4 t0;
	half4 t1;
	half4 t2;

	t0 = tex2D(normalMapSampler, IN.bumpCoord0.xy)*2.0 - 1.0;
	t1 = tex2D(normalMapSampler, IN.bumpCoord1.xy)*2.0 - 1.0;
	t2 = tex2D(normalMapSampler, IN.bumpCoord2.xy)*2.0 - 1.0;
	
	half3 N = t0.xyz + t1.xyz + t2.xyz;
	//    half3 N = t1.xyz;

	   half3x3 m; // tangent to world matrix
	   m[0] = IN.TexCoord1;
	   m[1] = IN.TexCoord2;
	   m[2] = IN.TexCoord3;
	   half3 Nw = mul(m, N.xyz);
	   Nw = normalize(Nw);

	   // reflection
	   float3 ev = IN.eyeVector;
	   ev.x = 1 - ev.x;
	   float3 E = normalize(ev);
	   half3 R = reflect(-E, Nw);

	   half4 reflection = texCUBE(envMapSampler, R);
	   // hdr effect (multiplier in alpha channel)
	   reflection.rgb *= (1.0 + reflection.a*hdrMultiplier);

	   // fresnel - could use 1D tex lookup for this
	   half facing = 1.0 - max(dot(E, Nw), 0);
	   half fresnel = fresnelBias + (1.0 - fresnelBias)*pow(facing, fresnelPower);

	   half4 waterColor = lerp(deepColor, shallowColor, facing);


	   return waterColor * waterAmount + reflection * reflectionColor*reflectionAmount*fresnel;
}

//////////////////// TECHNIQUE ////////////////

technique Main 
{
	pass p0
	{
		//FILLMODE = WIREFRAME;
		CULLMODE = NONE;
		VertexShader = compile vs_4_0 BumpReflectWaveVS();
		Zenable = true;
		ZWriteEnable = true;
		PixelShader = compile ps_4_0 OceanMain();

	}
}

///////////////////////////////// eof ///