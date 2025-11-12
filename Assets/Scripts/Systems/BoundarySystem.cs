using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using CellularSeance.Components;

namespace CellularSeance.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    public partial class BoundarySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var spaceRules = SystemAPI.GetSingleton<SpaceRulesComponent>();
            var boundaries = SystemAPI.GetSingleton<BoundaryDimensions>();

            Entities
                .WithAll<ParticleTag>()
                .ForEach((ref ParticleComponent particle) =>
                {
                    switch (spaceRules.Boundary)
                    {
                        case BoundaryType.VerticalRectangle:
                        case BoundaryType.HorizontalRectangle:
                        case BoundaryType.Square:
                            HandleRectangleBoundary(ref particle, boundaries, spaceRules.Elasticity);
                            break;

                        case BoundaryType.Circle:
                            HandleCircleBoundary(ref particle, boundaries, spaceRules.Elasticity);
                            break;

                        case BoundaryType.Torus:
                            HandleTorusBoundary(ref particle, boundaries);
                            break;

                        case BoundaryType.Ouroboros:
                            HandleOuroborosBoundary(ref particle, boundaries, spaceRules.Elasticity);
                            break;

                        case BoundaryType.Pillar:
                            HandlePillarBoundary(ref particle, boundaries, spaceRules.Elasticity);
                            break;

                        case BoundaryType.Triangle:
                            HandleTriangleBoundary(ref particle, boundaries, spaceRules.Elasticity);
                            break;

                        case BoundaryType.Lemniscate:
                            HandleLemniscateBoundary(ref particle, boundaries, spaceRules.Elasticity);
                            break;
                    }
                }).ScheduleParallel();
        }

        private static void HandleRectangleBoundary(ref ParticleComponent particle, BoundaryDimensions bounds, float elasticity)
        {
            float left = bounds.CenterX - bounds.Width / 2;
            float right = bounds.CenterX + bounds.Width / 2;
            float top = bounds.CenterY - bounds.Height / 2;
            float bottom = bounds.CenterY + bounds.Height / 2;

            if (particle.Position.x - particle.Size < left)
            {
                particle.Position.x = left + particle.Size;
                particle.Velocity.x *= -elasticity;
            }
            else if (particle.Position.x + particle.Size > right)
            {
                particle.Position.x = right - particle.Size;
                particle.Velocity.x *= -elasticity;
            }

            if (particle.Position.y - particle.Size < top)
            {
                particle.Position.y = top + particle.Size;
                particle.Velocity.y *= -elasticity;
            }
            else if (particle.Position.y + particle.Size > bottom)
            {
                particle.Position.y = bottom - particle.Size;
                particle.Velocity.y *= -elasticity;
            }
        }

        private static void HandleCircleBoundary(ref ParticleComponent particle, BoundaryDimensions bounds, float elasticity)
        {
            float2 center = new float2(bounds.CenterX, bounds.CenterY);
            float radius = math.min(bounds.Width, bounds.Height) / 2;
            float2 toCenter = center - particle.Position;
            float dist = math.length(toCenter);
            float maxDist = radius - particle.Size;

            if (dist > maxDist)
            {
                float2 normal = toCenter / dist;
                particle.Position = center - normal * maxDist;

                // Reflect velocity
                float2 normalVel = math.dot(particle.Velocity, normal) * normal;
                float2 tangentVel = particle.Velocity - normalVel;
                particle.Velocity = tangentVel - normalVel * elasticity;
            }
        }

        private static void HandleTorusBoundary(ref ParticleComponent particle, BoundaryDimensions bounds)
        {
            float left = bounds.CenterX - bounds.Width / 2;
            float right = bounds.CenterX + bounds.Width / 2;
            float top = bounds.CenterY - bounds.Height / 2;
            float bottom = bounds.CenterY + bounds.Height / 2;

            // Wrap around
            if (particle.Position.x < left)
                particle.Position.x = right - (left - particle.Position.x);
            else if (particle.Position.x > right)
                particle.Position.x = left + (particle.Position.x - right);

            if (particle.Position.y < top)
                particle.Position.y = bottom - (top - particle.Position.y);
            else if (particle.Position.y > bottom)
                particle.Position.y = top + (particle.Position.y - bottom);
        }

        private static void HandleOuroborosBoundary(ref ParticleComponent particle, BoundaryDimensions bounds, float elasticity)
        {
            // Outer circle boundary
            HandleCircleBoundary(ref particle, bounds, elasticity);

            // Inner circle (pillar)
            float2 center = new float2(bounds.CenterX, bounds.CenterY);
            float innerRadius = math.min(bounds.Width, bounds.Height) / 6;
            float2 toCenter = particle.Position - center;
            float dist = math.length(toCenter);

            if (dist < innerRadius + particle.Size)
            {
                float2 normal = toCenter / math.max(dist, 0.001f);
                particle.Position = center + normal * (innerRadius + particle.Size);

                // Reflect velocity
                float2 normalVel = math.dot(particle.Velocity, normal) * normal;
                float2 tangentVel = particle.Velocity - normalVel;
                particle.Velocity = tangentVel - normalVel * elasticity;
            }
        }

        private static void HandlePillarBoundary(ref ParticleComponent particle, BoundaryDimensions bounds, float elasticity)
        {
            // Outer rectangle boundary
            HandleRectangleBoundary(ref particle, bounds, elasticity);

            // Inner rectangle (pillar)
            float pillarWidth = bounds.Width / 4;
            float pillarHeight = bounds.Height / 4;
            float left = bounds.CenterX - pillarWidth / 2;
            float right = bounds.CenterX + pillarWidth / 2;
            float top = bounds.CenterY - pillarHeight / 2;
            float bottom = bounds.CenterY + pillarHeight / 2;

            // Check if inside pillar and push out
            if (particle.Position.x > left && particle.Position.x < right &&
                particle.Position.y > top && particle.Position.y < bottom)
            {
                // Find closest edge
                float distLeft = particle.Position.x - left;
                float distRight = right - particle.Position.x;
                float distTop = particle.Position.y - top;
                float distBottom = bottom - particle.Position.y;

                float minDist = math.min(math.min(distLeft, distRight), math.min(distTop, distBottom));

                if (minDist == distLeft)
                {
                    particle.Position.x = left - particle.Size;
                    particle.Velocity.x *= -elasticity;
                }
                else if (minDist == distRight)
                {
                    particle.Position.x = right + particle.Size;
                    particle.Velocity.x *= -elasticity;
                }
                else if (minDist == distTop)
                {
                    particle.Position.y = top - particle.Size;
                    particle.Velocity.y *= -elasticity;
                }
                else
                {
                    particle.Position.y = bottom + particle.Size;
                    particle.Velocity.y *= -elasticity;
                }
            }
        }

        private static void HandleTriangleBoundary(ref ParticleComponent particle, BoundaryDimensions bounds, float elasticity)
        {
            // Equilateral triangle - simplified implementation
            // For now, fall back to circle boundary
            HandleCircleBoundary(ref particle, bounds, elasticity);
        }

        private static void HandleLemniscateBoundary(ref ParticleComponent particle, BoundaryDimensions bounds, float elasticity)
        {
            // Infinity symbol - simplified implementation
            // For now, fall back to two circles
            float2 center = new float2(bounds.CenterX, bounds.CenterY);
            float radius = bounds.Width / 4;
            float offset = bounds.Width / 4;

            float2 leftCenter = center + new float2(-offset, 0);
            float2 rightCenter = center + new float2(offset, 0);

            float distLeft = math.distance(particle.Position, leftCenter);
            float distRight = math.distance(particle.Position, rightCenter);

            // Use closest circle
            if (distLeft < distRight)
            {
                var tempBounds = bounds;
                tempBounds.CenterX = leftCenter.x;
                tempBounds.CenterY = leftCenter.y;
                tempBounds.Width = radius * 2;
                tempBounds.Height = radius * 2;
                HandleCircleBoundary(ref particle, tempBounds, elasticity);
            }
            else
            {
                var tempBounds = bounds;
                tempBounds.CenterX = rightCenter.x;
                tempBounds.CenterY = rightCenter.y;
                tempBounds.Width = radius * 2;
                tempBounds.Height = radius * 2;
                HandleCircleBoundary(ref particle, tempBounds, elasticity);
            }
        }
    }
}
