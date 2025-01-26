using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics.Movement;

public class KeyboardMovementController : IMovementController
{
    private Vector3 _speed;
    private double _acceleration;
    private double _friction;

    public KeyboardMovementController(FullPosition initialPosition, double acceleration, double friction)
    {
        Position = initialPosition.Copy();
        Position.Front = new Vector3(0, 0, 1);
        _acceleration = acceleration;
        _speed = Vector3.Zero;
        _friction = friction;
    }

    public FullPosition UpdatePosition(double dt, KeyboardState keyboardState)
    {
        Vector3 acceleration = Vector3.Zero;
        float turn = 0;
        if (keyboardState.IsKeyDown(Keys.A))
        {
            turn += MathHelper.PiOver2;
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            turn -= MathHelper.PiOver2;
        }

        if (keyboardState.IsKeyDown(Keys.W))
        {
            acceleration += new Vector3(0, 0, 1);
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            acceleration += new Vector3(0, 0, -1);
        }

        if (acceleration != Vector3.Zero)
        {
            var f = Position.Front;
            var u = Vector3.UnitY;

            Vector3 r = Vector3.Cross(f, u).Normalized(); // Right vector
            u = Vector3.Cross(r, f).Normalized(); // Recalculate up to ensure orthogonality

            Matrix3 rotation = new Matrix3(
                r,
                u,
                -f
            );
            _speed += (float)_acceleration * acceleration.Normalized() * rotation * (float)dt;
        }

        if (turn != 0)
        {
            _speed = _speed * Matrix3.CreateRotationY(turn * (float)dt);
        }

        _speed -= _speed * (float)_friction * (float)dt;
        Position.Position += _speed * (float)dt;
        if (_speed.Length > 0.01)
        {
            Position.Front = -_speed.Normalized();
        }

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