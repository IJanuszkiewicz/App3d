using OpenTK.Mathematics;

namespace Graffics;

public class SpotLight
{
    public SpotLight(Vector3 position, Vector3 direction, Vector3 color, float concentration)
    {
        Position = position;
        Direction = direction;
        Color = color;
        Concentration = concentration;
    }

    public Vector3 Position { get; set; }
    public Vector3 Direction { get; set; }
    public Vector3 Color { get; set; }
    public float Concentration { get; set; }
}