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
    /// Calculates Archimed Force via the pressure affected on each triangle surface of the boat hull. 
    /// The pressure depends on depth of triangle center and triangle area.
    /// 
    /// Equation:
    /// F = r * g * h * S, where
    /// <list type="bullet">
    ///     <item>
    ///         <term>r (<c>DENSITY</c>)</term>
    ///         <description>Density of water, kg/m^3</description>
    ///     </item>
    ///     <item>
    ///         <term>g (<c>gravity</c>)</term>
    ///         <description>Gravity constant, m/s^2</description>
    ///         <see cref="UnityEngine.Physics.gravity.magnitude"/>
    ///     </item>
    ///     <item>
    ///         <term>h (<c>height</c>)</term>
    ///         <description>Distance between center of the triangle and waterline, m</description>
    ///     </item>
    ///     <item>
    ///         <term>S (<c>square</c>)</term>
    ///         <description>Area of the triangle, m^2</description>
    ///     </item>
    ///     <item>
    ///         <term>F (result force)</term>
    ///         <description>Archimed force, N=(kg*m)/s^2</description>
    ///     </item>
    /// </list>
    /// 
    /// Direction = normal of triangle.
    /// </summary>
    internal class ArchimedForce : IForce
    {
        private readonly float DENSITY;
        private readonly bool UP_ONLY;

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

