using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

namespace Buoyancy.Physics
{
    internal class ArchimedForce : IForce
    {
        private float DENSITY;
        private bool UP_ONLY;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float height;

        public ArchimedForce(float density, bool upOnly)
        {
            this.DENSITY = density;
            this.UP_ONLY = upOnly;
        }

        public void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
            this.center = TriangleMath.GetCenter(triangle);
            this.height = WaterMath.DistanceToWater(center);
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
            Vector3 direction = -TriangleMath.GetNormal(triangle);
            float square = TriangleMath.GetSquare(triangle);
            float gravity = UnityEngine.Physics.gravity.magnitude;

            float magnitude = DENSITY * gravity * height * square;
            if (UP_ONLY)
            {
                direction = Vector3.Project(direction, Vector3.up);
            }
            return direction * Mathf.Abs(magnitude);
        }
    }
}

