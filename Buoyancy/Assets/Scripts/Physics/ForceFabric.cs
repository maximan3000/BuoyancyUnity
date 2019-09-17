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

namespace Buoyancy.Physics
{
    /// <summary>
    /// Combines the creation instances of force controllers. Creates instances via interface <c>IForce</c>
    /// </summary>
    public static class ForceFabric
    {
        public static IForce GetArchimedForce(
            float density = 1000f,
            bool upOnly = false)
        {
            return new ArchimedForce(density, upOnly);
        }

        public static IForce GetWaterResistanceForces(
            float forceMultiply = 300f,
            float density = 1000f,
            float viscosity = 0.000001789f)
        {
            return new WaterResistanceForces(forceMultiply, density, viscosity);
        }

        public static IForce GetPressureForces(
            float pressureDragCoefficient = 1200f,
            float suctionDragCoefficient = 1200f,
            float pressureFallOfPower = 0.1f,
            float suctionFallOfPower = 0.1f)
        {
            return new PressureForces(pressureDragCoefficient, suctionDragCoefficient, 
                pressureFallOfPower, suctionFallOfPower);
        }

        public static IForce GetAirResistanceForce(
            float resistanceCoefficient = 1f,
            float density = 1.2f)
        {
            return new AirResistanceForce(resistanceCoefficient, density);
        }
    }
}
