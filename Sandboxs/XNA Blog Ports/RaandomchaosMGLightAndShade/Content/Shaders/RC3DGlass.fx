float4x4 wvp : WorldViewProjection;
float4x4 world : World;

float3 EyePosition : CAMERAPOSITION;
float4 tint = half4(1,1,1,1);

float3 etas = { 0.80, 0.82, 0.84 };

// wavelength colors
const half4 colors[3] = {
	{ 1, 0, 0, 0 },
	{ 0, 1, 0, 0 },
	{ 0, 0, 1, 0 },
};

struct VS_IN
{
	half4 Position : POSITION;
	half3 Normal : NORMAL;	
};
struct VS_OUT
{
	half4 Position : POSITION;
	half3 Reflect: TEXCOORD0;    
	half3 RefractRGB[3]: TEXCOORD1;	
};
struct PS_OUT
{
	half4 Color : COLOR;
};

texture cubeMap;
samplerCUBE CubeMap = sampler_state 
{ 
    texture = <cubeMap> ;     
};

half4 CubeMapLookup(half3 CubeTexcoord)
{    
    return texCUBE(CubeMap, CubeTexcoord);
}

half3 refract2( half3 I, half3 N, half eta)
{
	half IdotN = dot(I, N);
	half k = 1 - eta*eta*(1 - IdotN*IdotN);

	return eta*I - (eta*IdotN + sqrt(k))*N;
}

VS_OUT VS_Glass(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);	
	
	half3 Normal = mul(normalize(input.Normal), world);   
    half3 PosWorldr  = (mul(input.Position, world));
    half3 ViewDirection = normalize(PosWorldr - EyePosition);
	
	output.Reflect = reflect(ViewDirection, Normal);	
	
	// transmission
  	bool fail = false;
    for(int i=0; i<3; i++) 
    	output.RefractRGB[i] = refract2(ViewDirection, Normal, etas[i]);
	
	return output;
}
PS_OUT PS_Glass(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	half4 refract = half4(0,0,0,0);
	for(int c=0;c<3;c++)
		refract += CubeMapLookup(input.RefractRGB[c]) * colors[c];
	
	output.Color = lerp(refract,CubeMapLookup(input.Reflect),.5) * tint;
	
	return output;
}

technique Glass
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_Glass();
		PixelShader = compile ps_4_0 PS_Glass();
	}
}