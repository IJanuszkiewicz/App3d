using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics.Movement;

public class ChaoticMovementController : IMovementController
{
    private Vector3 _speed;
    private Vector3 _acceleration;
    private Random _rnd = new Random();

    public ChaoticMovementController()
    {
        _speed = new Vector3(0,0,4);
        _acceleration = new Vector3(0, 0, 0);
        Position = new FullPosition(Vector3.Zero);
    }

    public FullPosition UpdatePosition(double dt, KeyboardState keyboardState)
    {
        Position.Position += _speed*(float)dt ;
        Position.Front = - _speed;
        _speed += _acceleration*(float)dt;

        _acceleration = new Vector3((float)_rnd.NextDouble()- -.5f, (float)_rnd.NextDouble()-.5f, (float)_rnd.NextDouble()-.5f)*20;
        
        return Position;
    }

    public FullPosition Position { get; }
    public Matrix4 GetPositionMatrix()
    {
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