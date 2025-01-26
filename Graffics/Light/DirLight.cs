using OpenTK.Mathematics;

namespace Graffics;

public class DirLight
{
    public Vector3 Direction;
    public Vector3 Color;

    public DirLight(Vector3 color, Vector3 direction)
    {
        Color = color;
        Direction = direction;
    }
}