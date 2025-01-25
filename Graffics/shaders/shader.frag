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
};
#define MAX_POINT_LIGHTS 20

uniform sampler2D texture0;
uniform PointLight pointLights[MAX_POINT_LIGHTS];
uniform int numPointLights;
uniform Material material;
uniform vec3 viewPos;

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 norm = normalize(Normal);
    vec3 result = vec3(0.0f, 0.0f, 0.0f);
    for (int i = 0; i < numPointLights; i++) {
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
    }
    FragColor = vec4(result, 1.0);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 ambient = light.color * material.ambient * vec3(texture(texture0, TexCoord));

    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = light.color * diff * vec3(texture(texture0, TexCoord)) * material.diffuse;

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shiny);
    vec3 specular = light.color * spec * material.specular * vec3(texture(texture0, TexCoord));

    vec3 result = ambient + diffuse + specular;
    return result;
}
