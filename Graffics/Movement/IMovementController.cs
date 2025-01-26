using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics.Movement;

public class FullPosition(Vector3 position, Vector3 front, Vector3 up)

{
    public Vector3 Position = position;
    public Vector3 Front = front;
    public Vector3 Up = up;

    public FullPosition Copy() => new FullPosition(Position, Front, Up);
    public static FullPosition Zero() => new(Vector3.Zero, new Vector3(0, 0, -1), new Vector3(0, 1, 0));

    public FullPosition(Vector3 position) : this(position, new Vector3(0, 0, -1), new Vector3(0, 1, 0))
    {
    }
}

public interface IMovementController
{
    public FullPosition UpdatePosition(double dt, KeyboardState keyboardState);
    public FullPosition Position { get; }
    public Matrix4 GetPositionMatrix(){
        
        Vector3 f = Position.Front.Normalized();
        Vector3 u = Position.Up.Normalized();
        Vector3 r = Vector3.Cross(f, u).Normalized(); // Right vector
        u = Vector3.Cross(r, f).Normalized(); // Recalculate up to ensure orthogonality

        Matrix4 rotation = new Matrix4(
            new Vector4(r, 0.0f),
            new Vector4(u, 0.0f),
            new Vector4(-f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
        );

        Matrix4 translation = Matrix4.CreateTranslation(Position.Position);
        return rotation * translation;
    }
}

public class NoMovementController(FullPosition position) : IMovementController
{
    public FullPosition Position { get; } = position;

    public Matrix4 GetPositionMatrix()
    {
        var position = Position.Position;
        var front = Position.Front;
        var up = Position.Up;
        return Matrix4.LookAt(-position, -position + front, up);
    }

    public FullPosition UpdatePosition(double dt, KeyboardState keyboardState)
    {
        return Position;
    }
}