/* 
Copyright 2019 Maksim Petrov <grayen.job@gmail.com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

namespace Buoyancy.Physics
{
    /// <summary>
    /// That is a fake forces that makes turnes more realistic (controling drifting)
    /// and also create effect of planing forces (push the boat out of the water).
    /// 
    /// Pressure Drag Force - applied to triangles that are close to front of the boat.
    /// Equation (Pressure drag):
    /// F = C * V^2 * S * cos(n, V)^f, where 
    /// <list type="bullet">
    ///     <item>
    ///         <term>C,f (<c>PRESSURE_DRAG_COEFFICIENT</c><c>SUCTION_DRAG_COEFFICIENT</c>
    ///         <c>PRESSURE_FALL_OF_POWER</c><c>SUCTION_FALL_OF_POWER</c>)</term>
    ///         <description>Coefficients to control the force, f - dimensionless quantity, C - kg/m^3
    ///         C - setup magnitude, f - drift control</description>
    ///     </item>
    ///     <item>
    ///         <term>V (<c>speed</c>)</term>
    ///         <description>Speed of vehicle, m/s^2</description>
    ///     </item>
    ///     <item>
    ///         <term>S (<c>square</c>)</term>
    ///         <description>Area of the triangle, m^2</description>
    ///     </item>
    ///     <item>
    ///         <term>cos(n, V)</term>
    ///         <description>Cos from angle between normal to triangle and speed vectors</description>
    ///     </item>
    ///     <item>
    ///         <term>F (result force)</term>
    ///         <description>Pressure Drag or Suction force, N=(kg*m)/s^2</description>
    ///     </item>
    /// </list>
    /// 
    /// Direction = normal of triangle.
    /// 
    /// Suction Force magnitude is similar to Pressure Force.
    /// Suction Force - applied to triangles that are close to back of the boat.
    /// 
    /// Direction = -normal of triangle
    /// </summary>
    /// <see cref="https://www.gamasutra.com/view/news/263237/Water_interaction_model_for_boats_in_video_games_Part_2.php"/>
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
