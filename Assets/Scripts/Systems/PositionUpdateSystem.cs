using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PairwiseInteractionSystem))]
    [UpdateBefore(typeof(BoundarySystem))]
    public partial class PositionUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            Entities
                .WithAll<ParticleTag>()
                .ForEach((ref ParticleComponent particle) =>
                {
                    // Update position based on velocity
                    particle.Position += particle.Velocity * deltaTime * 60f; // 60x for visibility

                    // Age particle
                    particle.Age++;

                }).ScheduleParallel();
        }
    }
}
