using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buoyancy.Struct;

namespace Buoyancy.Math
{
    public class TriangleMath
    {
        public static Vector3 GetCenter(Triangle triangle)
        {
            var center = new Vector3(
                    (triangle.A.x + triangle.B.x + triangle.C.x) / 3,
                    (triangle.A.y + triangle.B.y + triangle.C.y) / 3,
                    (triangle.A.z + triangle.B.z + triangle.C.z) / 3
            );
            return center;
        }

        public static Vector3 GetNormal(Triangle triangle)
        {
            var normal = Vector3.Cross(triangle.B - triangle.A, triangle.C - triangle.A);
            normal.Normalize();
            return normal;
        }

        public static float GetSquare(Triangle triangle)
        {
            Vector3 AC = triangle.C - triangle.A;
            Vector3 AB = triangle.B - triangle.A;
            Vector3 cross = Vector3.Cross(AC, AB);
            return Mathf.Abs(cross.magnitude) / 2;
        }

        public static float GetLength(Triangle triangle)
        {
            Vector3 AC = triangle.C - triangle.A;
            Vector3 AB = triangle.B - triangle.A;
            Vector3 BC = triangle.C - triangle.B;
            return (Mathf.Abs(AC.magnitude) + Mathf.Abs(AB.magnitude) + Mathf.Abs(BC.magnitude)) / 3;
        }
    }
}

