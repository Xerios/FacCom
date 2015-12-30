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
SamplerState AnistropicSampler : register(s1);

//---------------------------------------------------------------------------

Texture2D backbufferMap: register(t0);
Texture2D underwaterHeight: register(t1);
Texture2D terrainNormals: register(t2);
Texture2D waterNormalMap: register(t3);
Texture2D shoreMap: register(t4);


//---------------------------------------------------------------------------

struct VS_IN {
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
};

struct PS_IN {
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 StoredPos : TEXCOORD1;
	float2 BumpMapSamplingPos1 : TEXCOORD2;
	float2 BumpMapSamplingPos2 : TEXCOORD3;
	float3 WorldPos : TEXCOORD4;
};

//---------------------------------------------------------------------------

float3 tex2Dlod_bilinear(Texture2D tex, float2 uv){

	float2 TexelSize;
	tex.GetDimensions( TexelSize.x, TexelSize.y);
	TexelSize.xy = 1.0f/TexelSize.xy;
	
	float3 height00 = tex.SampleLevel(ClampSampler, uv,0).rgb;
	float3 height10 = tex.SampleLevel(ClampSampler, uv + float2(-TexelSize.x, 0),0).rgb; 
	float3 height01 = tex.SampleLevel(ClampSampler, uv + float2(0, TexelSize.y),0).rgb;
	float3 height11 = tex.SampleLevel(ClampSampler, uv + float2(TexelSize.x, TexelSize.y),0).rgb; 
	
	
	float2 f = frac( uv );

	float3 tA = lerp( height00, height10, f.x );
	float3 tB = lerp( height01, height11, f.x );

	return lerp( tA, tB, f.y );
}
//-----------------------------
PS_IN VS( VS_IN input ) {
	PS_IN output = (PS_IN)0;
	
	output.Position = mul(input.Position, ViewProjection);
	output.TexCoord = input.TexCoord;
	output.StoredPos = output.Position;
	
	output.BumpMapSamplingPos1 = input.TexCoord/0.05f + Time/2000;
	output.BumpMapSamplingPos2 = input.TexCoord/0.1f + Time/8000;
	output.WorldPos = input.Position;
	return output;
}


float4 PS( PS_IN input ) : SV_Target {

	float2 ProjectedTexCoords;
	ProjectedTexCoords.x = input.StoredPos.x/input.StoredPos.w/2.0f + 0.5f;
	ProjectedTexCoords.y = -input.StoredPos.y/input.StoredPos.w/2.0f + 0.5f;

	float3 bumpColor = waterNormalMap.Sample(AnistropicSampler, input.BumpMapSamplingPos1);
	float3 bumpColor2 = waterNormalMap.Sample(AnistropicSampler, input.BumpMapSamplingPos2);
	float3 perturbation = ((2*(bumpColor.rgb) - 1) + (2*(bumpColor2.rgb) - 1)) * 0.5f;
	
	float perturbationPower = (1.5f-(min(EyeHeight,500)/500));

	float3 normalRefract = backbufferMap.Sample(ClampSampler, ProjectedTexCoords);
	
	float3 refraction;
	refraction.r = backbufferMap.Sample(ClampSampler, ProjectedTexCoords + perturbation.rg * perturbationPower * 0.03f).r;
	refraction.g = backbufferMap.Sample(ClampSampler, ProjectedTexCoords + perturbation.rg * perturbationPower * 0.02f).g;
	refraction.b = backbufferMap.Sample(ClampSampler, ProjectedTexCoords + perturbation.rg * perturbationPower * 0.01f).b;
	
	float4 final = float4(refraction,1);//float4(lerp(refraction * SunColor,normalRefract,difference),1);
		
	//Working basic specular
	/*float3 h = normalize(normalize(CameraPosition - input.WorldPos) - SunDirection);
	float SpecularAttn =  max(0,pow(dot(float3(0,1,0), h),5));
	final.rgb += SpecularAttn*SunColor;*/


	float3 normal = terrainNormals.Sample(AnistropicSampler, input.TexCoord);
	float2 pnormal = normal.rb;
	pnormal.x += Time/2000;
	pnormal.y += Time/2000;

	float2 pnormal2 = normal.rb*5;
	pnormal2.x += Time/500;
	pnormal2.y += Time/500;


	//pnormal.g *= 10;
	//pnormal = abs(pnormal);

	//float4 foam = shoreMap.Sample(AnistropicSampler, pnormal.xz);

	float4 foam = normalize(shoreMap.Sample(AnistropicSampler, pnormal.xy) + shoreMap.Sample(AnistropicSampler, pnormal2.xy));
		
	//final.r=final.g=final.b = (max(1-normal.y,0));
	float2 TexelSize;
	underwaterHeight.GetDimensions( TexelSize.x, TexelSize.y);
	TexelSize.xy = 1.0f/TexelSize.xy;

	float level = underwaterHeight.Sample(ClampSampler, input.TexCoord + TexelSize*0.5f);
	
	final += min(foam*level*(max(1-normal.y,0)*5),0.3f);

	final.a = 0;
	return final;
}

