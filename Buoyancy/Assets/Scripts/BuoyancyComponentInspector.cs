using System.Collections.Generic;
using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Physics;
using Buoyancy.Debug;

namespace Buoyancy
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    [HelpURL("https://www.gamasutra.com/view/news/237528/Water_interaction_model_for_boats_in_video_games.php")]
    public class BuoyancyComponentInspector : MonoBehaviour
    {
        #region ComponentParams
        [Header("Center of mass")]
        [Tooltip("If on, center of mass will be set to the gameObject's position")]
        public bool changeCenterOfMass;
        public GameObject newCenterOfMass = null;

        [Header("Buoyancy mesh")]
        [Tooltip("Use low polygonal meshes to improve performance")]
        public Mesh hullMesh;

        [Header("Global parameters")]
        [Range(100f, 2000f)]
        public float densityOfWater = 1000f;

        [Header("Archimed Force")]
        public bool useArchimedForce;
        public bool upOnly;
        [Header("Resistance Forces")]
        public bool useResistanceForces;
        [Range(0f, 1000f)]
        public float forceMultiply = 300f;
        [Range(0f, 2f)]
        public float viscosity = 0.0014f;
        [Header("Pressure Forces parameters")]
        public bool usePressureForces;
        [Range(0f, 2000f)]
        public float pressureDragCoefficient = 1200f;
        [Range(0f, 2000f)]
        public float suctionDragCoefficient = 1200f;
        [Range(0f, 1f)]
        public float pressureFallOfPower = 0.1f;
        [Range(0f, 1f)]
        public float suctionFallOfPower = 0.1f;
        #endregion

        #region DebugPurposes
        [Header("Debug purposes")]
        public bool showUnderwaterPart;
        protected delegate void DebugHandler(List<Triangle> triangles);
        protected DebugHandler debugHandler;
        #endregion

        protected delegate void ApplyForcesHandler(Triangle triangle, Rigidbody rb);
        protected ApplyForcesHandler applyForcesHandler;

        protected void HandleInspectorMenu()
        {
            if (useArchimedForce)
            {
                var archimedForce = ForceFabric.GetArchimedForce(densityOfWater, upOnly);
                applyForcesHandler += archimedForce.ApplyForce;
            }
            if (useResistanceForces)
            {
                var resistanceForces = ForceFabric.GetResistanceForces(forceMultiply, densityOfWater, viscosity);
                applyForcesHandler += resistanceForces.ApplyForce;
            }
            if (usePressureForces)
            {
                var pressureForces = ForceFabric.GetPressureForces(pressureDragCoefficient, suctionDragCoefficient, 
                    pressureFallOfPower, suctionFallOfPower);
                applyForcesHandler += pressureForces.ApplyForce;
            }
            if (changeCenterOfMass)
            {
                var center = transform.InverseTransformPoint(newCenterOfMass.transform.position);
                gameObject.GetComponent<Rigidbody>().centerOfMass = center;
            }
            if (showUnderwaterPart)
            {
                debugHandler += DisplayWorker.DisplayTriangles;
            }
        }

    }
}
