using System.Collections.Generic;
using Buoyancy.Struct;
using UnityEngine;

namespace Buoyancy.Math
{
    class HullMath
    {
        private List<Triangle> underwaterTriangles = new List<Triangle>();
        private Vector3 H, M, L;
        private float hH, hM, hL;

        public static List<Triangle> GetUnderwaterTriangles(List<Triangle> triangles)
        {
            return new HullMath().MakeUnderWaterTriangles(triangles);
        }

        private List<Triangle> MakeUnderWaterTriangles(List<Triangle> triangles)
        {
            foreach (Triangle triangle in triangles)
            {
                CheckTriangle(triangle);
            }
            return underwaterTriangles;
        }

        private void CheckTriangle(Triangle triangle)
        {
            List<Vector3> vertices = new List<Vector3>() { triangle.A, triangle.B, triangle.C };
            vertices.Sort((p1, p2) => p2.y.CompareTo(p1.y));

            H = vertices[0]; M = vertices[1]; L = vertices[2];
            hH = WaterMath.DistanceToWater(H); hM = WaterMath.DistanceToWater(M); hL = WaterMath.DistanceToWater(L);

            if (hH <= 0 && hM <= 0 && hL <= 0)
                underwaterTriangles.Add(triangle);
            else if (hH > 0 && hM < 0 && hL < 0)
                OneVertexAboveWater();
            else if (hH > 0 && hM > 0 && hL < 0)
                TwoVerticesAboveWater();
        }

        private void OneVertexAboveWater()
        {
            float tM = -hM / (hH - hM);
            float tL = -hL / (hH - hL);
            Vector3 MH = H - M;
            Vector3 LH = H - L;
            Vector3 cutMH = M + MH * tM;
            Vector3 cutLH = L + LH * tL;

            underwaterTriangles.Add(new Triangle(M, cutMH, L));
            underwaterTriangles.Add(new Triangle(L, cutLH, cutMH));
        }

        private void TwoVerticesAboveWater()
        {
            float tM = -hL / (hM - hL);
            float tH = -hL / (hH - hL);
            Vector3 LM = M - L;
            Vector3 LH = H - L;
            Vector3 cutLM = L + LM * tM;
            Vector3 cutLH = L + LH * tH;

            underwaterTriangles.Add(new Triangle(L, cutLM, cutLH));
        }
    }
}
