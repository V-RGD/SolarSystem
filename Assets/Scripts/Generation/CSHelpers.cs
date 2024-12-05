using System.Collections.Generic;
using UnityEngine;

namespace HPC.Helpers
{
    public static class CSHelpers
    {
        public static void SecondsToMinutesAndSeconds(float seconds, out float outputSeconds, out float outputMinutes)
        {
            outputMinutes = Mathf.Floor(seconds / 60);
            outputSeconds = Mathf.Floor(seconds % 60);
        }
        
        /// <summary>
        /// Returns a copy of the vector, ignoring the Y axis
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 IgnoreY(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        /// <summary>
        /// Checks if two vector3 are equal, with a precision of 0.0001
        /// </summary>
        public static bool Equals(this Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(a - b) < 0.0001; //tolerance
        }

        /// <summary>
        /// Checks if two vector2 are equal, with a precision of 0.0001
        /// </summary>
        public static bool Equals(this Vector2 a, Vector2 b)
        {
            return Vector2.SqrMagnitude(a - b) < 0.0001; //tolerance
        }

        /// <summary>
        /// Returns false if a coordinate of the vector is NaN or Infinity
        /// </summary>
        public static bool IsValid(this Vector3 vector)
        {
            return !(float.IsNaN(vector.x) || float.IsInfinity(vector.x) ||
                     float.IsNaN(vector.x) || float.IsInfinity(vector.x) ||
                     float.IsNaN(vector.x) || float.IsInfinity(vector.x));
        }

        public static T[] Clear<T>(this T[] array) where T : new()
        {
            T[] newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = new T();
            }
            return newArray;
        }
        
        public static T[] CopyArray<T>(this T[] array) where T : Object
        {
            T[] newArray = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            return newArray;
        }
        
        public static T[] CopyList<T>(this List<T> list) where T : Object
        {
            T[] newArray = new T[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                newArray[i] = list[i];
            }
            return newArray;
        }
    }
}