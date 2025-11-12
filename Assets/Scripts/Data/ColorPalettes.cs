using Unity.Mathematics;

namespace CellularSeance.Data
{
    public static class ColorPalettes
    {
        public static readonly string[] PaletteNames = new[]
        {
            "neon_primary",
            "pastel_dream",
            "deep_ocean",
            "volcanic",
            "forest_moss",
            "cyberpunk",
            "sunset",
            "aurora",
            "monochrome",
            "random_colors"
        };

        public static float4 GetColor(string paletteName, int index, int totalColors)
        {
            float t = (float)index / math.max(1, totalColors - 1);

            switch (paletteName)
            {
                case "neon_primary":
                    return LerpColor(
                        new float4(1, 0, 1, 1), // Magenta
                        new float4(0, 1, 1, 1), // Cyan
                        t
                    );

                case "pastel_dream":
                    return LerpColor(
                        new float4(1, 0.7f, 0.8f, 1), // Pink
                        new float4(0.7f, 0.9f, 1, 1), // Light blue
                        t
                    );

                case "deep_ocean":
                    return LerpColor(
                        new float4(0, 0.2f, 0.4f, 1), // Deep blue
                        new float4(0, 0.6f, 0.8f, 1), // Light blue
                        t
                    );

                case "volcanic":
                    return LerpColor(
                        new float4(0.8f, 0.2f, 0, 1), // Red
                        new float4(1, 0.6f, 0, 1), // Orange
                        t
                    );

                case "forest_moss":
                    return LerpColor(
                        new float4(0.2f, 0.4f, 0.1f, 1), // Dark green
                        new float4(0.5f, 0.8f, 0.3f, 1), // Light green
                        t
                    );

                case "cyberpunk":
                    return LerpColor(
                        new float4(1, 0, 0.5f, 1), // Hot pink
                        new float4(0, 1, 1, 1), // Cyan
                        t
                    );

                case "sunset":
                    return LerpColor(
                        new float4(1, 0.3f, 0, 1), // Orange
                        new float4(0.6f, 0, 0.8f, 1), // Purple
                        t
                    );

                case "aurora":
                    return LerpColor(
                        new float4(0, 1, 0.5f, 1), // Green
                        new float4(0.5f, 0, 1, 1), // Purple
                        t
                    );

                case "monochrome":
                    float gray = 0.3f + t * 0.6f;
                    return new float4(gray, gray, gray, 1);

                case "random_colors":
                default:
                    // Golden angle color distribution
                    float hue = (index * 0.618034f) % 1.0f;
                    return new float4(HsvToRgb(hue, 0.7f, 0.9f), 1.0f);
            }
        }

        private static float4 LerpColor(float4 a, float4 b, float t)
        {
            return math.lerp(a, b, t);
        }

        private static float3 HsvToRgb(float h, float s, float v)
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
    }
}
