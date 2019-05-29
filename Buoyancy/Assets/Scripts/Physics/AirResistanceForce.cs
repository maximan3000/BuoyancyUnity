using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

namespace Buoyancy.Physics
{
    internal class AirResistanceForce : IForce
    {
        private readonly float RESISTANCE_COEFFICIENT;
        private readonly float DENSITY;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float speed;

        public AirResistanceForce(float resistanceCoefficient, float density)
        {
            this.RESISTANCE_COEFFICIENT = resistanceCoefficient;
            this.DENSITY = density;
        }

        public void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
            this.center = TriangleMath.GetCenter(triangle);
            this.speed = rb.velocity.magnitude;

            ApplyForce();
        }

        private void ApplyForce()
        {
            var force = MakeForce();
            rb.AddForceAtPosition(force, center);
            //TODO debug purposes 
            //DisplayWorker.DisplayForce(center, force);
        }

        private Vector3 MakeForce()
        {
            var velocity = rb.velocity;
            var normal = TriangleMath.GetNormal(triangle);
            Vector3 direction = -Vector3.ProjectOnPlane(velocity.normalized, normal);
            float square = TriangleMath.GetSquare(triangle);

            float magnitude = (DENSITY * speed * speed * square * RESISTANCE_COEFFICIENT) / 2f;
            return direction * Mathf.Abs(magnitude) * RESISTANCE_COEFFICIENT;
        }
    }
}
