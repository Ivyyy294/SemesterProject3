using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class MultiInverseChain : MonoBehaviour
{
    public List<Transform> originalChain;

    private List<Transform> _inverseChain;
    private List<Transform> _originalChainCopy;

    public Transform GetOriginalCopy(int i) => _originalChainCopy[i];
    public Transform GetInverse(int i) => _inverseChain[i];
    
    public int InvertIndex(int i) => ChainLength - (i + 1);
    // The inverse chain is in "reverse" order from the original chain
    // with its first entry being the root of the inverse chain
    // Getting the transform that corresponds with the same bone in the original chain by index
    // therefore requires sampling the inverse chain in reverse order
    public Transform GetInverseReverseOrder(int i) => _inverseChain[InvertIndex(i)];
    public Transform GetOriginalReverseOrder(int i) => originalChain[InvertIndex(i)];
    
    private Quaternion _originalEndRotation;
    public Quaternion OriginalEndRotation => _originalEndRotation;
    
    private bool _isSetup = false;

    public int ChainLength => originalChain.Count;

    public static void Copy(Transform from, Transform to)
    {
        to.position = from.position;
        to.rotation = from.rotation;
    }
    
    #region MAKE / DESTROY
    public void OnEnable()
    {
        _inverseChain = new();
        _originalChainCopy = new();
        for (int i = 0; i < ChainLength; i++)
        {
            var newInverse = new GameObject($"InverseChain: {i}").transform;
            newInverse.hideFlags = HideFlags.HideInHierarchy;
            var newParent = i == 0 ? transform : _inverseChain.Last();

            newInverse.parent = newParent;
            _inverseChain.Add(newInverse);
            
            var newOriCopy = new GameObject($"OriCopy: {i}").transform;
            newOriCopy.hideFlags = HideFlags.HideInHierarchy;
            newOriCopy.parent = newInverse;
            _originalChainCopy.Add(newOriCopy);
        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < ChainLength; i++)
        {
            Destroy(_inverseChain[i].gameObject);
        }
        _inverseChain.Clear();
    }
    #endregion

    public void GetOriginal()
    {
        for (int i = 0; i < ChainLength; i++)
        {
            var inverseBone = GetInverse(i);
            var originalBone = GetOriginalReverseOrder(i);
            Copy(originalBone, inverseBone);
        }

        for (int i = 0; i < ChainLength - 1; i++)
        {
            var oriCopyBone = _originalChainCopy[i];
            var originalBone = GetOriginalReverseOrder(i + 1);
            oriCopyBone.position = originalBone.position;
            oriCopyBone.rotation = originalBone.rotation;
        }

        _originalEndRotation = originalChain.Last().rotation;
        
        _isSetup = true;
    }

    public void Apply()
    {
        if (!_isSetup)
        {
            #if UNITY_EDITOR
            Debug.LogError($"Inverse Chain {this} not set up before attempting to apply");
            #endif
            return;
        }

        Copy(GetOriginalCopy(InvertIndex(1)), originalChain[0]);
        for (int i = 1; i < ChainLength - 1; i++)
        {
            var originalBone = originalChain[i];
            var oriCopyBone = _originalChainCopy[InvertIndex(i + 1)];
            originalBone.rotation = oriCopyBone.rotation;
        }

        originalChain.Last().rotation = _originalEndRotation;
        
        _isSetup = false;
    }

    public void DrawGizmos(float radius = 0.1f)
    {
        try
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(originalChain[0].position, radius);
            for (int i = 1; i < ChainLength; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(originalChain[i].position, radius);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(originalChain[i].position, originalChain[i - 1].position);
            }
        }
        catch {}
        
        try
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_inverseChain[0].position, radius);
            for (int i = 1; i < ChainLength; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_inverseChain[i].position, radius);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(_inverseChain[i].position, _inverseChain[i - 1].position);
            }
        }
        catch {}
    }
}