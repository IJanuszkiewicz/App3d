using Graffics.Movement;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graffics;

public class AdvancedModel : IGameObject
{
    protected Model _model;
    protected IMovementController _movementController;
    protected float _size;
    protected Material _material;
    protected Texture _texture;
    protected PointLight[] _pointLights;
    protected Vector3[] _pointLightRelativePositions;
    protected SpotLight[] _spotLights;
    protected (Vector3 pos, Vector3 dir)[] _spotLightRelativePositions;


    public IMovementController MovementController => _movementController;
    

    public AdvancedModel(String path, IMovementController movementController, float size, Material material,
        Texture texture, PointLight[] pointLights, Vector3[] pointLightRelativePositions, SpotLight[] spotLights,
        (Vector3 pos, Vector3 dir)[] spotLightRelativePositions)
    {
        _model = new Model(path);
        _movementController = movementController;
        _size = size;
        _pointLights = pointLights;
        _pointLightRelativePositions = pointLightRelativePositions;
        _material = material;
        _texture = texture;
        _spotLights = spotLights;
        _spotLightRelativePositions = spotLightRelativePositions;
    }

    public void Render()
    {
        _texture.Use(TextureUnit.Texture0);
        _model.Draw();
    }

    public virtual void Update(double dt, KeyboardState keyboard)
    {
        _movementController.UpdatePosition(dt, keyboard);
        var modelMat = _movementController.GetPositionMatrix();
        var littleModelMat = new Matrix3(modelMat);
        for (int i = 0; i < _spotLights.Length; i++)
        {
            _spotLights[i].Position = (new Vector4(_spotLightRelativePositions[i].pos,1.0f) * modelMat).Xyz;
            _spotLights[i].Direction = _spotLightRelativePositions[i].dir * littleModelMat;
        }

        for (int i = 0; i < _pointLights.Length; i++)
        {
            _pointLights[i].Position = (new Vector4(_pointLightRelativePositions[i],1.0f) * modelMat).Xyz;
        }
    }

    public Matrix4 GetModelMatrix()
    {
        return Matrix4.CreateScale(_size) * _movementController.GetPositionMatrix();
    }

    public Material GetMaterial()
    {
        return _material;
    }

    public PointLight[] GetPointLights()
    {
        return _pointLights;
    }

    public SpotLight[] GetSpotLights()
    {
        return _spotLights;
    }
}