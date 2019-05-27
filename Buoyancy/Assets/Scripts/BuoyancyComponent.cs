using System.Collections.Generic;
using UnityEngine;
using Buoyancy.MeshWorker;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Physics;
using Buoyancy.Debug;
using System;

namespace Buoyancy
{

    [RequireComponent(typeof(Rigidbody))]
    public class BuoyancyComponent : MonoBehaviour
    {
        #region DebugPurposes
        [Header("Debug purposes")]
        public bool showUnderwaterPart;
        
        private delegate void DebugHandler(List<Triangle> triangles);
        private DebugHandler debugHandler;
        #endregion

        #region ComponentParams
        [Header("Center of mass")]
        [Tooltip("If on, center of mass will be set to the gameObject's position")]
        public bool shiftCenterOfMass;
        public GameObject centerOfMass = null;

        [Header("Buoyancy mesh")]
        [Tooltip("Use low polygonal meshes to improve performance")]
        public Mesh hullMesh;

        [Header("Archimed Force parameters")]
        public bool useArchimedForce;
        [Header("Resistance Forces parameters")]
        public bool useResistanceForces;
        [Header("Pressure Forces parameters")]
        public bool usePressureForces;
        #endregion

        private Rigidbody rb;
        private float totalTrianglesCount;
        private delegate void ApplyForcesHandler(Triangle triangle, Rigidbody rb);
        private ApplyForcesHandler applyForcesHandler;

        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            totalTrianglesCount = hullMesh.triangles.Length / 3;

            HandleInspectorMenu();
        }

        private void HandleInspectorMenu()
        {
            if (useArchimedForce)
                applyForcesHandler += ArchimedForce.ApplyForce;
            if (useResistanceForces)
                applyForcesHandler += ResistanceForces.ApplyForce;
            if (usePressureForces)
                applyForcesHandler += PressureForces.ApplyForce;

            if (shiftCenterOfMass)
            {
                var center = transform.InverseTransformPoint(centerOfMass.transform.position);
                rb.centerOfMass = center;
            }

            if (showUnderwaterPart)
            {
                debugHandler += DisplayWorker.DisplayTriangles;
            }
        }

        private void FixedUpdate()
        {
            var triangles = TriangleParser.parse(hullMesh, transform);
            var underwater = HullMath.GetUnderwaterTriangles(triangles);

            if (debugHandler != null)
                debugHandler(underwater);
            ApplyForces(underwater);
            WaterMath.casheHeightMap.Clear();
        }

        private void ApplyForces(List<Triangle> underwater)
        {
            foreach (Triangle triangle in underwater)
            {
                applyForcesHandler(triangle, rb);
            }
        }
    }
}