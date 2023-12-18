using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneDebug
{
    public class Debug : MonoBehaviour
    {
        [SerializeField] Color color = new Color(1, 1, 1, 1);
        [SerializeField] private float radius = 0.2f;
        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);

        }
#endif
    }
}

