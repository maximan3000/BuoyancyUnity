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
    class PressureForces
    {
        public static float pressureDragCoefficient = 300f;
        public static float suctionDragCoefficient = 300f;
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
            speed *= 2;
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
            }
            else if (angleNormalVelocity >= 90f)
            {
                force = MakeSuctionForce();
                rb.AddForceAtPosition(force, center);
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
            //Debug.DrawRay(center, direction, Color.white);
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
            //Debug.DrawRay(center, direction, Color.white);
            return direction * Mathf.Abs(magnitude);
        }
    }
}
