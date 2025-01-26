using Graffics.Movement;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics;

public class SpaceShip : AdvancedModel
{
    private const float SpotLightAngularSpeed = 1;

    public SpaceShip(IMovementController movementController) : base("Resources/Xwing/scene.gltf", movementController,
        30,
        new Material(0.1f, 0.6f, 0.3f, 3), Texture.LoadFromFile("Resources/Shuttle/metal.jpeg"), [], [], [
            new SpotLight(Vector3.Zero, Vector3.UnitZ, new Vector3(1, 1, 1), 100)
        ], [(Vector3.Zero, Vector3.UnitZ)])
    {
    }

    public override void Update(double dt, KeyboardState keyboard)
    {
        float spin = 0;
        if (keyboard.IsKeyDown(Keys.Right))
        {
            spin -= SpotLightAngularSpeed;
        }

        if (keyboard.IsKeyDown(Keys.Left))
        {
            spin += SpotLightAngularSpeed;
        }

        if (spin != 0)
        {
            _spotLightRelativePositions[0].dir =
                _spotLightRelativePositions[0].dir * Matrix3.CreateRotationY(spin * (float)dt);
        }

        base.Update(dt, keyboard);
    }
}