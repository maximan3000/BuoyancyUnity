using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;

namespace Buoyancy.Physics
{
    class ResistanceForces
    {
        public static float forceMultiply = 100f;
        public static float density = 1000f;
        public static float viscosity = 0.0014f;

        private static float lastSpeed;
        private static float resistanceCoefficient;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float speed;

        public ResistanceForces(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
            this.center = TriangleMath.GetCenter(triangle);
            this.speed = Mathf.Abs(rb.velocity.magnitude);
            if (lastSpeed != speed)
            {
                lastSpeed = speed;
                RecalculateResistanceCoefficient();
            }
        }

        private void RecalculateResistanceCoefficient()
        {
            float length = TriangleMath.GetLength(triangle);
            float reynoldsCoefficient = (density * speed * length) / viscosity;

            float temp = Mathf.Log10(reynoldsCoefficient) - 2;
            float resistance = 0.075f / (temp * temp);
            resistanceCoefficient = Mathf.Abs(resistance);
        }

        public static void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            new ResistanceForces(triangle, rb).ApplyForce();
        }

        private void ApplyForce()
        {
            var force = MakeForce();
            rb.AddForceAtPosition(force, center);
            //Debug.DrawRay(center, force.normalized, Color.white);
        }

        private Vector3 MakeForce()
        {
            var velocity = rb.velocity;
            var normal = TriangleMath.GetNormal(triangle);
            Vector3 direction = -Vector3.ProjectOnPlane(velocity.normalized, normal);
            float square = TriangleMath.GetSquare(triangle);

            float magnitude = (density * speed * speed * square * resistanceCoefficient) / 2;
            return direction * magnitude * forceMultiply;
        }
    }
}
