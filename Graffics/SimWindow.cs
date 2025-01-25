using Graffics.Movement;
using Graffics.shaders;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics;

public class SimWindow : GameWindow
{
    private const int MaxLights = 20;
    Shader _shader;
    Scene _scene;

    public SimWindow(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title,
        API = ContextAPI.OpenGL,
        Profile = ContextProfile.Core,
        DepthBits = 24
    })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
        GL.Enable(EnableCap.DepthTest);
        _shader = new Shader("shaders/shader.vert", "shaders/shader.frag");
        _scene = Scene.ShapesInSpace(_shader, Size.X / (float)Size.Y);

        _shader.Use();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _shader.Dispose();
    }

    private bool _firstMove = true;
    private Vector2 _lastPos;

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        const float cameraSpeed = 1.5f;
        const float sensitivity = 0.2f;
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        if (_scene.Camera.IsControllable)
        {
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                _scene.Camera.Position += _scene.Camera.Front * cameraSpeed * (float)e.Time;
            }

            if (KeyboardState.IsKeyDown(Keys.S))
            {
                _scene.Camera.Position -= _scene.Camera.Front * cameraSpeed * (float)e.Time;
            }

            if (KeyboardState.IsKeyDown(Keys.D))
            {
                _scene.Camera.Position += _scene.Camera.Right * cameraSpeed * (float)e.Time;
            }

            if (KeyboardState.IsKeyDown(Keys.A))
            {
                _scene.Camera.Position -= _scene.Camera.Right * cameraSpeed * (float)e.Time;
            }

            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                _scene.Camera.Position += _scene.Camera.Up * cameraSpeed * (float)e.Time;
            }

            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                _scene.Camera.Position -= _scene.Camera.Up * cameraSpeed * (float)e.Time;
            }

            var mouse = MouseState;
            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _scene.Camera.Yaw += deltaX * sensitivity;
                _scene.Camera.Pitch -= deltaY * sensitivity;
            }
        }

        if (KeyboardState.IsKeyPressed(Keys.C))
        {
            _scene.ChangeCamera();
        }

        foreach (var model in _scene.Models)
        {
            model.Update(e.Time);
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        if (_scene.PointLights.Length > MaxLights)
        {
            throw new Exception("Too many lights for simulation");
        }

        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();
        _shader.SetMatrix4("view", _scene.Camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _scene.Camera.GetProjectionMatrix());
        _shader.SetVector3("viewPos", _scene.Camera.Position);
        _shader.SetInt("numPointLights", _scene.PointLights.Length);


        for (int i = 0; i < _scene.PointLights.Length; i++)
        {
            _shader.SetVector3($"pointLights[{i}].position", _scene.PointLights[i].Position);
            _shader.SetVector3($"pointLights[{i}].color", _scene.PointLights[i].Color);
        }

        foreach (var model in _scene.Models)
        {
            var material = model.GetMaterial();
            _shader.SetFloat("material.diffuse", material.Diffuse);
            _shader.SetFloat("material.ambient", material.Ambient);
            _shader.SetFloat("material.specular", material.Specular);
            _shader.SetFloat("material.shiny", material.Shininess);
            _shader.SetMatrix4("model", model.GetModelMatrix());
            model.Render();
        }

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}