using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace Graffics;

public class Cube : IGameObject
{
    public static Cube MakeGoingInCircles(float radius, Vector3 center, float size, Texture texture, Shader shader,
        Material material)
    {
        return new Cube(size, texture, shader, material, new CirclesMovementController(radius, center, 0.5f));
    }

    public static Cube MakeSkybox(string imagePath, Shader shader)
    {
        var texture = Texture.LoadFromFile(imagePath);
        return Cube.MakeStaticCube(1000.0f, texture, shader, new Material(1, 0, 0, 1), FullPosition.Zero());
    }

    public static Cube MakeStaticCube(float size, Texture texture, Shader shader, Material material,
        FullPosition position)
    {
        return new Cube(size, texture, shader, material, new NoMovementController(position));
    }

    public static Cube MakeSpinning(float size, Texture texture, Shader shader, Material material,
        FullPosition position)
    {
        return new Cube(size, texture, shader, material,
            new SpinMovementController(1, new Vector3(0, 1, 0), position));
    }

    public Cube(float size, Texture texture, Shader shader, Material material, IMovementController movementController)
    {
        _size = size;
        _texture = texture;
        _material = material;
        _movementController = movementController;

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        var vertexLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);


        var normalLocation = shader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float),
            3 * sizeof(float));

        var texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float),
            6 * sizeof(float));
    }

    private float _size;
    private Texture _texture;
    private Material _material;
    private IMovementController _movementController;

    public IMovementController MovementController => _movementController;

    private float[] _vertices = new[]
    {
        // Positions          Normals              Texture coords
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f
    };

    private int _vertexBufferObject;
    private int _vertexArrayObject;

    public void Render()
    {
        _texture.Use(TextureUnit.Texture0);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length / 8);
    }

    public void Update(double dt, KeyboardState keyboard)
    {
        _movementController.UpdatePosition(dt, keyboard);
    }

    public Matrix4 GetModelMatrix()
    {
        Vector3 f = _movementController.Position.Front.Normalized();
        Vector3 u = _movementController.Position.Up.Normalized();
        Vector3 r = Vector3.Cross(f, u).Normalized(); // Right vector
        u = Vector3.Cross(r, f).Normalized(); // Recalculate up to ensure orthogonality

        Matrix4 rotation = new Matrix4(
            new Vector4(r, 0.0f),
            new Vector4(u, 0.0f),
            new Vector4(-f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
        );

        Matrix4 translation = Matrix4.CreateTranslation(_movementController.Position.Position);

        return Matrix4.CreateScale(_size) * rotation * translation;
    }

    public Material GetMaterial()
    {
        return _material;
    }

    public PointLight[] GetPointLights()
    {
        return [];
    }

    public SpotLight[] GetSpotLights()
    {
        return [];
    }
}