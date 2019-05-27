using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

namespace Buoyancy.Physics
{
    class PressureForces
    {
        public static float pressureDragCoefficient = 1200f;
        public static float suctionDragCoefficient = 1200f;
        public static float pressureFalloffPower = 0.1f;
        public static float suctionFalloffPower = 0.1f;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float speed;
        private Vector3 normal;
        private float angleNormalVelocity;
        private float cosNormalVelocity;

        public PressureForces(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
            this.center = TriangleMath.GetCenter(triangle);
            this.speed = rb.velocity.magnitude;
            this.normal = TriangleMath.GetNormal(triangle);
            this.angleNormalVelocity = Vector3.Angle(normal, rb.velocity);
            this.cosNormalVelocity = Mathf.Cos(angleNormalVelocity);
        }

        public static void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            new PressureForces(triangle, rb).ApplyForce();
        }

        private void ApplyForce()
        {
            Vector3 force;
            if (angleNormalVelocity <= 90f)
            {
                force = MakePressureForce();
                rb.AddForceAtPosition(force, center);
                //TODO debug purposes DisplayWorker.DisplayForce(center, force);
            }
            else if (angleNormalVelocity >= 90f)
            {
                force = MakeSuctionForce();
                rb.AddForceAtPosition(force, center);
                //TODO debug purposes DisplayWorker.DisplayForce(center, force);
            }
        }

        private Vector3 MakePressureForce()
        {
            Vector3 direction = -normal;
            float square = TriangleMath.GetSquare(triangle);

            float magnitude = 
                pressureDragCoefficient * 
                speed * speed * 
                square * 
                Mathf.Pow(Mathf.Abs(cosNormalVelocity), pressureFalloffPower);
            return direction * Mathf.Abs(magnitude);
        }

        private Vector3 MakeSuctionForce()
        {
            Vector3 direction = normal;
            float square = TriangleMath.GetSquare(triangle);

            float magnitude =
                suctionDragCoefficient *
                speed * speed *
                square *
                Mathf.Pow(Mathf.Abs(cosNormalVelocity), suctionFalloffPower);
            return direction * Mathf.Abs(magnitude);
        }
    }
}
