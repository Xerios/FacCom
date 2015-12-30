cbuffer PerFrameBuffer : register( b0 )
{
	float4x4 ViewProjection: packoffset(c0);
	float4   CameraPosition: packoffset(c4);
	float4   AmbientColor: packoffset(c5);
	float3   SunColor: packoffset(c6);
	float    Time: packoffset(c6.w);
	float3   SunDirection: packoffset(c7);
	float    EyeHeight: packoffset(c7.w);
}

SamplerState ClampSampler : register(s0);
SamplerState LinearSampler : register(s1);

//----------------------------------------

Texture2D terrainTextureMap: register(t0);
Texture2D terrainColorMap: register(t1);
Texture2D <float> terrainLightMap: register(t2);
Texture2DArray terrainTextures: register(t3);


cbuffer ObjectBuffer : register( b1 )
{
	float4 waterColor:packoffset(c0);
	float waterLevel:packoffset(c1.x);
}

//---------------------------------------------------------------------------

struct VS_IN {
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
	float3 Normal : NORMAL;
};

struct PS_IN {
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float  Height : TEXCOORD1;
	float3 Normal : NORMAL;
	float3 LightDir : TEXCOORD2;
	float3 EyeView : TEXCOORD3;
	float4 LightSpacePos : TEXCOORD4;
};

//---------------------------------------------------------------------------

PS_IN VS( VS_IN input ) {
	PS_IN output = (PS_IN)0;
	
	output.Position = mul(input.Position,ViewProjection);
	output.Height = input.Position.y;
	output.Normal = input.Normal;
	
	output.TexCoord = input.TexCoord;
	
	float3 n = input.Normal;
	float3 b = cross( float3(1,0,0), n );
	float3 t = cross( n, b );
	float3x3 tbnMatrix = float3x3(t.x, b.x, n.x, 
								  t.y, b.y, n.y, 
								  t.z, b.z, n.z);
								  
	output.LightDir = mul(-SunDirection,tbnMatrix);
	
	output.EyeView = CameraPosition-input.Position;

	
	return output;
}


float4 PS( PS_IN input ) : SV_Target {


	float4 texturemap = terrainTextureMap.Sample(LinearSampler, input.TexCoord);
	float normalGroundVisibility = saturate(1- texturemap.r - texturemap.g - texturemap.b);

	float2 texCoord = input.TexCoord * 50;
	float3 diffuse = 0;
	diffuse+=terrainTextures.Sample(LinearSampler, float3(texCoord,0) ).rgb * texturemap.r;
	diffuse+=terrainTextures.Sample(LinearSampler, float3(texCoord,2) ).rgb * texturemap.g;
	diffuse+=terrainTextures.Sample(LinearSampler, float3(texCoord,4) ).rgb * texturemap.b;
	diffuse+=terrainTextures.Sample(LinearSampler, float3(texCoord,6) ).rgb * normalGroundVisibility;
	
	float3 normal=0;
	normal+=terrainTextures.Sample(LinearSampler, float3(texCoord,1) ).rgb * texturemap.r;
	normal+=terrainTextures.Sample(LinearSampler, float3(texCoord,3) ).rgb * texturemap.g;
	normal+=terrainTextures.Sample(LinearSampler, float3(texCoord,5) ).rgb * texturemap.b;
	normal+=terrainTextures.Sample(LinearSampler, float3(texCoord,7) ).rgb * normalGroundVisibility;
	normal = normalize(2.0f * normal - 1.0f);
	
	float shade = dot(normal,normalize(input.LightDir));
	float shadowOcclusion=1;

	shadowOcclusion = min(shadowOcclusion,terrainLightMap.Sample(ClampSampler, input.TexCoord).r);
	
	float darkness = max(0,min(shade,shadowOcclusion));
	
	float shadeSecondQuartOut= max(-(pow(shade-1,8)-1),0.5f);
	
	diffuse = diffuse * terrainColorMap.Sample(LinearSampler, input.TexCoord) * lerp(AmbientColor,SunColor,darkness) * shadeSecondQuartOut;

	if (waterLevel!=0){
		float coast = saturate(((input.Height/waterLevel)-0.99f)/(1-0.99f));
		float coastWide = saturate(((input.Height/waterLevel)-0.95f)/(1-0.95f));
		float depth = saturate(input.Height/waterLevel);
		diffuse = lerp(depth * waterColor.rgb,diffuse.rgb,depth)*min(1-waterColor.a,0.8f) + lerp(waterColor.rgb,diffuse.rgb,coast)*max(0.2f,waterColor.a);
		
		diffuse =lerp(diffuse*(1+(coastWide * max(0,shade) * 2)),diffuse.rgb,coast);
	}



	return float4(diffuse,0);
}