using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(LocalDensitySystem))]
    public partial class StateUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var spaceRules = SystemAPI.GetSingleton<SpaceRulesComponent>();

            Entities
                .WithAll<ParticleTag>()
                .ForEach((ref ParticleComponent particle) =>
                {
                    // Determine state based on temperature and density
                    const float highTempThreshold = 150f;
                    const float lowTempThreshold = 50f;
                    const float highDensityThreshold = 10f;
                    const float lowDensityThreshold = 3f;

                    if (particle.Temperature > highTempThreshold || particle.LocalDensity < lowDensityThreshold)
                    {
                        particle.State = ParticleState.Gas;
                    }
                    else if (particle.Temperature < lowTempThreshold && particle.LocalDensity > highDensityThreshold)
                    {
                        particle.State = ParticleState.Solid;
                    }
                    else
                    {
                        particle.State = ParticleState.Liquid;
                    }
                }).ScheduleParallel();
        }
    }
}
