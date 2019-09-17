/* 
Copyright 2019 Maksim Petrov <grayen.job@gmail.com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
