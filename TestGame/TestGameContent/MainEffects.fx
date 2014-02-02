#define SHADER20_MAX_BONES 58

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 BoneTransforms[SHADER20_MAX_BONES];
float3 LightPosition;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


// TODO: add effect parameters here.

// Skinned Vertex Input
struct skinnedVin
{
    float4 Position : POSITION;
	float2 TexCoords: TEXCOORD0;
	float3 Normal: NORMAL0;
	float4 BoneIndeces: BLENDINDICES0; 
	float4 Weights: BLENDWEIGHT0;
	
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

//Skinned Vertex Output
struct skinnedVout
{
    float4 Position : POSITION0;
	float2 TexCoords: TEXCOORD0;
	float4 Position3D: TEXCOORD1;
	float3 Normal: TEXCOORD2;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

skinnedVout SkinnedVertexShaderFunction(skinnedVin input)
{
    skinnedVout output;

	float4x4 boneTransform = BoneTransforms[input.BoneIndeces.x]*input.Weights.x;
	boneTransform +=BoneTransforms[input.BoneIndeces.y]*input.Weights.y;
	boneTransform +=BoneTransforms[input.BoneIndeces.z]*input.Weights.z;
	float finalWeight = 1.0f-input.Weights.x-input.Weights.y-input.Weights.z;
	boneTransform +=BoneTransforms[input.BoneIndeces.w]*finalWeight;
	//float4 bonePosition = mul(input.Position, boneTransform);
	float4 worldPosition = mul(input.Position, boneTransform);//mul(bonePosition, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoords = input.TexCoords;
	output.Position3D = worldPosition;
	output.Normal =mul(input.Normal, (float3x3)(boneTransform));
    return output;
}

float4 SkinnedPixelShaderFunction(skinnedVout input) : COLOR0
{
    // TODO: add your pixel shader code here.
    float3 lightDirection = normalize(LightPosition-input.Position3D);
	float3 normal = normalize(input.Normal);
	float lumen = saturate(dot(lightDirection,normal)+0.6f); 
	float4 color =tex2D(TextureSampler, input.TexCoords);
	color*=lumen;
	return color;
}

technique Skinned
{
    pass Pass0
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 SkinnedVertexShaderFunction();
        PixelShader = compile ps_2_0 SkinnedPixelShaderFunction();
    }
}

/*---very simple technique---*/


struct simpleVin
{
    float4 Position : POSITION;
	float2 TexCoords: TEXCOORD0;
	float3 Normal: NORMAL0;
};

struct simpleVout
{
    float4 Position : POSITION0;
	float2 TexCoords: TEXCOORD0;
	float4 Position3D: TEXCOORD1;
	float3 Normal: TEXCOORD2;
};

simpleVout SimpleVertexShaderFunction(simpleVin input)
{
    simpleVout output;
	
	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoords = input.TexCoords;
    output.Position3D = mul(input.Position, World);
	output.Normal = mul(input.Normal, (float3x3)World);
	return output;
}

float4 SimplePixelShaderFunction(simpleVout input) : COLOR0
{
    float3 lightDirection =  normalize(LightPosition-input.Position3D);
	float3 normal = normalize(input.Normal);
	float lumen = saturate(dot(lightDirection,normal)+0.6f); 
	float4 color =tex2D(TextureSampler, input.TexCoords);
	//if(lumen<0.5f)lumen=0.5f;
	color*=lumen;
	return color;
}

technique Textured
{
    pass Pass0
    {
        // TODO: set renderstates here.
        VertexShader = compile vs_2_0 SimpleVertexShaderFunction();
        PixelShader = compile ps_2_0 SimplePixelShaderFunction();
    }
}