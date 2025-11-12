using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public partial class SpatialHashSystem : SystemBase
    {
        public const float CellSize = 150f;
        public NativeMultiHashMap<int, Entity> SpatialHash;

        protected override void OnCreate()
        {
            base.OnCreate();
            SpatialHash = new NativeMultiHashMap<int, Entity>(1000, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            if (SpatialHash.IsCreated)
                SpatialHash.Dispose();
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            // Clear the spatial hash
            SpatialHash.Clear();

            // Count particles for capacity
            var particleCount = 0;
            Entities.WithAll<ParticleTag>().ForEach((in ParticleComponent particle) =>
            {
                particleCount++;
            }).WithoutBurst().Run();

            // Ensure capacity
            if (SpatialHash.Capacity < particleCount * 3)
            {
                SpatialHash.Capacity = particleCount * 3;
            }

            // Build spatial hash
            var spatialHash = SpatialHash.AsParallelWriter();

            Entities
                .WithAll<ParticleTag>()
                .ForEach((Entity entity, in ParticleComponent particle) =>
                {
                    int hash = GetSpatialHash(particle.Position);
                    spatialHash.Add(hash, entity);
                }).ScheduleParallel();

            CompleteDependency();
        }

        public static int GetSpatialHash(float2 position)
        {
            int x = (int)math.floor(position.x / CellSize);
            int y = (int)math.floor(position.y / CellSize);
            return HashPosition(x, y);
        }

        public static int HashPosition(int x, int y)
        {
            // Simple hash function
            return x * 73856093 ^ y * 19349663;
        }

        public NativeList<Entity> GetNeighbors(float2 position, Allocator allocator)
        {
            var neighbors = new NativeList<Entity>(allocator);
            int centerX = (int)math.floor(position.x / CellSize);
            int centerY = (int)math.floor(position.y / CellSize);

            // Check 3x3 grid of cells
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int hash = HashPosition(centerX + dx, centerY + dy);
                    if (SpatialHash.TryGetFirstValue(hash, out var entity, out var iterator))
                    {
                        do
                        {
                            neighbors.Add(entity);
                        } while (SpatialHash.TryGetNextValue(out entity, ref iterator));
                    }
                }
            }

            return neighbors;
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PhysicsSystemGroup : ComponentSystemGroup { }
}
