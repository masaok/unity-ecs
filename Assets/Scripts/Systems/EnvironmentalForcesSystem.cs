using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(StateUpdateSystem))]
    public partial class EnvironmentalForcesSystem : SystemBase
    {
        private Unity.Mathematics.Random _random;

        protected override void OnCreate()
        {
            base.OnCreate();
            _random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        }

        protected override void OnUpdate()
        {
            var spaceRules = SystemAPI.GetSingleton<SpaceRulesComponent>();
            var boundaries = SystemAPI.GetSingleton<BoundaryDimensions>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            var seed = _random.NextUInt();

            Entities
                .WithAll<ParticleTag>()
                .ForEach((ref ParticleComponent particle, in Entity entity) =>
                {
                    var random = Unity.Mathematics.Random.CreateFromIndex((uint)entity.Index + seed);

                    // Viscosity (drag) - state dependent
                    float viscosity = spaceRules.Viscosity;
                    if (particle.State == ParticleState.Gas)
                    {
                        viscosity = 1.0f; // Higher drag for gas
                    }
                    else if (particle.State == ParticleState.Solid)
                    {
                        viscosity = 0.9f; // Lower drag for solid
                    }
                    particle.Velocity *= viscosity;

                    // Gravity
                    particle.Velocity.y += spaceRules.GravityY * deltaTime;

                    // Brownian motion
                    if (spaceRules.BrownianMotion > 0)
                    {
                        float brownianScale = spaceRules.BrownianMotion * deltaTime;
                        if (particle.State == ParticleState.Gas)
                        {
                            brownianScale *= 1.5f; // More brownian motion for gas
                        }
                        particle.Velocity.x += random.NextFloat(-brownianScale, brownianScale);
                        particle.Velocity.y += random.NextFloat(-brownianScale, brownianScale);
                    }

                    // Background charge
                    if (math.abs(spaceRules.BackgroundCharge) > 0.001f)
                    {
                        float force = spaceRules.BackgroundCharge * particle.Charge * deltaTime;
                        particle.Velocity += new float2(force, force);
                    }

                    // Vortex (spiral toward center)
                    if (spaceRules.Vortex > 0)
                    {
                        float2 center = new float2(boundaries.CenterX, boundaries.CenterY);
                        float2 toCenter = center - particle.Position;
                        float dist = math.length(toCenter);
                        if (dist > 1f)
                        {
                            float2 direction = toCenter / dist;
                            float2 perpendicular = new float2(-direction.y, direction.x);
                            float vortexForce = spaceRules.Vortex * deltaTime;
                            particle.Velocity += perpendicular * vortexForce;
                            particle.Velocity += direction * (vortexForce * 0.1f);
                        }
                    }

                    // Buoyancy (aether-based)
                    if (particle.Aether > spaceRules.AmbientAether)
                    {
                        float buoyancy = (particle.Aether - spaceRules.AmbientAether) * 0.001f * deltaTime;
                        particle.Velocity.y -= buoyancy;
                    }

                    // Inertia (mass-based resistance)
                    if (spaceRules.Inertia)
                    {
                        float inertiaFactor = 1.0f / (1.0f + particle.Mass * 0.01f);
                        particle.Velocity *= inertiaFactor;
                    }

                    // Aether flux
                    particle.Aether += (spaceRules.AmbientAether - particle.Aether) * spaceRules.AetherFlux * deltaTime;

                    // Heat dissipation
                    particle.Temperature += (spaceRules.AmbientTemperature - particle.Temperature) * spaceRules.HeatDissipation * deltaTime;

                }).ScheduleParallel();
        }
    }
}
