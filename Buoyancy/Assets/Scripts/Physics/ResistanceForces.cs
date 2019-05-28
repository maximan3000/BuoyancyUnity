using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

namespace Buoyancy.Physics
{
    internal class ResistanceForces : IForce
    {
        private float FORCE_MULTIPLY;
        private float DENSITY;
        private float VISCOSITY;

        private float lastSpeed;
        private float resistanceCoefficient;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float speed;

        public ResistanceForces(float forceMultiply, float density, float viscosity)
        {
            this.FORCE_MULTIPLY = forceMultiply;
            this.DENSITY = density;
            this.VISCOSITY = viscosity;
        }

        public void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
            this.center = TriangleMath.GetCenter(triangle);
            this.speed = rb.velocity.magnitude;
            if (lastSpeed != speed)
            {
                lastSpeed = speed;
                RecalculateResistanceCoefficient();
            }

            ApplyForce();
        }

        private void RecalculateResistanceCoefficient()
        {
            float length = TriangleMath.GetLength(triangle);
            float reynoldsCoefficient = (DENSITY * speed * length) / VISCOSITY;

            float temp = Mathf.Log10(reynoldsCoefficient) - 2f;
            float resistance = 0.075f / (temp * temp);
            resistanceCoefficient = Mathf.Abs(resistance);
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

            float magnitude = (DENSITY * speed * speed * square * resistanceCoefficient) / 2f;
            return direction * Mathf.Abs(magnitude) * FORCE_MULTIPLY;
        }
    }
}
