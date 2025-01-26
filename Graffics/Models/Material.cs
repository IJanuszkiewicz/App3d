namespace Graffics;

public class Material
{
    public Material(float ambient, float diffuse, float specular, float shininess)
    {
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
    }

    public float Ambient;
    public float Diffuse;
    public float Specular;
    public float Shininess;
}