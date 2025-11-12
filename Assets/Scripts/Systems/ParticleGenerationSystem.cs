using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using CellularSeance.Components;
using CellularSeance.Data;
using UnityEngine;

namespace CellularSeance.Systems
{
    public partial class ParticleGenerationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        private Unity.Mathematics.Random _random;
        private int _nextParticleId = 0;

        protected override void OnCreate()
        {
            base.OnCreate();
            _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            _random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        }

        protected override void OnUpdate()
        {
            // This system is manually triggered, not automatically updated
        }

        public Entity SpawnParticle(float2 position, int typeId, EntityCommandBuffer ecb = null)
        {
            bool disposeEcb = false;
            if (ecb == null)
            {
                ecb = _ecbSystem.CreateCommandBuffer();
                disposeEcb = true;
            }

            var entity = ecb.CreateEntity();

            var particle = new ParticleComponent
            {
                Id = _nextParticleId++,
                Position = position,
                Velocity = float2.zero,
                Size = _random.NextFloat(6f, 15f),
                TypeId = typeId,
                Charge = _random.NextFloat(-2f, 2f),
                Temperature = _random.NextFloat(50f, 150f),
                Aether = _random.NextFloat(30f, 70f),
                Resonance = _random.NextFloat(0f, 1f),
                State = ParticleState.Liquid,
                LocalDensity = 0,
                Age = 0,
                Lifespan = _random.NextInt(1000, 5000),
                IsGhost = false,
                IsOdd = false,
                IsOddClone = false,
                Color = GetRandomColor(typeId),
                Symbol = (char)_random.NextInt(33, 126)
            };

            particle.Mass = particle.Size * 0.5f;

            ecb.AddComponent(entity, particle);
            ecb.AddComponent(entity, new ParticleTag());

            return entity;
        }

        public void SpawnParticles(int count, BoundaryDimensions bounds)
        {
            var ecb = _ecbSystem.CreateCommandBuffer();

            for (int i = 0; i < count; i++)
            {
                float x = _random.NextFloat(bounds.CenterX - bounds.Width / 2 + 20, bounds.CenterX + bounds.Width / 2 - 20);
                float y = _random.NextFloat(bounds.CenterY - bounds.Height / 2 + 20, bounds.CenterY + bounds.Height / 2 - 20);
                int typeId = _random.NextInt(0, 5); // Assuming 5 particle types

                SpawnParticle(new float2(x, y), typeId, ecb);
            }
        }

        private float4 GetRandomColor(int typeId)
        {
            // Simple color palette based on type
            float hue = (typeId * 0.618034f) % 1.0f; // Golden angle
            return new float4(HsvToRgb(hue, 0.7f, 0.9f), 1.0f);
        }

        private float3 HsvToRgb(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - math.abs((h * 6) % 2 - 1));
            float m = v - c;

            float3 rgb;
            if (h < 1f / 6f)
                rgb = new float3(c, x, 0);
            else if (h < 2f / 6f)
                rgb = new float3(x, c, 0);
            else if (h < 3f / 6f)
                rgb = new float3(0, c, x);
            else if (h < 4f / 6f)
                rgb = new float3(0, x, c);
            else if (h < 5f / 6f)
                rgb = new float3(x, 0, c);
            else
                rgb = new float3(c, 0, x);

            return rgb + m;
        }

        public InteractionRulesData GenerateInteractionRules(int typeCount)
        {
            var rulesData = new InteractionRulesData
            {
                Rules = new NativeArray2D<InteractionRule>(typeCount, typeCount, Allocator.Persistent)
            };

            for (int i = 0; i < typeCount; i++)
            {
                for (int j = 0; j < typeCount; j++)
                {
                    var rule = new InteractionRule
                    {
                        Behavior = GetRandomBehavior(),
                        Force = _random.NextFloat(0.1f, 2.0f),
                        Range = _random.NextFloat(50f, 200f)
                    };

                    rulesData.Rules[i, j] = rule;
                }
            }

            return rulesData;
        }

        private BehaviorType GetRandomBehavior()
        {
            float rand = _random.NextFloat();
            if (rand < 0.3f) return BehaviorType.Attract;
            else if (rand < 0.5f) return BehaviorType.Repel;
            else if (rand < 0.6f) return BehaviorType.Mutate;
            else if (rand < 0.7f) return BehaviorType.HeatTransfer;
            else if (rand < 0.8f) return BehaviorType.ChargeTransfer;
            else return BehaviorType.Ignore;
        }
    }
}
