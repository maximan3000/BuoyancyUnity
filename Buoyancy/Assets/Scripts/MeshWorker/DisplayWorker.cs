using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Buoyancy.Struct;

namespace Buoyancy.MeshWorker
{
    public class DisplayWorker
    {
        public static void DisplayTriangles(Mesh mesh, Transform transform, List<Triangle> triangles)
        {
            List<Vector3> meshVertices = new List<Vector3>();
            List<int> meshTriangles = new List<int>();

            int index = 0;
            foreach (Triangle triangle in triangles)
            {
                Vector3 A = transform.InverseTransformPoint(triangle.A);
                Vector3 B = transform.InverseTransformPoint(triangle.B);
                Vector3 C = transform.InverseTransformPoint(triangle.C);

                meshVertices.Add(A); meshTriangles.Add(index++);
                meshVertices.Add(B); meshTriangles.Add(index++);
                meshVertices.Add(C); meshTriangles.Add(index++);
            }

            mesh.Clear();
            mesh.vertices = meshVertices.ToArray();
            mesh.triangles = meshTriangles.ToArray();
            mesh.RecalculateBounds();
        }
    }
}
