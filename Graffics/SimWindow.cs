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
        _scene = Scene.AssignmentScene(_shader, Size.X / (float)Size.Y);

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

        if (KeyboardState.IsKeyPressed(Keys.P))
        {
            _scene.Camera.IsPerspective = !_scene.Camera.IsPerspective;
        }

        if (KeyboardState.IsKeyPressed(Keys.N))
        {
            _scene.SwitchDay();
        }

        if (KeyboardState.IsKeyPressed(Keys.F))
        {
            _scene.SwitchFog();
        }

        foreach (var model in _scene.Models)
        {
            model.Update(e.Time, KeyboardState);
        }
        
        
        foreach(var camera in _scene.Cameras)
        {
            camera.Update(e.Time);
        } 
        _scene.Update(e.Time);
        
        
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        if (_scene.PointLights.Length > MaxLights)
        {
            throw new Exception("Too many lights for simulation");
        }

        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var viewMat = _scene.Camera.GetViewMatrix();   
        _shader.Use();
        _shader.SetMatrix4("view", _scene.Camera.GetViewMatrix());
        _shader.SetMatrix4("projection", _scene.Camera.GetProjectionMatrix());
        _shader.SetVector3("dirLight.color", _scene.DirLight.Color);
        _shader.SetVector3("dirLight.direction", ToCameraSpaceDir(_scene.DirLight.Direction, viewMat));
        _shader.SetFloat("fogIntensity", _scene.Fog.FogIntensity);
        _shader.SetVector3("fogColor", _scene.Fog.FogColor);
        _shader.SetInt("numPointLights", _scene.PointLights.Length);

        
        for (int i = 0; i < _scene.PointLights.Length; i++)
        {
            _shader.SetVector3($"pointLights[{i}].position", ToCameraSpace(_scene.PointLights[i].Position, viewMat));
            _shader.SetVector3($"pointLights[{i}].color", _scene.PointLights[i].Color);
            _shader.SetVector3($"pointLights[{i}].attenuation", _scene.PointLights[i].AttenuationCoefficients);
        }

        _shader.SetInt("numSpotLights", _scene.SpotLights.Length);
        for (int i = 0; i < _scene.SpotLights.Length; i++)
        {
            _shader.SetVector3($"spotLights[{i}].position", ToCameraSpace(_scene.SpotLights[i].Position, viewMat));
            _shader.SetVector3($"spotLights[{i}].direction", ToCameraSpaceDir(_scene.SpotLights[i].Direction, viewMat));
            _shader.SetVector3($"spotLights[{i}].color", _scene.SpotLights[i].Color);
            _shader.SetFloat($"spotLights[{i}].concentration", _scene.SpotLights[i].Concentration);
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

    private Vector3 ToCameraSpace(Vector3 point, Matrix4 viewMat)
    {
        return (new Vector4(point, 1.0f) * viewMat).Xyz;
    }

    private Vector3 ToCameraSpaceDir(Vector3 dir, Matrix4 viewMat)
    {
        return dir * new Matrix3(viewMat);
    } 
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}