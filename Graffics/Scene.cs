using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Mathematics;

namespace Graffics;

public struct Fog
{
    public float FogIntensity;
    public Vector3 FogColor;

    public Fog(Vector3 fogColor, float fogIntensity)
    {
        FogColor = fogColor;
        FogIntensity = fogIntensity;
    }
}

public class Scene
{
    public IGameObject[] Models => _gameObjects.Concat([_isDay ? _daySkybox : _nightSkybox]).ToArray();
    private IGameObject[] _gameObjects;
    public PointLight[] PointLights { get; }
    public SpotLight[] SpotLights { get; }
    public DirLight DirLight { get; }
    private Camera[] _cameras;
    private int _cameraIndex;
    public Fog Fog { get; private set; }
    public Camera Camera => _cameras[_cameraIndex];
    public Camera[] Cameras => _cameras;

    private Cube _daySkybox;
    private Cube _nightSkybox;

    private bool _isDay = false;

    private Scene(IGameObject[] models, PointLight[] lights, Camera[] cameras, SpotLight[] spotLights,
        Cube daySkybox, Cube nightSkybox)
    {
        _cameras = cameras;
        _cameraIndex = 0;
        _gameObjects = models;
        _daySkybox = daySkybox;
        DirLight = new DirLight(new Vector3(0.1f, 0.1f, 0.1f), -Vector3.UnitY);
        _nightSkybox = nightSkybox;

        var lightList = new List<PointLight>();
        var spotLightList = new List<SpotLight>();
        foreach (var model in models)
        {
            foreach (var light in model.GetPointLights())
            {
                lightList.Add(light);
            }

            foreach (var spotLight in model.GetSpotLights())
            {
                spotLightList.Add(spotLight);
            }
        }

        var pointLights = lightList.ToArray();

        PointLights = lights.Concat(pointLights).ToArray();

        SpotLights = spotLights.Concat(spotLightList.ToArray()).ToArray();
        SwitchDay();
    }

    public static Scene ShapesInSpace(Shader shader, float aspectRatio)
    {
        var containerTexture = Texture.LoadFromFile("Resources/container.png");
        var circlesCube =
            Cube.MakeGoingInCircles(3, new Vector3(0, -2, 0), 1, containerTexture, shader,
                new Material(0.1f, 0.7f, 0.8f, 10));
        var ship = new SpaceShip(new CirclesMovementController(5, new Vector3(0, 3, 0), -1));
        var shipChaotic = new SpaceShip(new KeyboardMovementController(new FullPosition(Vector3.Zero), 10, 0.2));
        IGameObject[] models =
        [
            Cube.MakeSpinning(1, containerTexture, shader, new Material(0.1f, 0.7f, 0.8f, 10),
                new FullPosition(new Vector3(0, 0, 0))),

            circlesCube,
            new PointLamp(new Vector3(1, 1, 1), shader,
                new NoMovementController(new FullPosition(new Vector3(0, 1, 1)))),
            new Sphere(2, 100, 100, new Material(0.1f, 0.4f, 0.6f, 5),
                new NoMovementController(new FullPosition(new Vector3(4, 4, -4))),
                Texture.LoadFromFile("Resources/shade.jpg"), shader: shader),
            ship,
            shipChaotic,
            new SpotLamp(
                new Vector3(1, 1, 1),
                100,
                new NoMovementController(new FullPosition(new Vector3(-2, 0, 0), new Vector3(1, 0, 0), Vector3.UnitY)),
                shader
            ),
        ];

        Camera staticCamera = new Camera(new Vector3(-2, 6, 4), aspectRatio, false)
        {
            Pitch = -45
        };

        var nightSkybox = Cube.MakeSkybox("Resources/nightsky.png", shader);
        var daySkybox = Cube.MakeSkybox("Resources/sky-texture.jpg", shader);
        return new Scene(models, [], [
                new RubberBandCamera(shipChaotic.MovementController, aspectRatio, new Vector3(0, 1, -2)),
                new Camera(new Vector3(0, 0, 2), aspectRatio, true),
                staticCamera,
                new FollowCamera(new Vector3(-2, 2, 6), aspectRatio, ship.MovementController.Position),
                new RubberBandCamera(ship.MovementController, aspectRatio, new Vector3(0, 1, -2)),
            ], [],
            daySkybox, nightSkybox);
    }

    public void ChangeCamera()
    {
        _cameraIndex = (_cameraIndex + 1) % _cameras.Length;
    }

    public void SwitchDay()
    {
        DirLight.Color = _isDay ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0.9f, 0.9f, 0.9f);
        Fog = _isDay ? new Fog(new Vector3(0.05f, 0.05f, 0.05f), 0.95f) : new Fog(new Vector3(0.4f, 0.4f, 0.4f), 1f);
        _isDay = !_isDay;
    }
}