namespace Cozy.Hexagons
{
    /// <summary>
    /// HexagonOrientation defines the two primary orientations of a hexagon.
    /// </summary>
    public enum HexagonOrientation
    {
        PointyTop,
        FlatTop
    }

    /// <summary>
    /// HexagonCoordinateSystem defines the coordinate systems used for hexagon grids.
    /// </summary>
    public enum HexagonCoordinateSystem
    {
        Offset,
        Axial
    }

    /// <summary>
    /// HexagonOffsetParity defines the parity types for offset coordinate systems.
    /// </summary>
    public enum HexagonOffsetParity
    {
        Even,
        Odd
    }

    /// <summary>
    /// Hexagon represents a coordinate in a hexagonal grid.
    /// 
    /// This implementation uses the axial coordinate system (q, r), and implies s = -q - r to maintain the constraint q + r + s = 0.
    /// </summary>
    [System.Serializable]
    public struct Hexagon
    {
        public int Q;
        public int R;
        public readonly int S => -Q - R;

        public Hexagon(int q, int r)
        {
            Q = q;
            R = r;
        }
    }
}