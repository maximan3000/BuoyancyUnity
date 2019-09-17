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
