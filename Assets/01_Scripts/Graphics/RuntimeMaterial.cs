using UnityEngine;

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

    public Material Release()
    {
        Object.Destroy(_runtimeMat);
        return _original;
    }
}
