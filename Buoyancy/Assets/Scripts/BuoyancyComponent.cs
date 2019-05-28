using System.Collections.Generic;
using UnityEngine;
using Buoyancy.MeshWorker;
using Buoyancy.Struct;
using Buoyancy.Math;

namespace Buoyancy
{
    public class BuoyancyComponent : BuoyancyComponentInspector
    {
        private Rigidbody rb;
        private float totalTrianglesCount;
        
        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            totalTrianglesCount = hullMesh.triangles.Length / 3;

            HandleInspectorMenu();
        }

        private void FixedUpdate()
        {
            var triangles = TriangleParser.parse(hullMesh, transform);
            var underwater = HullMath.GetUnderwaterTriangles(triangles);

            if (debugHandler != null)
            {
                debugHandler(underwater);
            }
            ApplyForces(underwater);
            WaterMath.casheHeightMap.Clear();
        }

        private void ApplyForces(List<Triangle> underwater)
        {
            if (applyForcesHandler == null)
            {
                return;
            }
            foreach (Triangle triangle in underwater)
            {
                applyForcesHandler(triangle, rb);
            }
        }
    }
}