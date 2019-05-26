using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Buoyancy.Struct;

namespace Buoyancy.MeshWorker
{
    class TriangleParser
    {
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
