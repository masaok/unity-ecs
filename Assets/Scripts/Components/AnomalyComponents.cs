using Unity.Entities;

namespace CellularSeance.Components
{
    public enum AnomalyType : byte
    {
        None,
        Whirlpool,
        ThickAether,
        IllusoryWalls,
        ChargedAtmosphere,
        UniversalGravity,
        ChaoticEnergy,
        Predation,
        CriticalMass,
        Conservation,
        VoidEchoes,
        ImperfectCollisions,
        SpontaneousApparition,
        Inertia,
        CascadingDestruction,
        ThermalShock,
        AbsoluteZero,
        Supernova,
        PhaseVolatility,
        AethericTide,
        EntropicField,
        ResonantHarmony,
        DissonantFeedback,
        StrangeAttractor,
        StrangeRepulsor,
        OddParticle
    }

    public struct ActiveAnomalyComponent : IComponentData
    {
        public AnomalyType Type;
        public float Strength;
        public bool IsSpecial;
    }

    public struct AnomalySettingsComponent : IComponentData
    {
        public float Frequency; // 0%, 2%, 5%, 15%
        public bool AllowSpecialAnomalies;
    }
}
