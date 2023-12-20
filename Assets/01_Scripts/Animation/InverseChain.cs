using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InverseChain : MonoBehaviour
{
    public Transform root;
    public Transform child;

    private Transform _rootInverse;
    private Transform _childInverse;
    private Quaternion _childOriginalRotation;

    public Transform RootInverse => _rootInverse;
    public Transform ChildInverse => _childInverse;
    
    public static void Copy(Transform from, Transform to)
    {
        to.position = from.position;
        to.rotation = from.rotation;
    }
    public void OnEnable()
    {
        _rootInverse = new GameObject("InverseChain: InverseRoot").transform;
        _childInverse = new GameObject("InverseChain: InverseChild").transform;
        _childInverse.SetParent(_rootInverse);
        _rootInverse.SetParent(transform);
    }

    public void OnDisable()
    {
        Destroy(_rootInverse.gameObject);
        Destroy(_childInverse.gameObject);
    }

    public void GetOriginal()
    {
        Copy(child, _rootInverse);
        Copy(root, _childInverse);
        _childOriginalRotation = child.rotation;
    }

    public void Apply(bool restoreChildRotation = true)
    {
        Copy(_childInverse, root);
        Copy(_rootInverse, child);
        if(restoreChildRotation)
            child.rotation = _childOriginalRotation;
    }
}