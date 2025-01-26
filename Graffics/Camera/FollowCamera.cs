using Graffics.Movement;
using OpenTK.Mathematics;

public class FollowCamera : Camera
{
    private FullPosition _target;
    private Vector3 _up;
    public FollowCamera(Vector3 position, float aspectRatio, FullPosition target) : base(position, aspectRatio, false)
    {
        _target = target;
        _up = Vector3.UnitY;
    }

    public override Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, _target.Position, _up);
    }
}