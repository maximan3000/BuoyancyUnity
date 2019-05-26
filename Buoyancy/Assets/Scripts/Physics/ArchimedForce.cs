using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;

namespace Buoyancy.Physics
{
    public class ArchimedForce
    {
        public static float r = 1000f;
        public static float g = 9.8f;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;

        public ArchimedForce(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
        }

        public static void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            new ArchimedForce(triangle, rb).ApplyForce();
        }

        private void ApplyForce()
        {
            var force = MakeForce();
            rb.AddForceAtPosition(force, center);
            Debug.DrawRay(center, TriangleMath.GetNormal(triangle), Color.white);
        }

        private Vector3 MakeForce()
        {
            Vector3 direction = -TriangleMath.GetNormal(triangle);
            this.center = TriangleMath.GetCenter(triangle);
            float H = WaterMath.DistanceToWater(center);
            float dS = TriangleMath.GetSquare(triangle);

            float magnitude = r * g * H * dS;
            return direction * Mathf.Abs(magnitude);
        }
    }
}

