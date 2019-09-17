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

using System.Collections.Generic;
using Buoyancy.Struct;
using UnityEngine;

namespace Buoyancy.Math
{
    /// <summary>
    /// Algorithm that determines underwater and abovewater parts of the boat. It also cuts triangles by waterline.
    /// Read about it from Jacques Kerner's article "Water interaction model for boats in video games"
    /// </summary>
    /// <see cref="https://www.gamasutra.com/view/news/237528/Water_interaction_model_for_boats_in_video_games.php"/>
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
