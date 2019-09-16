using UnityEngine;
using System.Collections.Generic;
using Buoyancy.Struct;

namespace Buoyancy.MeshWorker
{
    /// <summary>
    /// Converts between structs of triangles
    /// </summary>
    public class TriangleParser
    {
        /// <summary>
        /// Converts Unity triangles struct <c>Mesh.vertices</c>, <c>Mesh.triangles</c> into 
        /// comfortable list of <c>Buoyancy.Struct.Triangle</c>
        /// </summary>
        /// <param name="mesh">Mesh of the target gameobject (boat, etc)</param>
        /// <param name="transform">If there is need to shift position</param>
        /// <returns></returns>
        public static List<Triangle> parse(Mesh mesh, Transform transform = null)
        {
            var triangles = new List<Triangle>();

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Vector3 A = mesh.vertices[mesh.triangles[i]],
                    B = mesh.vertices[mesh.triangles[i + 1]],
                    C = mesh.vertices[mesh.triangles[i + 2]];

                if (transform != null)
                {
                    A = transform.TransformPoint(A);
                    B = transform.TransformPoint(B);
                    C = transform.TransformPoint(C);
                }

                triangles.Add(new Triangle(A,B,C));
            }

            return triangles;
        }
    }
}
