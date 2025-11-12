using Unity.Entities;

namespace CellularSeance.Components
{
    public enum BoundaryType : byte
    {
        VerticalRectangle,
        HorizontalRectangle,
        Square,
        Circle,
        Torus,
        Ouroboros,
        Pillar,
        Triangle,
        Lemniscate
    }

    public struct SpaceRulesComponent : IComponentData
    {
        public float Vortex;
        public float Viscosity;
        public BoundaryType Boundary;
        public float BackgroundCharge;
        public float GravityY;
        public float BrownianMotion;
        public float Elasticity;
        public float SpontaneousGeneration;
        public bool Predation;
        public float CriticalMass;
        public float DecayRate;
        public bool CreateGhosts;
        public bool Inertia;
        public bool ChainReaction;
        public bool ThermalShock;
        public float AmbientTemperature;
        public float HeatDissipation;
        public float StateChangeFactor;
        public float AmbientAether;
        public float AetherFlux;
        public float ResonantBondingThreshold;
        public float ResonantBondingStrength;
        public float DissonanceThreshold;
        public bool EnableResonanceBonding;
        public bool EnableDissonance;
    }

    public struct BoundaryDimensions : IComponentData
    {
        public float Width;
        public float Height;
        public float CenterX;
        public float CenterY;
    }
}
