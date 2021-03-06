﻿/* 
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

using System.Collections.Generic;
using UnityEngine;
using Buoyancy.Struct;
using Buoyancy.Physics;
using Buoyancy.Debug;

namespace Buoyancy
{
    /// <summary>
    /// Additional class for <c>BuoyancyComponent</c> that processes interactions with interface in Unity Editor. 
    /// It also contains all settings (variables & constants) for <c>BuoyancyComponent</c>.
    /// <see cref="BuoyancyComponent"/>
    /// </summary>
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
        [Tooltip("Density, kg/m^3")]
        [Range(100f, 2000f)]
        public float densityOfWater = 1000f;
        [Range(0f, 100f)]
        public float densityOfAir = 1.2f;

        [Header("Archimed Force")]
        public bool useArchimedForce;
        public bool upOnly;

        [Header("Water Resistance")]
        public bool useWaterResistanceForces;
        [Range(0f, 1000f)]
        public float waterResistanceMultiply = 300f;
        [Range(0.00001f, 0.0000001f)]
        [Tooltip("Kinematic viscosity, m^2/s")]
        public float viscosity = 0.000001789f;

        [Header("Air Resistance")]
        public bool useAirResistanceForces;
        [Range(0f, 1000f)]
        public float airResistanceCoefficient = 1f;

        [Header("Pressure Forces")]
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
        protected ApplyForcesHandler underwaterForces;
        protected ApplyForcesHandler abovewaterForces;

        protected void HandleInspectorMenu()
        {
            if (useArchimedForce)
            {
                var archimedForce = ForceFabric.GetArchimedForce(densityOfWater, upOnly);
                underwaterForces += archimedForce.ApplyForce;
            }
            if (useWaterResistanceForces)
            {
                var waterResistance = ForceFabric.GetWaterResistanceForces(waterResistanceMultiply, densityOfWater, viscosity);
                underwaterForces += waterResistance.ApplyForce;
            }
            if (useAirResistanceForces)
            {
                var airResistance = ForceFabric.GetAirResistanceForce(airResistanceCoefficient, densityOfAir);
                abovewaterForces += airResistance.ApplyForce;
            }
            if (usePressureForces)
            {
                var pressureForces = ForceFabric.GetPressureForces(pressureDragCoefficient, suctionDragCoefficient, 
                    pressureFallOfPower, suctionFallOfPower);
                underwaterForces += pressureForces.ApplyForce;
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
