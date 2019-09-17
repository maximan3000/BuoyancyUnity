/* 
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
using Buoyancy.MeshWorker;
using Buoyancy.Struct;
using Buoyancy.Math;

namespace Buoyancy
{
    /// <summary>
    /// The main component that manages all calls for computing physics of floating bodies.
    /// </summary>
    /// <example>
    /// Steps you need to do to make it work:
    /// <list type="number">
    ///     <item>
    ///         <term>Setup your water</term>
    ///         <description>The component requires a function <c>Func<Vector3,float></c> 
    ///         that returns depth of any point. If the point above water then depth>0.0 else depth<=0.0.
    ///         Water may be simple like plane (depth = height_of_plane - point.Y) 
    ///         or complicated with waves function</description>
    ///         <see cref="WaterMath.GetWaterHeight"/>
    ///     </item>
    ///     <item>
    ///         <term>Add this component</term>
    ///         <description>Add component <c>BuoyancyComponent</c> 
    ///         to the gameobject you want to be floating</description>
    ///     </item>
    ///     <item>
    ///         <term>Add simplified mesh</term>
    ///         <description>Add simplified mesh (less vertex is better) to <c>BuoyancyComponent.hullMesh</c>
    ///         to optimise physics calculation because each vertex is used in it</description>
    ///     </item>
    ///     <item>
    ///         <term>Setup center of mass (optional)</term>
    ///         <description>You can choose center of mass different from center of the boat (default).
    ///         Create any simple gameobject (like sphere) and move it inside the boat. Then add 
    ///         the sphere to <c>BuoyancyComponent.newCenterOfMass</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Setup physics parameters</term>
    ///         <description>Setup parameters in the Unity Inspector window to make physics more realisitc</description>
    ///     </item>
    /// </list>
    /// </example>
    public class BuoyancyComponent : BuoyancyComponentInspector
    {
        private Rigidbody rb;
        
        void Start()
        {
            WaterMath.GetWaterHeight += Demo.Ocean.OceanAdvanced.GetWaterHeight;
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
            
            WaterMath.caсheHeightMap.Clear();
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