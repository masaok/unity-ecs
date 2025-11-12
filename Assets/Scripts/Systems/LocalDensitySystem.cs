using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup), OrderFirst = true)]
    public partial class LocalDensitySystem : SystemBase
    {
        private SpatialHashSystem _spatialHashSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _spatialHashSystem = World.GetOrCreateSystemManaged<SpatialHashSystem>();
        }

        protected override void OnUpdate()
        {
            var spatialHash = _spatialHashSystem.SpatialHash;
            var particleLookup = GetComponentLookup<ParticleComponent>(true);

            Entities
                .WithAll<ParticleTag>()
                .WithReadOnly(spatialHash)
                .WithReadOnly(particleLookup)
                .ForEach((ref ParticleComponent particle) =>
                {
                    int neighbors = 0;
                    const float densityRadius = 100f;
                    const float densityRadiusSq = densityRadius * densityRadius;

                    int centerX = (int)math.floor(particle.Position.x / SpatialHashSystem.CellSize);
                    int centerY = (int)math.floor(particle.Position.y / SpatialHashSystem.CellSize);

                    // Check 3x3 grid
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int hash = SpatialHashSystem.HashPosition(centerX + dx, centerY + dy);
                            if (spatialHash.TryGetFirstValue(hash, out var neighbor, out var iterator))
                            {
                                do
                                {
                                    if (particleLookup.HasComponent(neighbor))
                                    {
                                        var otherParticle = particleLookup[neighbor];
                                        float distSq = math.distancesq(particle.Position, otherParticle.Position);
                                        if (distSq < densityRadiusSq && distSq > 0.01f)
                                        {
                                            neighbors++;
                                        }
                                    }
                                } while (spatialHash.TryGetNextValue(out neighbor, ref iterator));
                            }
                        }
                    }

                    particle.LocalDensity = neighbors;
                }).ScheduleParallel();
        }
    }
}
