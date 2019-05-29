using System.Collections.Generic;
using Buoyancy.Struct;
using UnityEngine;

namespace Buoyancy.Math
{
    public class HullMath
    {
        private HullTriangles hullTriangles = new HullTriangles();
        private Vector3 H, M, L;
        private float hH, hM, hL;

        public static HullTriangles CutHullAtWaterline(List<Triangle> triangles)
        {
            return new HullMath().CutHull(triangles);
        }

        private HullTriangles CutHull(List<Triangle> triangles)
        {
            foreach (Triangle triangle in triangles)
            {
                ProcessTriangle(triangle);
            }
            return hullTriangles;
        }

        private void ProcessTriangle(Triangle triangle)
        {
            var vertices = new List<Vector3>() { triangle.A, triangle.B, triangle.C };
            vertices.Sort((p1, p2) => p2.y.CompareTo(p1.y));

            H = vertices[0]; M = vertices[1]; L = vertices[2];
            hH = WaterMath.DistanceToWater(H); hM = WaterMath.DistanceToWater(M); hL = WaterMath.DistanceToWater(L);

            if (hH <= 0f && hM <= 0f && hL <= 0f)
            {
                hullTriangles.underwater.Add(triangle);
            }
            else if (hH > 0f && hM < 0f && hL < 0f)
            {
                OneVertexAboveWater();
            }
            else if (hH > 0f && hM > 0f && hL < 0f)
            {
                TwoVerticesAboveWater();
            }
            else
            {
                hullTriangles.abovewater.Add(triangle);
            }
        }

        private void OneVertexAboveWater()
        {
            float tM = -hM / (hH - hM);
            float tL = -hL / (hH - hL);
            Vector3 MH = H - M;
            Vector3 LH = H - L;
            Vector3 cutMH = M + MH * tM;
            Vector3 cutLH = L + LH * tL;

            hullTriangles.underwater.Add(new Triangle(M, cutMH, L));
            hullTriangles.underwater.Add(new Triangle(L, cutLH, cutMH));
            hullTriangles.abovewater.Add(new Triangle(H, cutMH, cutLH));
        }

        private void TwoVerticesAboveWater()
        {
            float tM = -hL / (hM - hL);
            float tH = -hL / (hH - hL);
            Vector3 LM = M - L;
            Vector3 LH = H - L;
            Vector3 cutLM = L + LM * tM;
            Vector3 cutLH = L + LH * tH;

            hullTriangles.underwater.Add(new Triangle(L, cutLM, cutLH));
            hullTriangles.abovewater.Add(new Triangle(H, cutLH, cutLM));
            hullTriangles.abovewater.Add(new Triangle(H, M, cutLM));
        }
    }
}
