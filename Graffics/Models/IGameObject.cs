using System.Security.Cryptography.X509Certificates;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics;

public interface IGameObject
{
    public void Render();
    public void Update(double dt, KeyboardState keyboard);

    public Matrix4 GetModelMatrix();
    public Material GetMaterial();

    public PointLight[] GetPointLights();
    public SpotLight[] GetSpotLights();
}