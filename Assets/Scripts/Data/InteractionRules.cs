using Unity.Collections;
using Unity.Mathematics;

namespace CellularSeance.Data
{
    public enum BehaviorType : byte
    {
        Attract,
        Repel,
        Mutate,
        Merge,
        Shatter,
        Drain,
        Ignore,
        ChargeTransfer,
        HeatTransfer,
        AetherSiphon
    }

    public struct InteractionRule
    {
        public BehaviorType Behavior;
        public float Force;
        public float Range;
    }

    public struct InteractionRulesData
    {
        public NativeArray2D<InteractionRule> Rules; // [typeA][typeB]

        public InteractionRule GetRule(int typeA, int typeB)
        {
            return Rules[typeA, typeB];
        }
    }

    // Helper struct for 2D native array
    public struct NativeArray2D<T> where T : struct
    {
        private NativeArray<T> data;
        public readonly int Width;
        public readonly int Height;

        public NativeArray2D(int width, int height, Allocator allocator)
        {
            Width = width;
            Height = height;
            data = new NativeArray<T>(width * height, allocator);
        }

        public T this[int x, int y]
        {
            get => data[y * Width + x];
            set => data[y * Width + x] = value;
        }

        public void Dispose()
        {
            if (data.IsCreated)
                data.Dispose();
        }
    }
}
