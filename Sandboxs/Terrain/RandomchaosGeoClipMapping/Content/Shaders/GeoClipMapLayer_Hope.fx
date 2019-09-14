#include "Shared.fxh"

float4x4 world : World;
float4x4 wvp : WorldViewProjection;


texture heightMap;

sampler ElevationSampler = sampler_state
{
	Texture = <heightMap>;  
	MinFilter = POINT;
	MagFilter = POINT;
	MipFilter = POINT;
};

struct OUTPUT 
{
  vector pos   : POSITION;
  float2 uv    : TEXCOORD0; // coordinates for normal-map lookup
  float  z     : TEXCOORD1; // coordinates for elevation-map lookup
  float alpha  : TEXCOORD2; // transition blend on normal map
};

uniform float4 ScaleFactor, FineBlockOrig;
uniform float2 ViewerPos, AlphaOffset, OneOverWidth;
uniform float  ZScaleFactor, ZTexScaleFactor;
uniform matrix WorldViewProjMatrix;

// Vertex shader for rendering the geometry clipmap
OUTPUT RenderVS(float2 gridPos: TEXCOORD0)
{
	OUTPUT output = (OUTPUT)0;

	// convert from grid xy to world xy coordinates
	//  ScaleFactor.xy: grid spacing of current level
	//  ScaleFactor.zw: origin of current block within world
	float2 worldPos = gridPos * ScaleFactor.xy + ScaleFactor.zw;

	// compute coordinates for vertex texture
	// FineBlockOrig.xy: 1/(w, h) of texture
	// FineBlockOrig.zw: origin of block in texture
	float2 uv = gridPos * FineBlockOrig.xy + FineBlockOrig.zw;

	// sample the vertex texture
	float zf_zd = tex2Dlod(ElevationSampler, float4(uv, 0, 1));

	// unpack to obtain zf and zd = (zc - zf)
	//  zf is elevation value in current (fine) level
	//  zc is elevation value in coarser level
	float zf = floor(zf_zd);

	float zd = frac(zf_zd) * 512 - 256; 
	// (zd = zc - zf)

	// compute alpha (transition parameter) and blend elevation
	float2 alpha = 1;//clamp((abs(worldPos - ViewerPos) – AlphaOffset) * OneOverWidth, 0, 1);

	alpha.x = max(alpha.x, alpha.y);

	float z = zf + alpha.x * zd;

	z = z * ZScaleFactor;

	output.pos = mul(float4(worldPos.x, worldPos.y, z, 1), WorldViewProjMatrix);

	output.uv = uv;

	output.z = z * ZTexScaleFactor;

	output.alpha = alpha.x;

	return output;

}

// Parameters uv, alpha, and z are interpolated from vertex shader.
// Two texture samplers have min and mag filters set to linear:
//   NormalMapSampler:   2D texture containing normal map,
//   ZBasedColorSampler: 1D texture containing elevation-based color
uniform float3 LightDirection;

// Pixel shader for rendering the geometry clipmap
float4 RenderPS(float2 uv    : TEXCOORD0,
                float  z     : TEXCOORD1,
                float  alpha : TEXCOORD2) : COLOR
{
  float4 normal_fc = float4(0,1,0,0);//tex2D(NormalMapSampler, uv);

  // normal_fc.xy contains normal at current (fine) level
  // normal_fc.zw contains normal at coarser level
  // blend normals using alpha computed in vertex shader
  float3 normal = float3((1 - alpha) * normal_fc.xy +  alpha * (normal_fc.zw), 1);

  // unpack coordinates from [0, 1] to [-1, +1] range, and renormalize
  normal = normalize(normal * 2 - 1);

  // compute simple diffuse lighting
  float s = clamp(dot(normal, LightDirection), 0, 1);

  // assign terrain color based on its elevation
  return s * 1;//tex1D(ZBasedColorSampler, z);

}

technique Technique1
{
    pass Pass1
    {
		FILLMODE = WIREFRAME;
		CULLMODE = NONE;
        VertexShader = compile VS_SHADERMODEL RenderVS();
        PixelShader = compile PS_SHADERMODEL RenderPS();
    }
}
