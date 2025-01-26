using OpenTK.Mathematics;

namespace Graffics;

public class PointLight(Vector3 position, Vector3 color, Vector3 attenuation)
{
    public Vector3 Position = position;
    public Vector3 Color = color;
    public Vector3 AttenuationCoefficients = attenuation;
}