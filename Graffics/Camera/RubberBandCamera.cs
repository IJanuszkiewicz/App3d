using Graffics.Movement;
using OpenTK.Mathematics;

namespace Graffics;

public class RubberBandCamera : Camera
{
    private IMovementController _targetMovementController;
    private Vector3 _relativePosition;
    private Vector3 _currentPosition;

    public RubberBandCamera(IMovementController targetMovementController, float aspectRatio, Vector3 relativePosition)
        : base((targetMovementController.GetPositionMatrix() * new Vector4(relativePosition, 1)).Xyz, aspectRatio,
            false)
    {
        _targetMovementController = targetMovementController;
        _relativePosition = relativePosition;
        _currentPosition =
            (targetMovementController.GetPositionMatrix() * new Vector4(relativePosition, 1)).Xyz;
    }

    public override void Update(double dt)
    {
        _currentPosition +=
            ((new Vector4(_relativePosition, 1) * _targetMovementController.GetPositionMatrix()).Xyz -
             _currentPosition) * (float)dt * 10;
    }

    public override Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(
            _currentPosition,
            _targetMovementController.Position.Position + Vector3.UnitY / 2, Vector3.UnitY);
    }
}