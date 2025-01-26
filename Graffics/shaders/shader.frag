#version 330 core
out vec4 FragColor;

in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;

struct Material {
    float ambient;
    float diffuse;
    float specular;
    float shiny;
};

struct PointLight {
    vec3 position;
    vec3 color;
    vec3 attenuation;
};
#define MAX_POINT_LIGHTS 20
struct SpotLight {
    vec3 position;
    vec3 direction;
    vec3 color;
    float concentration;
    vec3 attenuation;
};
#define MAX_SPOT_LIGHTS 20

struct DirLight{
    vec3 color;
    vec3 direction;
};

uniform sampler2D texture0;
uniform PointLight pointLights[MAX_POINT_LIGHTS];
uniform int numPointLights;
uniform int numSpotLights;
uniform SpotLight spotLights[MAX_SPOT_LIGHTS];
uniform Material material;
uniform DirLight dirLight;
uniform float fogIntensity;
uniform vec3 fogColor;

vec3 CalcPointLight(vec3 color, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 lightDir, vec3 attenuation, vec3 lightPostion);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 lightDir);
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
float CalcFogFactor(vec3 fragPos);

void main()
{
    vec3 viewDir = vec3(0, 0, 0);
    vec3 norm = normalize(Normal);
    vec3 result = CalcDirLight(dirLight, norm, FragPos, viewDir);
    for (int i = 0; i < numPointLights; i++) {
        vec3 lightDir = normalize(pointLights[i].position - FragPos);
        result += CalcPointLight(pointLights[i].color, norm, FragPos, viewDir, lightDir, pointLights[i].attenuation, pointLights[i].position);
    }
    for (int i = 0; i < numSpotLights; i++){
        vec3 lightDir = normalize(spotLights[i].position - FragPos);
        result += CalcSpotLight(spotLights[i], norm, FragPos, viewDir, lightDir);
    }
    FragColor = vec4(result, 1.0);
}

vec3 CalcPointLight(vec3 color, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 lightDir, vec3 attenuation, vec3 lightPostion)
{
    vec3 ambient = color * material.ambient * vec3(texture(texture0, TexCoord));

    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = color * diff * vec3(texture(texture0, TexCoord)) * material.diffuse;

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shiny);
    vec3 specular = color * spec * material.specular * vec3(texture(texture0, TexCoord));

    float distance = length(fragPos - lightPostion);
    float attenuationFactor =
    1.0 / (attenuation.x +
    attenuation.y * distance +
    attenuation.z * (distance * distance));

    vec3 result = (ambient + diffuse + specular) * attenuationFactor;
    result.x = clamp(result.x, 0.0, 1.0);
    result.y = clamp(result.y, 0.0, 1.0);
    result.z = clamp(result.z, 0.0, 1.0);
    float fogFactor = CalcFogFactor(FragPos);
    vec3 resultWithFog = mix(fogColor, result, fogFactor);
    return resultWithFog;
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 lightDir) {
    vec3 pl = CalcPointLight(light.color, normal, fragPos, viewDir, lightDir, vec3(1, 0, 0), light.position);
    return pl * pow(max(dot(lightDir, -light.direction), 0.0), light.concentration);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 fragPos, vec3 viewDir) {
    return CalcPointLight(light.color, normal, fragPos, viewDir, -light.direction, vec3(1, 0, 0), vec3(0, 0, 0));
}

float CalcFogFactor(vec3 fragPos) {
    if (fogIntensity == 0) return 1.0f;
    float gradient = (fogIntensity * fogIntensity - 50 * fogIntensity + 60);
    float distance = length(fragPos);
    float fog = exp(-pow((distance / gradient), 4));
    fog = clamp(fog, 0.0, 1.0);
    return fog;
}