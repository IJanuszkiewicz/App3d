using Graffics.shaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.ES30;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace Graffics;

public class Floor : IGameObject
{
    private Texture _texture;
    private Material _material;
    private int _vertexArrayObject;
    private int _vertexBufferObject;
    private Vertex[] _vertices;
    private const int Rows = 100;
    private const int Cols = 100;
    private const float Spacing = 0.4f;
    private readonly float _offset = Spacing * Rows / 2;
    private int[] _indices;

    private void GenerateVertices()
    {
        var vertices = new List<Vertex>();
        var indices = new List<int>();

        // Generate vertices
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                float x = j * Spacing - _offset;
                float y = RandomBump(); // Create random bumps (or replace with Perlin noise)
                float z = i * Spacing - _offset;

                vertices.Add(new Vertex(new Vector3(x, y, z), Vector3.UnitY,
                    new Vector2((float)j / Cols, (float)i / Rows)));
            }
        }

        // Generate indices for the grid
        for (int i = 0; i < Rows - 1; i++)
        {
            for (int j = 0; j < Cols - 1; j++)
            {
                int topLeft = i * Cols + j;
                int topRight = topLeft + 1;
                int bottomLeft = (i + 1) * Cols + j;
                int bottomRight = bottomLeft + 1;

                // Two triangles per quad
                indices.Add(topLeft);
                indices.Add(bottomLeft);
                indices.Add(topRight);

                indices.Add(topRight);
                indices.Add(bottomLeft);
                indices.Add(bottomRight);
            }
        }

        _vertices = vertices.ToArray();
        _indices = indices.ToArray();
    }

    private float RandomBump()
    {
        Random random = new Random();
        return (float)(random.NextDouble() - 0.5) * 0.1f; // Small random offset for bumps
    }

    public Floor(Texture texture, Material material, Shader shader)
    {
        _texture = texture;
        _material = material;
        GenerateVertices();

        // Flatten vertex data to a float array
        var vertexData = new List<float>();
        foreach (var vertex in _vertices)
        {
            vertexData.AddRange(vertex.ToArray());
        }

        // Generate buffers
        _vertexArrayObject = GL.GenVertexArray();
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();

        GL.BindVertexArray(_vertexArrayObject);

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
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Update(double dt, KeyboardState keyboard)
    {
    }

    public Matrix4 GetModelMatrix()
    {
        return Matrix4.CreateScale(2);
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