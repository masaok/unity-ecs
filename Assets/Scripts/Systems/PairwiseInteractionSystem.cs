using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using CellularSeance.Components;
using CellularSeance.Data;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(EnvironmentalForcesSystem))]
    public partial class PairwiseInteractionSystem : SystemBase
    {
        private SpatialHashSystem _spatialHashSystem;
        public InteractionRulesData InteractionRules;

        protected override void OnCreate()
        {
            base.OnCreate();
            _spatialHashSystem = World.GetOrCreateSystemManaged<SpatialHashSystem>();
        }

        protected override void OnUpdate()
        {
            if (!InteractionRules.Rules.data.IsCreated)
                return;

            var spatialHash = _spatialHashSystem.SpatialHash;
            var particleLookup = GetComponentLookup<ParticleComponent>(false);
            var deltaTime = SystemAPI.Time.DeltaTime;
            var rules = InteractionRules;

            Entities
                .WithAll<ParticleTag>()
                .WithReadOnly(spatialHash)
                .ForEach((Entity entity, ref ParticleComponent particle) =>
                {
                    int centerX = (int)math.floor(particle.Position.x / SpatialHashSystem.CellSize);
                    int centerY = (int)math.floor(particle.Position.y / SpatialHashSystem.CellSize);

                    // Check 3x3 grid
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int hash = SpatialHashSystem.HashPosition(centerX + dx, centerY + dy);
                            if (spatialHash.TryGetFirstValue(hash, out var otherEntity, out var iterator))
                            {
                                do
                                {
                                    if (otherEntity != entity && particleLookup.HasComponent(otherEntity))
                                    {
                                        var otherParticle = particleLookup[otherEntity];

                                        // Get interaction rule
                                        var rule = rules.GetRule(particle.TypeId, otherParticle.TypeId);

                                        float2 diff = otherParticle.Position - particle.Position;
                                        float distSq = math.lengthsq(diff);
                                        float rangeSq = rule.Range * rule.Range;

                                        if (distSq < rangeSq && distSq > 0.01f)
                                        {
                                            float dist = math.sqrt(distSq);
                                            float2 direction = diff / dist;

                                            // Apply force based on behavior type
                                            switch (rule.Behavior)
                                            {
                                                case BehaviorType.Attract:
                                                    {
                                                        float forceMagnitude = rule.Force * deltaTime;
                                                        particle.Velocity += direction * forceMagnitude;
                                                    }
                                                    break;

                                                case BehaviorType.Repel:
                                                    {
                                                        float forceMagnitude = rule.Force * deltaTime;
                                                        particle.Velocity -= direction * forceMagnitude;
                                                    }
                                                    break;

                                                case BehaviorType.Ignore:
                                                    break;

                                                // Contact behaviors handled in separate system
                                            }
                                        }
                                    }
                                } while (spatialHash.TryGetNextValue(out otherEntity, ref iterator));
                            }
                        }
                    }
                }).ScheduleParallel();
        }
    }
}
