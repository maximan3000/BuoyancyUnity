using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;

namespace Buoyancy.Physics
{
    public class ArchimedForce
    {
        public static float density = 1000f;
        public static float gravity = 9.8f;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float height;

        public ArchimedForce(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
            this.center = TriangleMath.GetCenter(triangle);
            this.height = WaterMath.DistanceToWater(center);
        }

        public static void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            new ArchimedForce(triangle, rb).ApplyForce();
        }

        private void ApplyForce()
        {
            var force = MakeForce();
            rb.AddForceAtPosition(force, center);
            //Debug.DrawRay(center, force.normalized, Color.white);
        }

        private Vector3 MakeForce()
        {
            Vector3 direction = -TriangleMath.GetNormal(triangle);
            float square = TriangleMath.GetSquare(triangle);

            float magnitude = density * gravity * height * square;
            //direction = Vector3.Project(direction, Vector3.up);
            return direction * Mathf.Abs(magnitude);
        }
    }
}

