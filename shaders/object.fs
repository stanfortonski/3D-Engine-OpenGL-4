//Copyright (c) 2020 by Stan Fortoński
#version 410 core

layout (location = 0) out vec4 fragColor;
layout (location = 1) out vec4 brightColor;

in VS_OUT
{
  vec3 fragPos;
  vec3 normal;
  vec2 texCoords;
} fs_in;

const vec3 BRIGHT_FACTOR = vec3(0.2126, 0.7152, 0.0722);
const int AMOUNT_OF_POINT_LIGHTS = 1;
const int AMOUNT_OF_TEXTURES = 10;
const int DISCS_PCF_AMOUNT = 20;
const vec3 DISCS_PCF[DISCS_PCF_AMOUNT] = vec3[](
   vec3(1, 1, 1), vec3(1, -1, 1), vec3(-1, -1, 1), vec3(-1, 1, 1),
   vec3(1, 1, -1), vec3(1, -1, -1), vec3(-1, -1, -1), vec3(-1, 1, -1),
   vec3(1, 1, 0), vec3(1, -1, 0), vec3(-1, -1, 0), vec3(-1, 1, 0),
   vec3(1, 0, 1), vec3(-1, 0, 1), vec3(1, 0, -1), vec3(-1, 0, -1),
   vec3(0, 1, 1), vec3(0, -1, 1), vec3(0, -1, -1), vec3(0, 1, -1)
);

struct Material
{
  vec3 ambient;
  vec3 diffuse;
  vec3 specular;
  float shininess;
  int isSupportTex;
  float transparency;
};

struct PointLight
{
  vec3 position;
  float constant;
  float linear;
  float quadratic;
  vec3 diffuse;
  vec3 ambient;
  vec3 specular;
};

uniform vec3 viewPos;
uniform float farPlane;
uniform int diffuseTexturesAmount;
uniform int specularTexturesAmount;
uniform int lightsAmount;
uniform sampler2D texture_diffuse[AMOUNT_OF_TEXTURES];
uniform sampler2D texture_specular[AMOUNT_OF_TEXTURES];
uniform PointLight lights[AMOUNT_OF_POINT_LIGHTS];
uniform samplerCube shadowMaps[AMOUNT_OF_POINT_LIGHTS];
uniform Material mat;
uniform int allowShadows;

float shadowCalculation(samplerCube shadowMap, vec3 lightPos);
vec4 calcPointLight(PointLight light, Material material, vec3 viewDir, vec3 normal, float shadow);
vec4 calcFog(vec4 result);

void main()
{
  vec3 viewDir = normalize(viewPos - fs_in.fragPos);

  Material material;
  material.shininess = mat.shininess;
  if (mat.isSupportTex == 1)
  {
    material.diffuse = vec3(1.0);
    for (int i = 0; i < diffuseTexturesAmount; ++i)
      material.diffuse *= texture(texture_diffuse[i], fs_in.texCoords).rgb;
    material.ambient = material.diffuse;

    material.specular = vec3(1.0);
    for (int i = 0; i < specularTexturesAmount; ++i)
      material.specular *= texture(texture_specular[i], fs_in.texCoords).rgb;
  }
  else material = mat;

  vec4 result = vec4(0.0, 0.0, 0.0, 1.0);
  vec3 testBright = material.ambient + material.diffuse + material.specular;
  float bright = dot(testBright, BRIGHT_FACTOR);
  if (bright < 1.0)
  {
    for (int i = 0; i < lightsAmount; ++i)
    {
      float pointShadow = 0.0;
      if (allowShadows == 1)
        pointShadow = shadowCalculation(shadowMaps[i], lights[i].position);
      else result += calcPointLight(lights[i], material, viewDir, fs_in.normal, pointShadow);
    }
  }
  else result = vec4(testBright, 1.0);

  float brightness = dot(result.xyz, BRIGHT_FACTOR);
  if (brightness > 1.0)
    brightColor = vec4(result.xyz, 1.0);
  else brightColor = vec4(0.0, 0.0, 0.0, 1.0);

  fragColor = calcFog(result);
}

float shadowCalculation(samplerCube shadowMap, vec3 lightPos)
{
  const float bias = 0.15;

  vec3 LightToFragDir = fs_in.fragPos - lightPos;
  float currentDepth = length(LightToFragDir) - bias;
  float viewDistance = length(viewPos - fs_in.fragPos);
  float diskRadius = clamp(viewDistance / farPlane, 0.009, 0.01);

  float shadow = 0.0;
  for (int i = 0; i < DISCS_PCF_AMOUNT; ++i)
  {
    float closestDepth = texture(shadowMap, LightToFragDir + DISCS_PCF[i] * diskRadius).r;
    closestDepth *= farPlane;
    if (currentDepth > closestDepth)
      shadow += 1.0;
  }
  shadow /= float(DISCS_PCF_AMOUNT);
  return shadow;
}

vec4 calcPointLight(PointLight light, Material material, vec3 viewDir, vec3 normal, float shadow)
{
  vec3 fragToLightDir = light.position - fs_in.fragPos;
  float distances = length(fragToLightDir);
  fragToLightDir = normalize(fragToLightDir);
  float weakening = 1.0 / (light.constant + light.linear * distances + light.quadratic * pow(distances, 2));

  float diff = max(dot(fragToLightDir, normal), 0.0);
  vec3 halfwayDir = normalize(fragToLightDir + viewDir);
  float specAngle = max(dot(halfwayDir, normal), 0.0);
  float spec = pow(specAngle, material.shininess);

  vec3 ambient = light.ambient * material.ambient * weakening;
  vec3 diffuse = light.diffuse * diff * weakening;
  vec3 specular = light.specular * spec * weakening;
  vec3 result = (ambient + (1.0 - shadow) * (diffuse + specular)) * (material.diffuse + material.specular);
  return vec4(result, material.transparency);
}

vec4 calcFog(vec4 result)
{
  const vec4 fogColor = vec4(0.6, 0.7, 0.8, 1.0);
  const float fogStart = 120;
  const float fogEnd = 200;
  float dist = length(viewPos - fs_in.fragPos);
  float fogFactor = clamp(((fogEnd - dist)/(fogEnd - fogStart)), 0.125, 1.0);
  return mix(fogColor, result, fogFactor);
}
