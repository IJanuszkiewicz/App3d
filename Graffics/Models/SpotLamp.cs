using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics;

public class SpotLamp : IGameObject
{
    private Cube _shade;
    private SpotLight _spotLight;

    public SpotLamp(Vector3 color, float concentration, IMovementController movementController, Shader shader)
    {
        _spotLight = new SpotLight(movementController.Position.Position, movementController.Position.Front, color, concentration);
        _shade = new Cube(0.1f, Texture.LoadFromFile("Resources/shade.jpg"), shader, new Material(0.65f, 0, 0, 1), movementController);
    }
    
    public void Render()
    {
        _shade.Render();
    }

    public void Update(double dt, KeyboardState keyboardState)
    {
        _shade.Update(dt, keyboardState);
        _spotLight.Position = _shade.MovementController.Position.Position;
        _spotLight.Direction = _shade.MovementController.Position.Front;
    }

    public Matrix4 GetModelMatrix()
    {
        return _shade.GetModelMatrix();
    }

    public Material GetMaterial()
    {
        return _shade.GetMaterial();
    }

    public PointLight[] GetPointLights()
    {
        return [];
    }

    public SpotLight[] GetSpotLights()
    {
        return [_spotLight];
    }
}