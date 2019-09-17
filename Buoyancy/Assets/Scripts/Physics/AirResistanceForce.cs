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
    /// Calculates air resistance same with method of <c>WaterResistanceForces</c> 
    /// (using traditional equation for drag in fluid dynamics)
    /// 
    /// Equation:
    /// R = 1/2 * r * C * S * V^2, where
    /// <list type="bullet">
    ///     <item>
    ///         <term>r (<c>DENSITY</c>)</term>
    ///         <description>Density of water, kg/m^3</description>
    ///     </item>
    ///     <item>
    ///         <term>C (<c>FORCE_MULTIPLY</c>)</term>
    ///         <description>Multiplier to control the force, dimensionless quantity</description>
    ///     </item>
    ///     <item>
    ///         <term>S (<c>square</c>)</term>
    ///         <description>Area of the triangle, m^2</description>
    ///     </item>
    ///     <item>
    ///         <term>V (<c>speed</c>)</term>
    ///         <description>Speed of vehicle, m/s^2</description>
    ///     </item>
    ///     <item>
    ///         <term>R (result force)</term>
    ///         <description>Resistance force, N=(kg*m)/s^2</description>
    ///     </item>
    /// </list>
    /// 
    /// Direction = -V projected on plane of triangle.
    /// </summary>
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
