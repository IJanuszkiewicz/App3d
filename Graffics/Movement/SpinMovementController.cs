using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics.Movement;

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

    public FullPosition UpdatePosition(double dt, KeyboardState keyboardState)
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