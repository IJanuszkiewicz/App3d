using OpenTK.Mathematics;

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
    public FullPosition UpdatePosition(double dt);
    public FullPosition Position { get; }
    public Matrix4 GetPositionMatrix();
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

    public FullPosition UpdatePosition(double dt)
    {
        return Position;
    }
}

public class SpinMovementController(double spinSpeed, Vector3 axis, FullPosition initialPosition)
    : IMovementController
{
    private readonly FullPosition _initialPosition = initialPosition;
    private readonly Vector3 _axis = axis;
    private double _angle = 0;

    public FullPosition Position { get; } = initialPosition.Copy();

    public Matrix4 GetPositionMatrix()
    {
        var position = Position.Position;
        var front = Position.Front;
        var up = Position.Up;
        return Matrix4.LookAt(-position, -position + front, up);
    }

    public FullPosition UpdatePosition(double dt)
    {
        _angle += dt * spinSpeed;
        if (Math.Abs(_angle) > 2 * Math.PI)
        {
            _angle = 0;
        }

        var mat = Matrix3.CreateFromAxisAngle(_axis, (float)_angle);
        Position.Front = mat * _initialPosition.Front;
        Position.Up = mat * _initialPosition.Up;

        return Position;
    }
}

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
        var position = Position.Position;
        var front = Position.Front;
        var up = Position.Up;
        return Matrix4.LookAt(-position, -position + front, up);
    }
    public FullPosition UpdatePosition(double dt)
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