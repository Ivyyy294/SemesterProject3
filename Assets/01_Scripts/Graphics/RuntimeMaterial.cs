using UnityEngine;
using UnityEngine.UI;

public class RuntimeMaterial
{
    private Material _original;
    private Material _runtimeMat;

    public Material Mat => _runtimeMat;
    
    public RuntimeMaterial(Material original)
    {
        _original = original;
        _runtimeMat = new Material(_original);
        _runtimeMat.name = $"{_original.name}_Runtime";
    }

    public RuntimeMaterial(Shader shader)
    {
        _original = null;
        _runtimeMat = new Material(shader);
        _runtimeMat.name = $"{shader.name}_Runtime";
    }

    public Material Release()
    {
        Object.Destroy(_runtimeMat);
        return _original;
    }

    public static RuntimeMaterial FromImage(Image image)
    {
        var m = new RuntimeMaterial(image.material);
        image.material = m.Mat;
        return m;
    }
}
