using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cozy.Hexagons
{    
    using FromHexagonProjectionFunc = Func<Hexagon, float, (float x, float y)>;
    using ToHexagonProjectionFunc = Func<float, float, float, Hexagon>;
    using HexagonSpacingFunc = Func<float, (float xSpacing, float ySpacing)>;

    public static class HexagonMath
    {
        public const float Sqrt3 = 1.73205080757f;

        public static readonly (int x, int y)[] AxialNeighbors = new (int x, int y)[6]
        {
            (1, 0), (0, 1), (-1, 1),
            (-1, 0), (0, -1), (1, -1)
        };

        public static readonly Dictionary<HexagonOrientation, Dictionary<HexagonOffsetParity, (int q, int r)[]>> OffsetNeighbors = new()
        {
            {
                HexagonOrientation.PointyTop,
                new Dictionary<HexagonOffsetParity, (int q, int r)[]>
                {
                    {
                        HexagonOffsetParity.Even,
                        new (int q, int r)[6]
                        {
                            (1, 0), (0, -1), (-1, -1),
                            (-1, 0), (-1, 1), (0, 1)
                        }
                    },
                    {
                        HexagonOffsetParity.Odd,
                        new (int q, int r)[6]
                        {
                            (+1, 0), (+1, -1), (0, -1),
                            (-1, 0), (0, +1), (+1, +1)
                        }
                    }
                }
            },
            {
                HexagonOrientation.FlatTop,
                new Dictionary<HexagonOffsetParity, (int q, int r)[]>
                {
                    {
                        HexagonOffsetParity.Even,
                        new (int q, int r)[6]
                        {
                            (+1, 0), (+1, -1), (0, -1),
                            (-1, 0), (0, +1), (+1, +1)
                        }
                    },
                    {
                        HexagonOffsetParity.Odd,
                        new (int q, int r)[6]
                        {
                            (+1, 0), (0, -1), (-1, -1),
                            (-1, 0), (-1, +1), (0, +1)
                        }
                    }
                }
            }
        };

        public static readonly Dictionary<HexagonOrientation, float[]> Angles = new()
        {
            { HexagonOrientation.PointyTop, new float[6] {30f, 90f, 150f, 210f, 270f, 330f} },
            { HexagonOrientation.FlatTop, new float[6] {0f, 60f, 120f, 180f, 240f, 300f} },
        };

        public static readonly Dictionary<HexagonOrientation, HexagonSpacingFunc> Spacing = new()
        {
            {
                HexagonOrientation.PointyTop,
                (radius) => (Sqrt3 * radius, 1.5f * radius)
            },
            {
                HexagonOrientation.FlatTop,
                (radius) => (1.5f * radius, Sqrt3 * radius)
            }
        };

        public static readonly Dictionary<HexagonOrientation, FromHexagonProjectionFunc> FromHex = new()
        {
            {
                HexagonOrientation.PointyTop,
                (hexagon, radius) =>
                {
                    float x = radius * Sqrt3 * (hexagon.Q + hexagon.R / 2f);
                    float y = radius * 1.5f * hexagon.R;
                    return (x, y);
                }
            },
            {
                HexagonOrientation.FlatTop,
                (hexagon, radius) =>
                {
                    float x = radius * 1.5f * hexagon.Q;
                    float y = radius * Sqrt3 * (hexagon.R + hexagon.Q / 2f);
                    return (x, y);
                }
            }
        };

        public static readonly Dictionary<HexagonOrientation, ToHexagonProjectionFunc> ToHex = new()
        {
            {
                HexagonOrientation.PointyTop,
                (x, y, radius) =>
                {
                    float q = (Sqrt3 / 3f * x - 1f / 3f * y) / radius;
                    float r = 2f / 3f * y / radius;
                    return RoundHex(q, r, -q - r);
                }
            },
            {
                HexagonOrientation.FlatTop,
                (x, y, radius) =>
                {
                    float q = 2f / 3f * x / radius;
                    float r = (Sqrt3 / 3f * y - 1f / 3f * x) / radius;
                    return RoundHex(q, r, -q - r);
                }
            }
        };

        public static Hexagon RoundHex(float q, float r, float s)
        {
            int rq = (int)MathF.Round(q);
            int rr = (int)MathF.Round(r);
            int rs = (int)MathF.Round(s);

            float qDiff = MathF.Abs(rq - q);
            float rDiff = MathF.Abs(rr - r);
            float sDiff = MathF.Abs(rs - s);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                rq = -rr - rs;
            }
            else if (rDiff > sDiff)
            {
                rr = -rq - rs;
            }

            return new Hexagon(rq, rr);
        }

        public static (float x, float y) GetCorner(float radius, int corner, HexagonOrientation orientation)
        {
            if (corner < 0) corner = 0;
            if (corner > 5) corner = 5;

            float angleDeg = Angles[orientation][corner];
            float angleRad = Mathf.Deg2Rad * angleDeg;
            float x = radius * Mathf.Cos(angleRad);
            float y = radius * Mathf.Sin(angleRad);

            return (x, y);
        }
    }
}