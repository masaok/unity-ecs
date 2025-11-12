using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PositionUpdateSystem))]
    public partial class AgingAndDecaySystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var spaceRules = SystemAPI.GetSingleton<SpaceRulesComponent>();
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            var random = Unity.Mathematics.Random.CreateFromIndex((uint)UnityEngine.Time.frameCount);

            Entities
                .WithAll<ParticleTag>()
                .ForEach((Entity entity, int entityInQueryIndex, ref ParticleComponent particle) =>
                {
                    // Check lifespan
                    if (particle.Lifespan > 0 && particle.Age >= particle.Lifespan)
                    {
                        // Create ghost if enabled
                        if (spaceRules.CreateGhosts && !particle.IsGhost)
                        {
                            // Mark for ghost creation (simplified - would spawn new entity)
                            particle.IsGhost = true;
                            particle.Age = 0;
                            particle.Lifespan = 600; // Ghost lifespan
                            particle.Color.w *= 0.3f; // Make transparent
                        }
                        else
                        {
                            // Remove particle
                            ecb.DestroyEntity(entityInQueryIndex, entity);
                        }
                    }

                    // Decay
                    if (spaceRules.DecayRate > 0 && !particle.IsGhost)
                    {
                        if (random.NextFloat() < spaceRules.DecayRate)
                        {
                            ecb.DestroyEntity(entityInQueryIndex, entity);
                        }
                    }

                }).ScheduleParallel();

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
