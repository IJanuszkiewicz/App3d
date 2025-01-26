using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace Graffics;

class Sphere : IGameObject
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

    public void Update(double dt, KeyboardState keyboardState)
    {
        MovementController.UpdatePosition(dt, keyboardState);
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

    public SpotLight[] GetSpotLights()
    {
        return [];
    }
}