namespace Buoyancy.Physics
{
    public static class ForceFabric
    {
        public static IForce GetArchimedForce(
            float density = 1000f,
            bool upOnly = false)
        {
            return new ArchimedForce(density, upOnly);
        }

        public static IForce GetResistanceForces(
            float forceMultiply = 300f,
            float density = 1000f,
            float viscosity = 0.0014f)
        {
            return new ResistanceForces(forceMultiply, density, viscosity);
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
    }
}
