
using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Mathematics;

namespace Graffics;

public class Scene
{
    public IModel3d[] Models { get; }
    public PointLight[] PointLights { get; }
    private Camera[] _cameras;
    private int _cameraIndex;
    public Camera Camera => _cameras[_cameraIndex];

    private Scene(IModel3d[] models, PointLight[] lights, Camera[] cameras)
    {
        _cameras = cameras;
        _cameraIndex = 0;
        Models = models;
        
        var lightList = new List<PointLight>();
        foreach (var model in models)
        {
            foreach (var light in model.GetPointLights())
            {
                lightList.Add(light);
            }
        }

        var pointLights = lightList.ToArray();
        
        PointLights = lights.Concat(pointLights).ToArray();
    }

    public static Scene ShapesInSpace(Shader shader, float aspectRatio)
    {
        var containerTexture = Texture.LoadFromFile("Resources/container.png");
        IModel3d[] models =
        [
            Cube.MakeSpinning(1, containerTexture, shader, new Material(0.1f, 0.7f, 0.8f, 10),
                new FullPosition(new Vector3(0, 0, 0))),

            Cube.MakeSkybox("Resources/nightsky.png", shader),
            Cube.MakeGoingInCircles(3, new Vector3(0,-2, 0), 1, containerTexture, shader, new Material(0.1f, 0.7f, 0.8f, 10)),
            
            new PointLamp(new Vector3(1, 1, 1), shader,
                new NoMovementController(new FullPosition(new Vector3(-1, -1, 0)))),

            new PointLamp(new Vector3(1, 1, 1), shader,
                new NoMovementController(new FullPosition(new Vector3(0, 1, 1)))),
            new Sphere(2, 100, 100, new Material(0.1f, 0.4f, 0.6f, 5),
                new NoMovementController(new FullPosition(new Vector3(4, 4, -4))),
                Texture.LoadFromFile("Resources/shade.jpg"), shader: shader),
        ];
        
        Camera staticCamera = new Camera(new Vector3(-2,6,4), aspectRatio, false);
        staticCamera.Pitch = -45;
        
        
        return new Scene(models, [], [
            new Camera(new Vector3(0,0,2), aspectRatio, true),
            staticCamera,
        ]);
    }

    public void ChangeCamera()
    {
        _cameraIndex = (_cameraIndex + 1) % _cameras.Length;
    }
}