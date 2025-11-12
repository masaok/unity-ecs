using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class ParticleRenderingSystem : SystemBase
    {
        private Material _particleMaterial;
        private Mesh _particleMesh;

        protected override void OnCreate()
        {
            base.OnCreate();
            CreateRenderingResources();
        }

        private void CreateRenderingResources()
        {
            // Create a simple quad mesh for particles
            _particleMesh = new Mesh();
            var vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, 0.5f, 0)
            };
            var triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            var uvs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            _particleMesh.vertices = vertices;
            _particleMesh.triangles = triangles;
            _particleMesh.uv = uvs;
            _particleMesh.RecalculateNormals();

            // Create material with a simple shader
            // Note: You'll need to create the actual shader asset in Unity
            _particleMaterial = new Material(Shader.Find("Unlit/Color"));
        }

        protected override void OnUpdate()
        {
            var cosmeticRules = SystemAPI.GetSingleton<CosmeticRulesComponent>();

            // For each particle, we'll use Graphics.DrawMeshInstanced in batches
            // In a full implementation, you'd want to use DrawMeshInstancedIndirect for better performance

            Entities
                .WithAll<ParticleTag>()
                .ForEach((in ParticleComponent particle) =>
                {
                    // Convert 2D position to 3D for rendering
                    var position = new Vector3(particle.Position.x, particle.Position.y, 0);
                    var rotation = Quaternion.identity;
                    var scale = Vector3.one * particle.Size;

                    var matrix = Matrix4x4.TRS(position, rotation, scale);

                    // Set material color
                    var color = new Color(particle.Color.x, particle.Color.y, particle.Color.z, particle.Color.w);
                    _particleMaterial.SetColor("_Color", color);

                    // Draw the particle
                    Graphics.DrawMesh(_particleMesh, matrix, _particleMaterial, 0);

                }).WithoutBurst().Run();
        }

        protected override void OnDestroy()
        {
            if (_particleMesh != null)
                Object.Destroy(_particleMesh);
            if (_particleMaterial != null)
                Object.Destroy(_particleMaterial);
            base.OnDestroy();
        }
    }
}
