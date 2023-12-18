using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimUtils
{
    public class AngleTracker
    {
        private Vector3 _origin;
        public Vector3 Origin => _origin;
        
        private Vector3 _target;
        private Vector3 _forward;
        private Vector3 _normal1;
        private Vector3 _normal2;
        
        public float Angle1 => GetAngleOnProjectedPlane(_origin, _target, _normal1, _forward);
        public float Angle2 => GetAngleOnProjectedPlane(_origin, _target, _normal2, _forward);
        public Quaternion Rotation1 => Quaternion.AngleAxis(Angle1, _normal1);
        public Quaternion Rotation2 => Quaternion.AngleAxis(Angle2, _normal2);

        public AngleTracker(Vector3 origin, Vector3 target, Vector3 forward, Vector3 normal1, Vector3 normal2)
        {
            _origin = origin;
            _target = target;
            _forward = forward;
            _normal1 = normal1;
            _normal2 = normal2;
        }
        
        public static float GetAngleOnProjectedPlane(Vector3 origin, Vector3 target, Vector3 normal, Vector3 forward)
        {
            var projectedDelta = GetProjectedPoint(origin, target, normal) - origin;
            var forwardDot = Vector3.Dot(projectedDelta, forward);
            var upDot = Vector3.Dot(projectedDelta, Vector3.Cross(forward, normal));
            return -Mathf.Atan2(upDot, forwardDot) * Mathf.Rad2Deg;
        }

        public static Vector3 GetProjectedPoint(Vector3 origin, Vector3 target, Vector3 normal)
        {
            var delta = target - origin;
            var normalDot = Vector3.Dot(delta, normal);
            return target - normal * normalDot;
        }
        
        #if UNITY_EDITOR
        public void DrawDebug()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Origin, Origin + _normal1);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Origin, Origin + _normal2);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Origin, Origin + _forward);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Origin, Origin + Rotation1 * _forward);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(Origin, Origin + Rotation2 * _forward);
        }
        #endif
    }
}
