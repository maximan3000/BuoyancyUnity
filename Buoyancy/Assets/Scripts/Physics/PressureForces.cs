using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

namespace Buoyancy.Physics
{
    internal class PressureForces : IForce
    {
        private readonly float PRESSURE_DRAG_COEFFICIENT;
        private readonly float SUCTION_DRAG_COEFFICIENT;
        private readonly float PRESSURE_FALL_OF_POWER;
        private readonly float SUCTION_FALL_OF_POWER;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float speed;
        private Vector3 normal;
        private float angleNormalVelocity;
        private float cosNormalVelocity;

        public PressureForces(float pressureDragCoefficient, float suctionDragCoefficient, 
            float pressureFallOfPower, float suctionFallOfPower)
        {
            this.PRESSURE_DRAG_COEFFICIENT = pressureDragCoefficient;
            this.SUCTION_DRAG_COEFFICIENT = suctionDragCoefficient;
            this.PRESSURE_FALL_OF_POWER = pressureFallOfPower;
            this.SUCTION_FALL_OF_POWER = suctionFallOfPower;
        }

        public void ApplyForce(Triangle triangle, Rigidbody rb)
        {
            this.triangle = triangle;
            this.rb = rb;
            this.center = TriangleMath.GetCenter(triangle);
            this.speed = rb.velocity.magnitude;
            this.normal = TriangleMath.GetNormal(triangle);
            this.angleNormalVelocity = Vector3.Angle(normal, rb.velocity);
            this.cosNormalVelocity = Mathf.Cos(angleNormalVelocity);

            ApplyForce();
        }

        private void ApplyForce()
        {
            Vector3 force;
            if (angleNormalVelocity <= 90f)
            {
                force = MakePressureForce();
                rb.AddForceAtPosition(force, center);
                //TODO debug purposes 
                //DisplayWorker.DisplayForce(center, force);
            }
            else if (angleNormalVelocity >= 90f)
            {
                force = MakeSuctionForce();
                rb.AddForceAtPosition(force, center);
                //TODO debug purposes 
                //DisplayWorker.DisplayForce(center, force);
            }
        }

        private Vector3 MakePressureForce()
        {
            Vector3 direction = -normal;
            float square = TriangleMath.GetSquare(triangle);

            float magnitude = 
                PRESSURE_DRAG_COEFFICIENT * 
                speed * speed * 
                square * 
                Mathf.Pow(Mathf.Abs(cosNormalVelocity), PRESSURE_FALL_OF_POWER);
            return direction * Mathf.Abs(magnitude);
        }

        private Vector3 MakeSuctionForce()
        {
            Vector3 direction = normal;
            float square = TriangleMath.GetSquare(triangle);

            float magnitude =
                SUCTION_DRAG_COEFFICIENT *
                speed * speed *
                square *
                Mathf.Pow(Mathf.Abs(cosNormalVelocity), SUCTION_FALL_OF_POWER);
            return direction * Mathf.Abs(magnitude);
        }
    }
}
