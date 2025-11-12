using Unity.Entities;
using Unity.Mathematics;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    public partial class AnomalySystem : SystemBase
    {
        private Unity.Mathematics.Random _random;

        protected override void OnCreate()
        {
            base.OnCreate();
            _random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        }

        protected override void OnUpdate()
        {
            // Anomalies are applied during world generation, not every frame
        }

        public void ApplyRandomAnomaly(ref SpaceRulesComponent spaceRules, float frequency)
        {
            if (_random.NextFloat() > frequency)
            {
                return; // No anomaly
            }

            // Check for special anomalies (0.1% chance)
            if (_random.NextFloat() < 0.001f)
            {
                ApplySpecialAnomaly(ref spaceRules);
                return;
            }

            // Apply standard anomaly
            var anomalyType = (AnomalyType)_random.NextInt(1, 23);
            ApplyAnomaly(ref spaceRules, anomalyType);
        }

        private void ApplyAnomaly(ref SpaceRulesComponent spaceRules, AnomalyType type)
        {
            switch (type)
            {
                case AnomalyType.Whirlpool:
                    spaceRules.Vortex = _random.NextFloat(0.02f, 0.04f);
                    break;

                case AnomalyType.ThickAether:
                    spaceRules.AmbientAether = _random.NextFloat(70f, 100f);
                    spaceRules.AetherFlux = 0.005f;
                    break;

                case AnomalyType.ChargedAtmosphere:
                    spaceRules.BackgroundCharge = _random.NextFloat(-0.5f, 0.5f);
                    break;

                case AnomalyType.UniversalGravity:
                    spaceRules.GravityY = _random.NextFloat(0.05f, 0.1f);
                    break;

                case AnomalyType.ChaoticEnergy:
                    spaceRules.BrownianMotion = _random.NextFloat(30f, 60f);
                    break;

                case AnomalyType.Predation:
                    spaceRules.Predation = true;
                    break;

                case AnomalyType.CriticalMass:
                    spaceRules.CriticalMass = 20f;
                    break;

                case AnomalyType.VoidEchoes:
                    spaceRules.CreateGhosts = true;
                    break;

                case AnomalyType.ImperfectCollisions:
                    spaceRules.Elasticity = _random.NextFloat(0.4f, 0.7f);
                    break;

                case AnomalyType.SpontaneousApparition:
                    spaceRules.SpontaneousGeneration = _random.NextFloat(0.0001f, 0.001f);
                    break;

                case AnomalyType.Inertia:
                    spaceRules.Inertia = true;
                    break;

                case AnomalyType.CascadingDestruction:
                    spaceRules.ChainReaction = true;
                    break;

                case AnomalyType.ThermalShock:
                    spaceRules.ThermalShock = true;
                    break;

                case AnomalyType.AbsoluteZero:
                    spaceRules.AmbientTemperature = -50f;
                    spaceRules.HeatDissipation = 0.01f;
                    break;

                case AnomalyType.Supernova:
                    spaceRules.AmbientTemperature = 200f;
                    spaceRules.HeatDissipation = 0.01f;
                    break;

                case AnomalyType.PhaseVolatility:
                    spaceRules.StateChangeFactor = _random.NextFloat(0.5f, 0.7f);
                    break;

                case AnomalyType.AethericTide:
                    spaceRules.AetherFlux = _random.NextFloat(0.003f, 0.005f);
                    break;

                case AnomalyType.ResonantHarmony:
                    spaceRules.EnableResonanceBonding = true;
                    spaceRules.ResonantBondingStrength = 0.1f;
                    break;

                case AnomalyType.DissonantFeedback:
                    spaceRules.EnableDissonance = true;
                    spaceRules.DissonanceThreshold = 0.85f;
                    break;
            }
        }

        private void ApplySpecialAnomaly(ref SpaceRulesComponent spaceRules)
        {
            int specialType = _random.NextInt(0, 3);
            switch (specialType)
            {
                case 0: // Strange Attractor
                    spaceRules.Vortex = 0.1f; // Very strong attraction
                    break;

                case 1: // Strange Repulsor
                    spaceRules.BackgroundCharge = 2.0f; // Very strong repulsion
                    break;

                case 2: // Odd Particle
                    // This would spawn a special particle that freezes others on contact
                    // Implementation would require spawning logic
                    break;
            }
        }

        public string GetAnomalyName(AnomalyType type)
        {
            return type switch
            {
                AnomalyType.Whirlpool => "Whirlpool",
                AnomalyType.ThickAether => "Thick Aether",
                AnomalyType.ChargedAtmosphere => "Charged Atmosphere",
                AnomalyType.UniversalGravity => "Universal Gravity",
                AnomalyType.ChaoticEnergy => "Chaotic Energy",
                AnomalyType.Predation => "Predation",
                AnomalyType.CriticalMass => "Critical Mass",
                AnomalyType.VoidEchoes => "Void Echoes",
                AnomalyType.ImperfectCollisions => "Imperfect Collisions",
                AnomalyType.SpontaneousApparition => "Spontaneous Apparition",
                AnomalyType.Inertia => "Inertia",
                AnomalyType.CascadingDestruction => "Cascading Destruction",
                AnomalyType.ThermalShock => "Thermal Shock",
                AnomalyType.AbsoluteZero => "Absolute Zero",
                AnomalyType.Supernova => "Supernova",
                AnomalyType.PhaseVolatility => "Phase Volatility",
                AnomalyType.AethericTide => "Aetheric Tide",
                AnomalyType.ResonantHarmony => "Resonant Harmony",
                AnomalyType.DissonantFeedback => "Dissonant Feedback",
                AnomalyType.StrangeAttractor => "Strange Attractor",
                AnomalyType.StrangeRepulsor => "Strange Repulsor",
                AnomalyType.OddParticle => "Odd Particle",
                _ => "None"
            };
        }
    }
}
