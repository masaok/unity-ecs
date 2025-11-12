# Cellular Seance - Unity ECS Version

A recreation of the Cellular Seance particle physics simulation using Unity's Entity Component System (ECS) and Data-Oriented Technology Stack (DOTS).

## Overview

Cellular Seance is a sophisticated particle physics simulation featuring:
- **Complex particle interactions** with customizable behavior matrices
- **Multi-phase physics engine** with spatial hash optimization
- **30+ physics parameters** controlling emergent behaviors
- **22 standard anomalies** + 3 rare special anomalies
- **9 boundary geometries** (Rectangle, Circle, Torus, Ouroboros, etc.)
- **State systems** (Gas, Liquid, Solid) based on temperature and density
- **Interactive brush tools** for real-time particle manipulation
- **20 particle rendering styles** with multiple effects
- **80+ color palettes** for visual customization

## Architecture

### Core Components
- **ParticleComponent**: Main particle data (position, velocity, mass, charge, temperature, aether, etc.)
- **SpaceRulesComponent**: Global physics parameters (gravity, viscosity, vortex, etc.)
- **BoundaryDimensions**: World boundary configuration
- **CosmeticRulesComponent**: Rendering settings (styles, effects, palettes)
- **BrushSettingsComponent**: Interactive tool settings

### Main Systems

#### Physics Systems (Sequential)
1. **SpatialHashSystem**: Builds spatial grid for efficient neighbor queries
2. **LocalDensitySystem**: Calculates particle density for state determination
3. **StateUpdateSystem**: Updates particle states (gas/liquid/solid)
4. **EnvironmentalForcesSystem**: Applies global forces (gravity, drag, brownian motion, vortex)
5. **PairwiseInteractionSystem**: Handles particle-particle interactions
6. **PositionUpdateSystem**: Updates positions based on velocities
7. **BoundarySystem**: Handles collision with 9 different boundary types
8. **AgingAndDecaySystem**: Ages particles and handles lifespan/decay

#### Generation Systems
- **ParticleGenerationSystem**: Spawns particles and generates interaction rules
- **AnomalySystem**: Applies random anomalies to space rules

#### Rendering Systems
- **ParticleRenderingSystem**: Renders particles with various styles

#### Interaction Systems
- **BrushSystem**: Handles user input and particle painting

## Project Structure

```
Assets/
├── Scripts/
│   ├── Components/
│   │   ├── ParticleComponent.cs
│   │   ├── SpaceRulesComponent.cs
│   │   ├── RenderingComponents.cs
│   │   ├── AnomalyComponents.cs
│   │   └── BrushComponents.cs
│   ├── Systems/
│   │   ├── SpatialHashSystem.cs
│   │   ├── LocalDensitySystem.cs
│   │   ├── StateUpdateSystem.cs
│   │   ├── EnvironmentalForcesSystem.cs
│   │   ├── PairwiseInteractionSystem.cs
│   │   ├── PositionUpdateSystem.cs
│   │   ├── BoundarySystem.cs
│   │   ├── AgingAndDecaySystem.cs
│   │   ├── ParticleGenerationSystem.cs
│   │   ├── AnomalySystem.cs
│   │   ├── ParticleRenderingSystem.cs
│   │   └── BrushSystem.cs
│   ├── Data/
│   │   ├── InteractionRules.cs
│   │   └── ColorPalettes.cs
│   ├── Utilities/
│   │   └── MathUtilities.cs
│   ├── Authoring/
│   └── CellularSeanceBootstrap.cs
└── Shaders/
    └── ParticleShader.shader
```

## Setup Instructions

### Prerequisites
- Unity 2022.3 LTS or later
- Entities package (com.unity.entities)
- Burst package (com.unity.burst)
- Mathematics package (com.unity.mathematics)

### Installation
1. Create a new Unity project or open an existing one
2. Copy all files from this repository into your project
3. Install required packages via Package Manager:
   - Unity.Entities
   - Unity.Burst
   - Unity.Mathematics
   - Unity.Collections

### Running the Simulation
1. Create a new scene
2. Add a GameObject and attach the `CellularSeanceBootstrap` component
3. Configure the settings in the Inspector:
   - Initial Particle Count (default: 500)
   - Particle Type Count (default: 5)
   - Boundary Type (Circle, Square, Torus, etc.)
   - Anomaly Frequency (0% - 15%)
   - World dimensions
4. Add a Camera to the scene
5. Press Play

## Controls

- **Mouse Click**: Paint particles (hold to continuously paint)
- **1-5 Keys**: Switch brush modes
  - 1: Stream (continuous creation)
  - 2: Spray (scattered creation)
  - 3: Pulse (burst creation)
  - 4: Velocity (add force to particles)
  - 5: Eraser (remove particles)
- **R Key**: Regenerate universe with new rules

## Physics Parameters

### Space Rules
- **Vortex**: Spiral force toward center (0-0.04)
- **Viscosity**: Drag coefficient (0.8-1.0)
- **Gravity**: Downward force (0-0.1)
- **Brownian Motion**: Random jitter (0-60)
- **Background Charge**: Global electrical field (-0.5 to +0.5)
- **Elasticity**: Collision bounciness (0.4-0.8)
- **Ambient Temperature**: Global temperature (-50 to 200)
- **Ambient Aether**: Global energy level (0-100)

### Particle Properties
- **Position/Velocity**: 2D physics
- **Mass**: Calculated from size and type
- **Charge**: Electrical charge (-4 to +4)
- **Temperature**: Thermal energy (affects state)
- **Aether**: Mystical energy/stability metric
- **Resonance**: Frequency value (0-1)
- **State**: Gas, Liquid, or Solid
- **Lifespan**: Age before decay

### Interaction Behaviors
- **Attract**: Pull particles together
- **Repel**: Push particles apart
- **Mutate**: Change particle type
- **Merge**: Combine into larger particle
- **Shatter**: Break into fragments
- **Drain**: Transfer size/mass
- **Charge Transfer**: Equalize charges
- **Heat Transfer**: Equalize temperatures
- **Aether Siphon**: Transfer aether energy

## Anomalies

### Standard Anomalies (22 types)
- **Whirlpool**: Strong vortex force
- **Thick Aether**: High ambient aether
- **Charged Atmosphere**: Background electrical field
- **Universal Gravity**: Global downward force
- **Chaotic Energy**: High brownian motion
- **Predation**: Particles consume each other
- **Critical Mass**: Particles shatter when too large
- **Void Echoes**: Create ghost particles on death
- **Imperfect Collisions**: Lower elasticity
- **Spontaneous Apparition**: Random particle spawning
- **Inertia**: Mass affects movement
- **Cascading Destruction**: Chain reaction explosions
- **Thermal Shock**: Temperature changes cause damage
- **Absolute Zero**: Very low ambient temperature
- **Supernova**: Very high ambient temperature
- **Phase Volatility**: Unstable state changes
- **Aetheric Tide**: Strong aether flux
- **Resonant Harmony**: Attract similar frequencies
- **Dissonant Feedback**: Destroy clashing frequencies

### Special Anomalies (0.1% chance)
- **Strange Attractor**: Massive attraction force
- **Strange Repulsor**: Massive repulsion force
- **Odd Particle**: Freezing infection particle

## Boundary Types

1. **Vertical Rectangle**: 1:1.5 aspect ratio
2. **Horizontal Rectangle**: 1.5:1 aspect ratio
3. **Square**: 1:1 balanced
4. **Circle**: Circular boundary with normal reflection
5. **Torus**: Wrap-around edges (no walls)
6. **Ouroboros**: Circle with central pillar (donut shape)
7. **Pillar**: Rectangle with central obstacle
8. **Triangle**: Equilateral triangle boundary (simplified)
9. **Lemniscate**: Infinity symbol (∞) boundary (simplified)

## Performance

### Original JavaScript Version
- 500 particles @ 60fps
- 2000 particles @ 30fps

### Expected Unity ECS Performance
- 5,000+ particles @ 60fps (10x improvement)
- 20,000+ particles @ 30fps (10x improvement)
- Benefits from Burst compilation and Job System parallelization

## Future Enhancements

### Not Yet Implemented
- **Full particle styles**: Currently only basic circle rendering
- **Particle effects**: Glow, afterimage, mycelium, etc.
- **Background styles**: Grid, rings, parallax, etc.
- **Viewer modes**: Aether density, doppler shift visualizations
- **Audio reactivity**: Microphone analysis with 4 visualization modes
- **Contact behaviors**: Mutate, merge, shatter on collision
- **Seed system**: Save/load universe configurations
- **Advanced rendering**: 20 particle styles, 11 effects, 80+ palettes
- **Resonance system**: Frequency-based bonding and dissonance
- **Ghost particles**: Proper ghost spawning on decay
- **Chain reactions**: Explosion cascades
- **Thermal shock**: Temperature-based damage
- **State transitions**: Visual changes for gas/liquid/solid

### Planned Features
- GPU-accelerated rendering with DrawMeshInstancedIndirect
- Shader variants for all 20 particle styles
- Advanced color palette system with texture lookups
- Seed-based universe generation and sharing
- UI system for runtime parameter adjustment
- Audio visualizer integration
- Advanced contact behavior system
- Proper triangle and lemniscate boundary implementations

## Technical Details

### Spatial Hash Optimization
- Cell size: 150 pixels
- 3x3 neighborhood search
- O(n) complexity instead of O(n²)
- Dramatically improves performance with 500+ particles

### ECS Architecture Benefits
- **Data-oriented design**: Cache-friendly memory layout
- **Burst compilation**: SIMD optimization for math-heavy operations
- **Job system**: Parallel execution across CPU cores
- **Stateless systems**: Clean separation of logic and data
- **Deterministic**: Reproducible simulations with seeds

### Component-Based Rules
All behaviors emerge from combinations of:
- Particle types (5-20 types)
- Interaction rules (n² matrix of behaviors)
- Space rules (30+ physics parameters)
- Anomalies (random modifications to rules)

This creates thousands of unique scenarios from simple data combinations.

## Credits

Original Cellular Seance concept and implementation: HTML/JavaScript version
Unity ECS Recreation: Converted to Unity DOTS architecture

## License

This project is a recreation and adaptation of the original Cellular Seance simulation.
