using System.Collections.Generic;
using UnityEngine;
using Buoyancy.Struct;

namespace Buoyancy.Debug
{
    public class DisplayWorker
    {
        public static void DisplayTriangles(List<Triangle> triangles)
        {
            foreach (Triangle triangle in triangles)
            {
                UnityEngine.Debug.DrawLine(triangle.A, triangle.B, Color.red);
                UnityEngine.Debug.DrawLine(triangle.B, triangle.C, Color.red);
                UnityEngine.Debug.DrawLine(triangle.A, triangle.C, Color.red);
            }
        }

        public static void DisplayForce(Vector3 point, Vector3 force)
        {
            UnityEngine.Debug.DrawRay(point, force.normalized, Color.white);
        }
    }
}
