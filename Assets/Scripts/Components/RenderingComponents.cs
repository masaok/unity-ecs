using Unity.Entities;

namespace CellularSeance.Components
{
    public enum ParticleStyle : byte
    {
        Circle,
        Hard,
        Core,
        Ringed,
        Square,
        TinySquare,
        Cross,
        Triangle,
        Ascii,
        Symbol,
        IChing,
        Rune,
        AstrologicalSymbol,
        InkStroke,
        TinyInkStroke,
        Protostar,
        Glitch,
        DatamoshBlock,
        ScanlineError,
        Random
    }

    public enum ParticleEffect : byte
    {
        None,
        Glow,
        SoftGlow,
        PulseGlow,
        AfterimageTrail,
        DirectionalCone,
        Translucent,
        Scanlines,
        ChromaticAberration,
        Mycelium,
        Lenticular
    }

    public enum BackgroundStyle : byte
    {
        Solid,
        Gradient,
        Grid,
        Rings,
        ParallaxGrid,
        ColdWarBoard,
        CrtGlow,
        InterferencePattern
    }

    public enum ViewerMode : byte
    {
        None,
        AetherDensity,
        DopplerShift
    }

    public struct CosmeticRulesComponent : IComponentData
    {
        public ParticleStyle Style;
        public ParticleEffect Effect;
        public BackgroundStyle Background;
        public ViewerMode Viewer;
        public int PaletteId;
    }
}
