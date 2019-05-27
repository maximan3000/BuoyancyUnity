using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

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
            //TODO debug purposes DisplayWorker.DisplayForce(center, force);
        }

        private Vector3 MakeForce()
        {
            Vector3 direction = -TriangleMath.GetNormal(triangle);
            float square = TriangleMath.GetSquare(triangle);

            float magnitude = density * gravity * height * square;
            /* TODO If need only vertical force */
            //direction = Vector3.Project(direction, Vector3.up);
            return direction * Mathf.Abs(magnitude);
        }
    }
}

