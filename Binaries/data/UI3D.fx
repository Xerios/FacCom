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

//---------------------------------------------------------------------------
Texture2D Texture : register(t0);
//---------------------------------------------------------------------------

struct VS_IN {
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD;
	float4 Color    : COLOR;
};
struct PS_IN {
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Color    : COLOR;
	float4 LightSpacePos : TEXCOORD1;
};

//---------------------------------------------------------------------------

PS_IN VS( VS_IN input ) {
	PS_IN output = (PS_IN)0;
	
	output.Position = mul(input.Position, ViewProjection);
	output.TexCoord = input.TexCoord;
	output.Color = input.Color;

	return output;
}

float4 PS( PS_IN input ) : SV_Target {
	return Texture.Sample(LinearSampler, input.TexCoord) * input.Color;
}