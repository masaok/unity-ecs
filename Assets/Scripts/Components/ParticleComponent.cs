using Unity.Entities;
using Unity.Mathematics;

namespace CellularSeance.Components
{
    public enum ParticleState : byte
    {
        Gas,
        Liquid,
        Solid
    }

    public struct ParticleComponent : IComponentData
    {
        public int Id;
        public float2 Position;
        public float2 Velocity;
        public float Size;
        public float Mass;
        public int TypeId;
        public float Charge;
        public float Temperature;
        public float Aether;
        public float Resonance;
        public ParticleState State;
        public float LocalDensity;
        public int Age;
        public int Lifespan;
        public bool IsGhost;
        public bool IsOdd;
        public bool IsOddClone;
        public float4 Color; // RGBA
        public char Symbol;
    }

    public struct ParticleTypeComponent : ISharedComponentData
    {
        public int Id;
        public float BaseSize;
        public float MassFactor;
        public float ChargeFactor;
        public float2 TempRange;
        public float2 AetherRange;
        public float BaseLifespanFactor;
        public float Hue;
        public bool IsSpecial;
    }

    public struct ParticleTag : IComponentData { }
}
