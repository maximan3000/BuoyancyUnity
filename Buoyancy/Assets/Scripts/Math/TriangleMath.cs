﻿using UnityEngine;
using Buoyancy.Struct;

namespace Buoyancy.Math
{
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

