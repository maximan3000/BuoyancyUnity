using Buoyancy.Struct;
using UnityEngine;

namespace Buoyancy.Physics
{
    /// <summary>
    /// Interface for all forces that are depend on triangles.
    /// </summary>
    public interface IForce
    {
        /// <summary>
        /// Applies force to rigidbody.
        /// </summary>
        /// <param name="triangle">Triangle for which calculates the force</param>
        /// <param name="rb"><c>Rigidbody</c> component of the gameobject (boat, etc)
        /// <see cref="Rigidbody.AddForceAtPosition"/></param>
        void ApplyForce(Triangle triangle, Rigidbody rb);
    }
}
