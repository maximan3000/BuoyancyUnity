using UnityEngine;

namespace Buoyancy.Struct
{
    /// <summary>
    /// Inner (for this library) type for mesh triangles that is convenient for math operations
    /// </summary>
    /// <see cref="Mesh.triangles"/>
    /// <see cref="Mesh.vertices"/>
    public struct Triangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        public Triangle(Vector3 A, Vector3 B, Vector3 C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
        }
    }
}
