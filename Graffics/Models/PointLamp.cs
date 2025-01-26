using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics;

class PointLamp : IGameObject
{
    private PointLight _light;
    private Sphere _shade;

    public PointLamp(Vector3 color, Shader shader, IMovementController movementController)
    {
        _light = new PointLight(movementController.Position.Position, color, new Vector3(0.6f, 0.5f, 0.5f));
        var texture = Texture.LoadFromFile("Resources/shade.jpg");
        _shade = new Sphere(0.1f, 30, 30, new Material(0.65f, 0, 0, 1), movementController, texture, shader);
    }

    public void Render()
    {
        _shade.Render();
    }

    public void Update(double dt, KeyboardState keyboard)
    {
        _shade.Update(dt, keyboard);
        _light.Position = _shade.MovementController.Position.Position;
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
        return [_light];
    }

    public SpotLight[] GetSpotLights()
    {
        return [];
    }
}