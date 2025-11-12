using Unity.Mathematics;

namespace CellularSeance.Utilities
{
    public static class MathUtilities
    {
        public static float3 HsvToRgb(float h, float s, float v)
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

        public static float3 RgbToHsv(float3 rgb)
        {
            float max = math.max(rgb.x, math.max(rgb.y, rgb.z));
            float min = math.min(rgb.x, math.min(rgb.y, rgb.z));
            float delta = max - min;

            float h = 0;
            if (delta > 0)
            {
                if (max == rgb.x)
                    h = ((rgb.y - rgb.z) / delta) % 6;
                else if (max == rgb.y)
                    h = (rgb.z - rgb.x) / delta + 2;
                else
                    h = (rgb.x - rgb.y) / delta + 4;

                h /= 6;
                if (h < 0) h += 1;
            }

            float s = max > 0 ? delta / max : 0;
            float v = max;

            return new float3(h, s, v);
        }

        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }

        public static float SmoothStep(float edge0, float edge1, float x)
        {
            x = math.clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            return x * x * (3.0f - 2.0f * x);
        }
    }
}
