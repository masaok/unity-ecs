using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using CellularSeance.Components;
using CellularSeance.Systems;
using CellularSeance.Data;

namespace CellularSeance
{
    /// <summary>
    /// Main bootstrap for Cellular Seance - Unity ECS Version
    /// This recreates the particle physics simulation from the original HTML/JavaScript version
    /// </summary>
    public class CellularSeanceBootstrap : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [SerializeField] private int initialParticleCount = 500;
        [SerializeField] private int particleTypeCount = 5;
        [SerializeField] private BoundaryType boundaryType = BoundaryType.Circle;
        [SerializeField] private float anomalyFrequency = 0.05f;

        [Header("World Dimensions")]
        [SerializeField] private float worldWidth = 1920f;
        [SerializeField] private float worldHeight = 1080f;

        [Header("Color Palette")]
        [SerializeField] private string paletteName = "neon_primary";

        private EntityManager _entityManager;
        private ParticleGenerationSystem _generationSystem;
        private PairwiseInteractionSystem _interactionSystem;
        private AnomalySystem _anomalySystem;

        private Entity _spaceRulesEntity;
        private Entity _boundaryEntity;
        private Entity _cosmeticRulesEntity;
        private Entity _brushSettingsEntity;

        private void Start()
        {
            InitializeECS();
            GenerateUniverse();
        }

        private void InitializeECS()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Get or create systems
            _generationSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ParticleGenerationSystem>();
            _interactionSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PairwiseInteractionSystem>();
            _anomalySystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<AnomalySystem>();

            Debug.Log("Cellular Seance ECS Initialized");
        }

        private void GenerateUniverse()
        {
            Debug.Log("Generating new universe...");

            // Create space rules
            CreateSpaceRules();

            // Create boundary dimensions
            CreateBoundaryDimensions();

            // Create cosmetic rules
            CreateCosmeticRules();

            // Create brush settings
            CreateBrushSettings();

            // Generate interaction rules
            var interactionRules = _generationSystem.GenerateInteractionRules(particleTypeCount);
            _interactionSystem.InteractionRules = interactionRules;

            // Apply anomalies
            var spaceRules = _entityManager.GetComponentData<SpaceRulesComponent>(_spaceRulesEntity);
            _anomalySystem.ApplyRandomAnomaly(ref spaceRules, anomalyFrequency);
            _entityManager.SetComponentData(_spaceRulesEntity, spaceRules);

            // Spawn initial particles
            var boundaries = _entityManager.GetComponentData<BoundaryDimensions>(_boundaryEntity);
            _generationSystem.SpawnParticles(initialParticleCount, boundaries);

            Debug.Log($"Universe generated with {initialParticleCount} particles and {particleTypeCount} types");
        }

        private void CreateSpaceRules()
        {
            _spaceRulesEntity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(_spaceRulesEntity, new SpaceRulesComponent
            {
                Vortex = 0f,
                Viscosity = 0.95f,
                Boundary = boundaryType,
                BackgroundCharge = 0f,
                GravityY = 0f,
                BrownianMotion = 10f,
                Elasticity = 0.7f,
                SpontaneousGeneration = 0f,
                Predation = false,
                CriticalMass = float.PositiveInfinity,
                DecayRate = 0f,
                CreateGhosts = false,
                Inertia = false,
                ChainReaction = false,
                ThermalShock = false,
                AmbientTemperature = 100f,
                HeatDissipation = 0.001f,
                StateChangeFactor = 1.0f,
                AmbientAether = 50f,
                AetherFlux = 0.002f,
                ResonantBondingThreshold = 0.1f,
                ResonantBondingStrength = 0.05f,
                DissonanceThreshold = 0.85f,
                EnableResonanceBonding = false,
                EnableDissonance = false
            });
        }

        private void CreateBoundaryDimensions()
        {
            _boundaryEntity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(_boundaryEntity, new BoundaryDimensions
            {
                Width = worldWidth,
                Height = worldHeight,
                CenterX = 0f,
                CenterY = 0f
            });
        }

        private void CreateCosmeticRules()
        {
            _cosmeticRulesEntity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(_cosmeticRulesEntity, new CosmeticRulesComponent
            {
                Style = ParticleStyle.Circle,
                Effect = ParticleEffect.Glow,
                Background = BackgroundStyle.Solid,
                Viewer = ViewerMode.None,
                PaletteId = 0
            });
        }

        private void CreateBrushSettings()
        {
            _brushSettingsEntity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(_brushSettingsEntity, new BrushSettingsComponent
            {
                Mode = BrushMode.Stream,
                SelectedTypeId = 0,
                Size = 30f,
                Strength = 1.0f,
                IsActive = false,
                Position = float2.zero,
                Direction = float2.zero
            });
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            var brushSettings = _entityManager.GetComponentData<BrushSettingsComponent>(_brushSettingsEntity);

            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                brushSettings.IsActive = true;
                brushSettings.Position = new float2(mousePos.x, mousePos.y);
                brushSettings.Direction = new float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
            else
            {
                brushSettings.IsActive = false;
            }

            // Switch brush modes
            if (Input.GetKeyDown(KeyCode.Alpha1))
                brushSettings.Mode = BrushMode.Stream;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                brushSettings.Mode = BrushMode.Spray;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                brushSettings.Mode = BrushMode.Pulse;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                brushSettings.Mode = BrushMode.Velocity;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                brushSettings.Mode = BrushMode.Eraser;

            // Regenerate universe
            if (Input.GetKeyDown(KeyCode.R))
            {
                ClearParticles();
                GenerateUniverse();
            }

            _entityManager.SetComponentData(_brushSettingsEntity, brushSettings);
        }

        private void ClearParticles()
        {
            var query = _entityManager.CreateEntityQuery(typeof(ParticleTag));
            _entityManager.DestroyEntity(query);
        }

        private void OnDestroy()
        {
            // Cleanup
            if (_interactionSystem != null && _interactionSystem.InteractionRules.Rules.data.IsCreated)
            {
                _interactionSystem.InteractionRules.Rules.Dispose();
            }
        }

        private void OnGUI()
        {
            var query = _entityManager.CreateEntityQuery(typeof(ParticleTag));
            int particleCount = query.CalculateEntityCount();

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Cellular Seance v.ECS", new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold });
            GUILayout.Label($"Particles: {particleCount}");
            GUILayout.Label($"Boundary: {boundaryType}");

            var brushSettings = _entityManager.GetComponentData<BrushSettingsComponent>(_brushSettingsEntity);
            GUILayout.Label($"Brush Mode: {brushSettings.Mode}");

            GUILayout.Space(10);
            GUILayout.Label("Controls:");
            GUILayout.Label("1-5: Switch brush modes");
            GUILayout.Label("R: Regenerate universe");
            GUILayout.Label("Mouse: Paint particles");

            GUILayout.EndArea();
        }
    }
}
