using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Buoyancy.MeshWorker;
using Buoyancy.Struct;
using Buoyancy.Math;
using Buoyancy.Physics;

namespace Buoyancy
{


    public class BuoyancyComponent : MonoBehaviour
    {
        //public GameObject underwaterMesh;
        public GameObject centerOfMass = null;

        private Mesh mesh;
        private List<Triangle> triangles;
        private Rigidbody rb;
        private float totalTrianglesCount;

        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            mesh = gameObject.GetComponent<MeshFilter>().mesh;
            totalTrianglesCount = mesh.triangles.Length / 3;

            if (centerOfMass!=null)
            {
                var center = transform.InverseTransformPoint(centerOfMass.transform.position);
                rb.centerOfMass = center;
            }
        }

        private void FixedUpdate()
        {
            triangles = TriangleParser.parse(mesh, transform);
            var underwater = HullMath.GetUnderwaterTriangles(triangles);

            //var linkMesh = underwaterMesh.GetComponent<MeshFilter>().mesh;
            //DisplayWorker.DisplayTriangles(linkMesh, transform, underwater);

            ApplyForces(underwater);
            WaterMath.casheHeightMap.Clear();
        }

        private void ApplyForces(List<Triangle> underwater)
        {
            foreach (Triangle triangle in underwater)
            {
                ArchimedForce.ApplyForce(triangle, rb);
                ResistanceForces.ApplyForce(triangle, rb);
                PressureForces.ApplyForce(triangle, rb);
            }
        }
    }
}