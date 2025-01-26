using OpenTK.Mathematics;

namespace Graffics;

public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoords;

    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords)
    {
        Position = position;
        Normal = normal;
        TexCoords = texCoords;
    }

    public float[] ToArray()
    {
        return new float[]
        {
            Position.X, Position.Y, Position.Z,
            Normal.X, Normal.Y, Normal.Z,
            TexCoords.X, TexCoords.Y
        };
    }
}