using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HPC.Helpers.Unity
{
    public static class UnityHelpers
    {
        /// <summary>
        /// Shorthand for Quaternion.Euler(0,rotation,0)
        /// </summary>
        public static Quaternion EulerY(this float rotation)
        {
            return Quaternion.Euler(0, rotation, 0);
        }

        /// <summary>
        /// Increases timer if t is inferior than max
        /// </summary>
        public static void IncreaseTimer(this ref float t, float speed = 1, float max = 1)
        {
            if (t < max) t += Time.deltaTime * speed;
            if (t >= max) t = max;
        }
        
        /// <summary>
        /// Increases timer if t is inferior than max
        /// </summary>
        public static float IncreaseTimerValue(this float t, float speed = 1, float max = 1)
        {
            if (t < max) t += Time.deltaTime * speed;
            if (t >= max) t = max;
            return t;
        }

        /// <summary>
        /// Decreases a timer if t > min value
        /// </summary>
        public static void DecreaseTimer(this ref float f, float speed = 1, float min = 0)
        {
            if (f > min) f -= Time.deltaTime * speed;
            if (f < min) f = min;
        }
        
        /// <summary>
        /// Decreases a timer if t > min value
        /// </summary>
        public static float DecreaseTimerValue(this float f, float speed = 1, float min = 0)
        {
            if (f > min) f -= Time.deltaTime * speed;
            if (f < min) f = min;
            return f;
        }

        /// <summary>
        /// Resets the values of a transform
        /// </summary>
        public static void ResetTransformation(this Transform tr)
        {
            tr.position = Vector3.zero;
            tr.localRotation = Quaternion.identity;
            tr.localScale = new Vector3();
        }

        /// <summary>
        /// Rotates a point depending on an axis and a pivot
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pivot"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        /// <summary>
        /// Rotates a vector by the specified angle
        /// </summary>
        public static Vector3 Rotate3D(this Vector3 original, Vector3 rotation)
        {
            return (Quaternion.AngleAxis(rotation.x, Vector3.right) * Quaternion.AngleAxis(rotation.y, Vector3.up) * Quaternion.AngleAxis(rotation.z, Vector3.forward) * original);
        }

        /// <summary>
        /// Rotates a vector by the specified angle
        /// </summary>
        public static Vector3 RotateOnY(this Vector3 original, float rotation)
        {
            return (Quaternion.AngleAxis(rotation, Vector3.up) * original);
        }

        /// <summary>
        /// Checks if a renderer is visible from the camera specified (UNTESTED)
        /// </summary>
        public static bool InVisibleFrom(this Renderer renderer, Camera camera)
        {
            if (!renderer) return false;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
        //
        // public static Transform ClosestFrom(this List<Transform> list, Transform transform)
        // {
        //     return list.OrderBy(pos => pos. 
        // }

        // /// <summary>
        // /// Checks if a layer
        // /// </summary>
        // public static bool IsInLayer(int layer, LayerMask layerMask)
        // {
        //     int objectLayerMask = 1 << layer;
        //     return (layerMask.value & objectLayerMask) > 0;
        // }
    }
}
