using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using CellularSeance.Components;
using UnityEngine;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BrushSystem : SystemBase
    {
        private ParticleGenerationSystem _generationSystem;
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        private float _lastSpawnTime;

        protected override void OnCreate()
        {
            base.OnCreate();
            _generationSystem = World.GetOrCreateSystemManaged<ParticleGenerationSystem>();
            _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var brushSettings = SystemAPI.GetSingleton<BrushSettingsComponent>();

            if (!brushSettings.IsActive)
                return;

            var ecb = _ecbSystem.CreateCommandBuffer();
            var deltaTime = SystemAPI.Time.DeltaTime;

            switch (brushSettings.Mode)
            {
                case BrushMode.Stream:
                    ApplyStreamBrush(brushSettings, ecb);
                    break;

                case BrushMode.Spray:
                    ApplySprayBrush(brushSettings, ecb);
                    break;

                case BrushMode.Pulse:
                    ApplyPulseBrush(brushSettings, ecb);
                    break;

                case BrushMode.Velocity:
                    ApplyVelocityBrush(brushSettings);
                    break;

                case BrushMode.Eraser:
                    ApplyEraserBrush(brushSettings, ecb);
                    break;
            }
        }

        private void ApplyStreamBrush(BrushSettingsComponent brush, EntityCommandBuffer ecb)
        {
            // Spawn particles continuously
            if (Time.time - _lastSpawnTime > 0.05f)
            {
                for (int i = 0; i < (int)brush.Strength; i++)
                {
                    _generationSystem.SpawnParticle(brush.Position, brush.SelectedTypeId, ecb);
                }
                _lastSpawnTime = Time.time;
            }
        }

        private void ApplySprayBrush(BrushSettingsComponent brush, EntityCommandBuffer ecb)
        {
            // Spawn scattered particles
            if (Time.time - _lastSpawnTime > 0.1f)
            {
                var random = new Unity.Mathematics.Random((uint)Time.frameCount);
                for (int i = 0; i < (int)brush.Strength; i++)
                {
                    float2 offset = random.NextFloat2(-brush.Size, brush.Size);
                    _generationSystem.SpawnParticle(brush.Position + offset, brush.SelectedTypeId, ecb);
                }
                _lastSpawnTime = Time.time;
            }
        }

        private void ApplyPulseBrush(BrushSettingsComponent brush, EntityCommandBuffer ecb)
        {
            // Burst creation on click
            var random = new Unity.Mathematics.Random((uint)Time.frameCount);
            for (int i = 0; i < (int)(brush.Strength * 10); i++)
            {
                float angle = random.NextFloat(0, 2 * math.PI);
                float dist = random.NextFloat(0, brush.Size);
                float2 offset = new float2(math.cos(angle), math.sin(angle)) * dist;
                _generationSystem.SpawnParticle(brush.Position + offset, brush.SelectedTypeId, ecb);
            }
        }

        private void ApplyVelocityBrush(BrushSettingsComponent brush)
        {
            // Add force to particles within radius
            Entities
                .WithAll<ParticleTag>()
                .ForEach((ref ParticleComponent particle) =>
                {
                    float dist = math.distance(particle.Position, brush.Position);
                    if (dist < brush.Size)
                    {
                        float strength = (1.0f - dist / brush.Size) * brush.Strength;
                        particle.Velocity += brush.Direction * strength;
                    }
                }).ScheduleParallel();
        }

        private void ApplyEraserBrush(BrushSettingsComponent brush, EntityCommandBuffer ecb)
        {
            // Remove particles within radius
            var ecbParallel = ecb.AsParallelWriter();

            Entities
                .WithAll<ParticleTag>()
                .ForEach((Entity entity, int entityInQueryIndex, in ParticleComponent particle) =>
                {
                    float dist = math.distance(particle.Position, brush.Position);
                    if (dist < brush.Size)
                    {
                        ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();
        }
    }
}
