using System.Security.Cryptography.X509Certificates;
using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Mathematics;
using OpenTK.Graphics.ES30;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace Graffics;

public interface IModel3d
{
    public void Render();
    public void Update(double dt);

    public Matrix4 GetModelMatrix();
    public Material GetMaterial();

    public PointLight[] GetPointLights();
}

public class Material
{
    public Material(float ambient, float diffuse, float specular, float shininess)
    {
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Shininess = shininess;
    }

    public float Ambient;
    public float Diffuse;
    public float Specular;
    public float Shininess;
}

public class Cube : IModel3d
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

    public IMovementController MovementController
    {
        get => _movementController;
    }

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

    public void Update(double dt)
    {
        _movementController.UpdatePosition(dt);
    }

    public Matrix4 GetModelMatrix()
    {
        Vector3 f = _movementController.Position.Front.Normalized();
        Vector3 u = _movementController.Position.Up.Normalized();
        Vector3 r = Vector3.Cross(f, u).Normalized(); // Right vector
        u = Vector3.Cross(r, f).Normalized(); // Recalculate up to ensure orthogonality

        Matrix4 rotation = new Matrix4(
            new Vector4(r, 0.0f),  // Right vector
            new Vector4(u, 0.0f),  // Up vector
            new Vector4(-f, 0.0f), // Negative Front vector (because OpenGL uses right-handed coordinate system)
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
}

class PointLamp : IModel3d
{
    private PointLight _light;
    private Sphere _shade;

    public PointLamp(Vector3 color, Shader shader, IMovementController movementController)
    {
        _light = new PointLight(movementController.Position.Position, color);
        var texture = Texture.LoadFromFile("Resources/shade.jpg");
        _shade = new Sphere(0.1f, 30, 30, new Material(0.65f, 0, 0, 1), movementController, texture, shader);
    }

    public void Render()
    {
        _shade.Render();
    }

    public void Update(double dt)
    {
        _shade.Update(dt);
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
        return new[] { _light };
    }
}

struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoords;

    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords)
    {
        Position = position;
        Normal = normal;
        TexCoords = texCoords;
    }

    public float[] ToArray()
    {
        return new float[]
        {
            Position.X, Position.Y, Position.Z,
            Normal.X, Normal.Y, Normal.Z,
            TexCoords.X, TexCoords.Y
        };
    }
}

class Sphere : IModel3d
{
    private Vertex[] _vertices;
    private int[] _indices;
    private Texture _texture;
    private Material _material;
    public IMovementController MovementController { get; }
    private int _vao;
    private float _radius;

    private void GenerateVertices(float radius, int longitudeSegments, int latitudeSegments)
    {
        _radius = radius;

        var verts = new List<Vertex>();
        var inds = new List<int>();

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * MathF.PI / latitudeSegments;
            float sinTheta = MathF.Sin(theta);
            float cosTheta = MathF.Cos(theta);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2 * MathF.PI / longitudeSegments;
                float sinPhi = MathF.Sin(phi);
                float cosPhi = MathF.Cos(phi);

                // Position
                float x = cosPhi * sinTheta;
                float y = cosTheta;
                float z = sinPhi * sinTheta;

                // Normal (normalized position)
                var normal = new Vector3(x, y, z).Normalized();

                // Texture coordinates
                float u = (float)lon / longitudeSegments;
                float v = (float)lat / latitudeSegments;

                verts.Add(new Vertex(new Vector3(x, y, z), normal, new Vector2(u, v)));
            }
        }

        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int first = (lat * (longitudeSegments + 1)) + lon;
                int second = first + longitudeSegments + 1;

                inds.Add(first);
                inds.Add(second);
                inds.Add(first + 1);

                inds.Add(second);
                inds.Add(second + 1);
                inds.Add(first + 1);
            }
        }

        _vertices = verts.ToArray();
        _indices = inds.ToArray();
    }

    public Sphere(float radius, int longitudeSegments, int latitudeSegments, Material material,
        IMovementController movementController, Texture texture, Shader shader)
    {
        _texture = texture;
        _material = material;
        MovementController = movementController;
        GenerateVertices(radius, longitudeSegments, latitudeSegments);


        // Flatten vertex data to a float array
        var vertexData = new List<float>();
        foreach (var vertex in _vertices)
        {
            vertexData.AddRange(vertex.ToArray());
        }

        // Generate buffers
        _vao = GL.GenVertexArray();
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        // Bind vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Count * sizeof(float), vertexData.ToArray(),
            BufferUsageHint.StaticDraw);

        // Bind element data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices,
            BufferUsageHint.StaticDraw);

        // Define vertex layout
        int stride = sizeof(float) * 8; // 3 position + 3 normal + 2 texture
        var vertexLocation = shader.GetAttribLocation("aPosition");
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, stride, 0); // Position
        GL.EnableVertexAttribArray(vertexLocation);

        var normalLocation = shader.GetAttribLocation("aNormal");
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, stride,
            sizeof(float) * 3); // Normal
        GL.EnableVertexAttribArray(normalLocation);

        var texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, stride,
            sizeof(float) * 6); // Texture
        GL.EnableVertexAttribArray(texCoordLocation);

        GL.BindVertexArray(0);
    }

    public void Render()
    {
        _texture.Use(TextureUnit.Texture0);
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Update(double dt)
    {
        MovementController.UpdatePosition(dt);
    }

    public Matrix4 GetModelMatrix()
    {
        return Matrix4.CreateScale(_radius) * MovementController.GetPositionMatrix();
    }

    public Material GetMaterial()
    {
        return _material;
    }

    public PointLight[] GetPointLights()
    {
        return [];
    }
}