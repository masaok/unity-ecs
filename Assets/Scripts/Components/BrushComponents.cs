using Unity.Entities;
using Unity.Mathematics;

namespace CellularSeance.Components
{
    public enum BrushMode : byte
    {
        // Aetheric Brush modes
        Stream,
        Spray,
        Pulse,
        Calligrapher,

        // Effect Brush modes
        Velocity,
        TypeMutator,
        Shrapnel,
        Eraser
    }

    public struct BrushSettingsComponent : IComponentData
    {
        public BrushMode Mode;
        public int SelectedTypeId;
        public float Size;
        public float Strength;
        public bool IsActive;
        public float2 Position;
        public float2 Direction;
    }
}
