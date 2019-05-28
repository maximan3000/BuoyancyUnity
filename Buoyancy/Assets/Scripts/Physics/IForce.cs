using Buoyancy.Struct;
using UnityEngine;

namespace Buoyancy.Physics
{
    public interface IForce
    {
        void ApplyForce(Triangle triangle, Rigidbody rb);
    }
}
