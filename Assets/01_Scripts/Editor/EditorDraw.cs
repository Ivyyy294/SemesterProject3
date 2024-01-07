using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class EditorDraw : MonoBehaviour
{
    public static void DrawWireCube(Vector3 origin, Quaternion rot, Vector3 scale, float thickness, Vector3? localAnchor = null)
    {

        if (localAnchor == null)
        {
            localAnchor =new Vector3(.5f, .5f, .5f);
        }
        var anchor = (Vector3)localAnchor;
        
        Vector3[] points = new Vector3[8];
        points[0] = new Vector3(0, 0, 0);
        points[1] = new Vector3(1, 0, 0);
        points[2] = new Vector3(1, 0, 1);
        points[3] = new Vector3(0, 0, 1);
        
        points[4] = new Vector3(0, 1, 0);
        points[5] = new Vector3(1, 1, 0);
        points[6] = new Vector3(1, 1, 1);
        points[7] = new Vector3(0, 1, 1);

        for (int i = 0; i < 8; i++)
        {
            var p = points[i] - anchor;
            p.Scale(scale);
            points[i] = rot * p + origin;
        }
        
    
        Handles.DrawLine(points[0], points[1], thickness);
        Handles.DrawLine(points[1], points[2], thickness);
        Handles.DrawLine(points[2], points[3], thickness);
        Handles.DrawLine(points[3], points[0], thickness);
    
        Handles.DrawLine(points[0], points[4], thickness);
        Handles.DrawLine(points[1], points[5], thickness);
        Handles.DrawLine(points[2], points[6], thickness);
        Handles.DrawLine(points[3], points[7], thickness);
        
        Handles.DrawLine(points[4], points[5], thickness);
        Handles.DrawLine(points[5], points[6], thickness);
        Handles.DrawLine(points[6], points[7], thickness);
        Handles.DrawLine(points[7], points[4], thickness);
    }
}

#endif