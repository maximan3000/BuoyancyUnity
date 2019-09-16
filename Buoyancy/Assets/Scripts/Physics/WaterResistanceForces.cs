using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Debug;

namespace Buoyancy.Physics
{
    /// <summary>
    /// Calculates the water drag (on the move) in terms of Viscosity (Viscous Water Resistance).
    /// The force calculation uses Reynolds Coefficient so the force magnitude depends on speed very much.
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
    ///         <description>Multiplier to control the force, dimensionless quantity
    ///         C = 0.075/(log10(Re)-2)^2,
    ///         Re (Reynolds) = (r*V)/w, w (<c>VISCOSITY</c>) is kinematic viscosity of water, m^2/s
    ///         </description>
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
    internal class WaterResistanceForces : IForce
    {
        private readonly float FORCE_MULTIPLY;
        private readonly float DENSITY;
        private readonly float VISCOSITY;

        private float lastSpeed;
        private float resistanceCoefficient;

        private Rigidbody rb;
        private Triangle triangle;
        private Vector3 center;
        private float speed;

        public WaterResistanceForces(float forceMultiply, float density, float viscosity)
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
            float reynoldsCoefficient = (DENSITY * speed) / VISCOSITY;

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
