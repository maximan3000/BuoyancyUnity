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
        
        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            HandleInspectorMenu();
        }

        private void FixedUpdate()
        {
            var triangles = TriangleParser.parse(hullMesh, transform);
            var hullTriangles = HullMath.CutHullAtWaterline(triangles);

            if (debugHandler != null)
            {
                debugHandler(hullTriangles.underwater);
            }
            if (underwaterForces != null)
            {
                ApplyUnderwaterForces(hullTriangles.underwater);
            }
            if (abovewaterForces != null)
            {
                ApplyAbovewaterForces(hullTriangles.abovewater);
            }
            
            WaterMath.casheHeightMap.Clear();
        }

        private void ApplyUnderwaterForces(List<Triangle> underwater)
        {
            foreach (Triangle triangle in underwater)
            {
                underwaterForces(triangle, rb);
            }
        }

        private void ApplyAbovewaterForces(List<Triangle> abovewater)
        {
            foreach (Triangle triangle in abovewater)
            {
                abovewaterForces(triangle, rb);
            }
        }
    }
}