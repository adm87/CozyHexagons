using System;
using System.Collections.Generic;

namespace Cozy.Hexagons
{    
    /// <summary>
    /// HexagonMath provides mathematical functions and constants for working with hexagonal grids.
    /// </summary>
    public static class HexagonMath
    {
        public const float Deg2Rad = (float)(Math.PI / 180.0);
        public const float Rad2Deg = (float)(180.0 / Math.PI);

        /// <summary>
        /// Sqrt3 is the square root of 3, approximately 1.732.
        /// </summary>
        public const float Sqrt3 = 1.73205080757f;

        /// <summary>
        /// HalfSqrt3 is half of the square root of 3, approximately 0.866.
        /// </summary>
        public readonly static float HalfSqrt3 = 0.5f * Sqrt3;

        /// <summary>
        /// OffsetNeighbors defines the six neighboring hexagons in offset coordinates, based on orientation and parity.
        /// </summary>
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
                            ( 1, 0), (0, -1), (-1, -1),
                            (-1, 0), (-1, 1), ( 0,  1)
                        }
                    },
                    {
                        HexagonOffsetParity.Odd,
                        new (int q, int r)[6]
                        {
                            ( 1, 0), (1, -1), (0, -1),
                            (-1, 0), (0,  1), (1,  1)
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
                            ( 1, 0), (1, -1), (0, -1),
                            (-1, 0), (0,  1), (1,  1)
                        }
                    },
                    {
                        HexagonOffsetParity.Odd,
                        new (int q, int r)[6]
                        {
                            ( 1, 0), ( 0, -1), (-1, -1),
                            (-1, 0), (-1,  1), ( 0,  1)
                        }
                    }
                }
            }
        };

        /// <summary>
        /// AxialNeighbors defines the six neighboring hexagons in axial coordinates (q, r) for a hexagon at (0, 0).
        /// </summary>
        public static readonly (int q, int r)[] AxialNeighbors = new (int q, int r)[6]
        {
            (0, 1), (-1, 1), (-1, 0), (0, -1), (1, -1),(1, 0)
        };

        /// <summary>
        /// CornerAngles aligned so that Index 0 starts the edge leading to AxialNeighbors[0].
        /// </summary>
        public static readonly Dictionary<HexagonOrientation, float[]> CornerAngles = new()
        {
            [HexagonOrientation.PointyTop] = new float[6] { 30f, 90f, 150f, 210f, 270f, 330f },
            
            [HexagonOrientation.FlatTop] = new float[6] { 0f, 60f, 120f, 180f, 240f,300f }
        };

        /// <summary>
        /// EdgeNormals aligned so that Index i is the physical direction of AxialNeighbors[i].
        /// </summary>
        public readonly static Dictionary<HexagonOrientation, (float x, float y)[]> EdgeNormals = new()
        {
            // Pointy: Normal 0 is at 330° (between 300° and 0°)
            [HexagonOrientation.PointyTop] = new (float x, float y)[]
            {
                (0.5f, HalfSqrt3), 
                (-0.5f, HalfSqrt3), 
                (-1, 0), 
                (-0.5f, -HalfSqrt3), 
                (0.5f, -HalfSqrt3),
                (1, 0)
            },
            // Flat: Normal 0 is at 0° (between 330° and 30°)
            [HexagonOrientation.FlatTop] = new (float x, float y)[]
            {
                (HalfSqrt3, 0.5f), 
                (0, 1), 
                (-HalfSqrt3, 0.5f), 
                (-HalfSqrt3, -0.5f), 
                (0, -1),
                (HalfSqrt3, -0.5f)
            }
        };
        
        /// <summary>
        /// GetEdgeSegment calculates the Cartesian coordinates of the endpoints of a specific edge of a hexagon.
        /// </summary>
        public static (float x1, float y1, float x2, float y2) GetEdgeSegment(float centerX, float centerY, float radius, int edgeIndex, HexagonOrientation orientation)
        {
            var p1 = GetCorner(radius, edgeIndex, orientation);
            var p2 = GetCorner(radius, (edgeIndex + 1) % 6, orientation);            
            return (centerX + p1.x, centerY + p1.y, centerX + p2.x, centerY + p2.y);
        }

        /// <summary>
        /// IsPointInHex checks if a given point (px, py) is inside a hexagon defined by its center (centerX, centerY), radius, and orientation.
        /// </summary>
        public static bool IsPointInHex(float px, float py, float centerX, float centerY, float radius, HexagonOrientation orientation)
        {
            float dx = MathF.Abs(px - centerX);
            float dy = MathF.Abs(py - centerY);

            if (orientation == HexagonOrientation.PointyTop)
            {
                float h = radius * Sqrt3 / 2f; // apothem
                if (dy > radius || dx > h) return false;
                return (radius * h - radius * dx - h / 2f * dy) >= 0;
            }
            else
            {
                float h = radius * Sqrt3 / 2f;
                if (dx > radius || dy > h) return false;
                return (radius * h - radius * dy - h / 2f * dx) >= 0;
            }
        }

        /// <summary>
        /// ToHex converts Cartesian coordinates to axial coordinates of a hexagon based on its radius and orientation.
        /// Note: The returned axial coordinates are fractional and may need to be rounded to the nearest hexagon using RoundHex or SnapToHex.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static (float q, float r, float s) ToHex(float x, float y, float radius, HexagonOrientation orientation)
        {
            switch (orientation)
            {
                case HexagonOrientation.PointyTop:
                    float q = (Sqrt3 / 3f * x - 1f / 3f * y) / radius;
                    float r = 2f / 3f * y / radius;
                    return (q, r, -q - r);
                case HexagonOrientation.FlatTop:
                    float q2 = 2f / 3f * x / radius;
                    float r2 = (Sqrt3 / 3f * y - 1f / 3f * x) / radius;
                    return (q2, r2, -q2 - r2);
            }
            throw new InvalidOperationException("Invalid hexagon orientation");
        }

        /// <summary>
        /// FromHex converts axial coordinates of a hexagon to Cartesian coordinates based on its radius and orientation.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <param name="radius"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static (float x, float y) FromHex(float q, float r, float radius, HexagonOrientation orientation)
        {
            switch (orientation)
            {
                case HexagonOrientation.PointyTop:
                    float x = radius * Sqrt3 * (q + r / 2f);
                    float y = radius * 1.5f * r;
                    return (x, y);
                case HexagonOrientation.FlatTop:
                    float x2 = radius * 1.5f * q;
                    float y2 = radius * Sqrt3 * (r + q / 2f);
                    return (x2, y2);
            }
            throw new InvalidOperationException("Invalid hexagon orientation");
        }

        /// <summary>
        /// GetSpacing calculates the horizontal and vertical spacing between hexagons based on their radius and orientation.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static (float xSpacing, float ySpacing) GetSpacing(float radius, HexagonOrientation orientation)
        {
            switch (orientation)
            {
                case HexagonOrientation.PointyTop:
                    return (Sqrt3 * radius, 1.5f * radius);
                case HexagonOrientation.FlatTop:
                    return (1.5f * radius, Sqrt3 * radius);
            }
            throw new InvalidOperationException("Invalid hexagon orientation");
        }

        /// <summary>
        /// SnapToQRS converts Cartesian coordinates to axial coordinates and rounds them to the nearest hexagon,
        /// effectively snapping a point to the nearest hexagon center.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static (int q, int r, int s) SnapToQRS(float x, float y, float radius, HexagonOrientation orientation)
        {
            var (q, r, s) = ToHex(x, y, radius, orientation);
            return RoundHex(q, r, s).ToQRS();
        }

        public static Hexagon SnapToHex(float x, float y, float radius, HexagonOrientation orientation)
        {
            var (q, r, s) = ToHex(x, y, radius, orientation);
            return RoundHex(q, r, s);
        }

        /// <summary>
        /// RoundHex rounds fractional axial coordinates to the nearest hexagon.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <param name="s"></param>
        /// <returns></returns>
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
            else
            {
                rs = -rq - rr;
            }

            return new Hexagon(rq, rr);
        }

        /// <summary>
        /// GetCorner calculates the Cartesian coordinates of a specific corner of a hexagon.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="corner"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static (float x, float y) GetCorner(float radius, int corner, HexagonOrientation orientation)
        {
            if (corner < 0) corner = 0;
            if (corner > 5) corner = 5;

            float angleDeg = CornerAngles[orientation][corner];
            float angleRad = angleDeg * Deg2Rad;
            float x = radius * (float)Math.Cos(angleRad);
            float y = radius * (float)Math.Sin(angleRad);

            return (x, y);
        }
    }
}