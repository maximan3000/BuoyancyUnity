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
using Buoyancy.Struct;

namespace Buoyancy.Math
{
    /// <summary>
    /// Lib with all calculations for <c>Triangle</c> struct
    /// </summary>
    public static class TriangleMath
    {
        public static Vector3 GetCenter(Triangle triangle)
        {
            var center = new Vector3(
                    (triangle.A.x + triangle.B.x + triangle.C.x) / 3f,
                    (triangle.A.y + triangle.B.y + triangle.C.y) / 3f,
                    (triangle.A.z + triangle.B.z + triangle.C.z) / 3f
            );
            return center;
        }

        public static Vector3 GetNormal(Triangle triangle)
        {
            var plane = new Plane(triangle.A, triangle.B, triangle.C);
            return plane.normal;
        }

        public static float GetSquare(Triangle triangle)
        {
            Vector3 AC = triangle.C - triangle.A;
            Vector3 AB = triangle.B - triangle.A;
            Vector3 cross = Vector3.Cross(AC, AB);
            float square = Mathf.Abs(cross.magnitude) / 2f;
            return square;
        }
    }
}

