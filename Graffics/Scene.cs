using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Graffics;

public class Fog
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
    public Vector3 BackgroundColor => _isDay ? _dayColor : _nightColor;
    private Vector3 _nightColor;
    private Vector3 _dayColor;
    public IGameObject[] Models => _gameObjects;
    private IGameObject[] _gameObjects;
    public PointLight[] PointLights { get; }
    public SpotLight[] SpotLights { get; }
    public DirLight DirLight { get; }
    private Camera[] _cameras;
    private int _cameraIndex;
    public Fog Fog { get; private set; }
    private const float MaxFogIntensity = 0.95f;
    public Camera Camera => _cameras[_cameraIndex];
    public Camera[] Cameras => _cameras;

    private bool _isDay = false;
    private bool _isFog = false;

    private Scene(IGameObject[] models, PointLight[] lights, Camera[] cameras, SpotLight[] spotLights,
        Vector3 nightColor, Vector3 dayColor)
    {
        _nightColor = nightColor;
        _dayColor = dayColor;
        _cameras = cameras;
        _cameraIndex = 0;
        _gameObjects = models;
        DirLight = new DirLight(new Vector3(0.1f, 0.1f, 0.1f), -Vector3.UnitY);

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
        Fog = new Fog(Vector3.One, 0f);
        SwitchDay();
    }

    public static Scene AssignmentScene(Shader shader, float aspectRatio)
    {
        var sandTexture = Texture.LoadFromFile("Resources/sand_texture.jpeg");

        var floor = new Floor(sandTexture, new Material(0.1f, .8f, 0.1f, 40), shader);

        var crateTexture = Texture.LoadFromFile("Resources/container.png");
        var bigCrate = Cube.MakeStaticCube(1, crateTexture, shader, new Material(0.1f, 0.8f, 0.6f, 40),
            new FullPosition(new Vector3(0, 0.5f, -1)));
        var lamp = new PointLamp(new Vector3(1, 1, 1), shader,
            new NoMovementController(new FullPosition(new Vector3(1, 1, 0))));
        var pointLamp = new SpotLamp(Vector3.One, 30,
            new NoMovementController(new FullPosition(new Vector3(-1.5f, 0.5f, -1), Vector3.UnitX, Vector3.UnitY)),
            shader);
        var ship = new SpaceShip(new CirclesMovementController(7, new Vector3(0, 4, -4), -1));
        var car = new SpaceShip(new KeyboardMovementController(new FullPosition(new Vector3(1, 0.5f, 0)), 5, 0.6));
        var metalicTexture = Texture.LoadFromFile("Resources/Shuttle/metal.jpeg");
        var ball = new Sphere(1, 50, 50, new Material(0.1f, 0.3f, 1f, 10),
            new NoMovementController(new FullPosition(new Vector3(-1, 1, -4))), metalicTexture, shader);
        
        var shinyBox = Cube.MakeSpinning(1, crateTexture, shader,new Material(0.1f, 0.8f, 0.9f, 40), 
            new FullPosition(new Vector3(2, 0.5f, -1)));
        var shinyBoxReflector = new SpotLamp(new Vector3(1, 1, 1), 10, 
            new SpinMovementController(1f, Vector3.UnitY, new FullPosition(new Vector3(1, 0.5f, -2), new Vector3(1,0,1).Normalized(), Vector3.UnitY)), shader);

        return new Scene([
            floor, bigCrate, lamp, pointLamp, ship, ball, shinyBox,
            new PointLamp(Vector3.One, shader,
                new NoMovementController(new FullPosition(new Vector3(-3, 1, -0.5f)))),
            new PointLamp(Vector3.One, shader,
                new NoMovementController(new FullPosition(new Vector3(-3, 2, 1.5f)))),
            new PointLamp(Vector3.One, shader,
                new NoMovementController(new FullPosition(new Vector3(0, 1, -3)))),
            car, shinyBoxReflector,
        ], [], [
            new Camera(Vector3.UnitY, aspectRatio, true),
            new RubberBandCamera(car.MovementController, aspectRatio, new Vector3(0, 1, -2)),
            new FollowCamera(new Vector3(-2, 2, 6), aspectRatio, ship.MovementController.Position),
            new Camera(new Vector3(-1, 0.8f, 4), aspectRatio, false)
        ], [], new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0.7f, 0.9f, 1f));
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

        return new Scene(models, [], [
            new RubberBandCamera(shipChaotic.MovementController, aspectRatio, new Vector3(0, 1, -2)),
            new Camera(new Vector3(0, 0, 2), aspectRatio, true),
            staticCamera,
            new FollowCamera(new Vector3(-2, 2, 6), aspectRatio, ship.MovementController.Position),
            new RubberBandCamera(ship.MovementController, aspectRatio, new Vector3(0, 1, -2)),
        ], [], new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.7f, 1f, 1f));
    }

    public void ChangeCamera()
    {
        _cameraIndex = (_cameraIndex + 1) % _cameras.Length;
    }

    public void SwitchDay()
    {
        DirLight.Color = _isDay ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0.9f, 0.9f, 0.9f);
        Fog = _isDay ? new Fog(_nightColor, Fog.FogIntensity) : new Fog(_dayColor, Fog.FogIntensity);
        if (_isDay)
        {
            GL.ClearColor(_nightColor.X, _nightColor.Y, _nightColor.Z, 1);
        }
        else
        {
            GL.ClearColor(_dayColor.X, _dayColor.Y, _dayColor.Z, 1);
        }

        _isDay = !_isDay;
    }

    public void SwitchFog()
    {
        _isFog = !_isFog;
    }

    public void Update(double dt)
    {
        if (_isFog)
        {
            Fog.FogIntensity += (MaxFogIntensity - Fog.FogIntensity)*(float)dt ;
        }
        else
        {
            Fog.FogIntensity -= Fog.FogIntensity*(float)dt;
        }
    }
}