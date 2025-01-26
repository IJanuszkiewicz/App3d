using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics.Movement;

public class CirclesMovementController : IMovementController
{
    public CirclesMovementController(float radius, Vector3 center, float spinSpeed)
    {
        _radius = radius;
        _center = center;
        _spinSpeed = spinSpeed;

        Position = new FullPosition(center + new Vector3(radius, 0, 0), new Vector3(0,0,1), new Vector3(0,1,0));
    }

    private float _radius;
    private Vector3 _center;
    private double _angle = 0;
    private double _spinSpeed;

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
    public FullPosition UpdatePosition(double dt, KeyboardState keyboardState)
    {
        _angle += dt * _spinSpeed;
        if (Math.Abs(_angle) > 2 * Math.PI)
        {
            _angle = 0;
        }
        
        Position.Position = _center + new Vector3(_radius * MathF.Cos((float)_angle), 0, _radius * MathF.Sin((float)_angle));
        Position.Front = new Vector3(MathF.Cos((float)_angle + MathHelper.PiOver2), 0, MathF.Sin((float)_angle + MathHelper.PiOver2));
        return Position;
    }
}