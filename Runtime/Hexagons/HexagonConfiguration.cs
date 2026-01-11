namespace Cozy.Hexagons
{
    /// <summary>
    /// OffsetGridConfiguration defines the configuration parameters for an offset hexagon grid.
    /// </summary>
    [System.Serializable]
    public struct OffsetGridConfiguration
    {
        public HexagonOffsetParity OffsetParity;
        public uint Width;
        public uint Height;
    }

    /// <summary>
    /// AxialGridConfiguration defines the configuration parameters for an axial hexagon grid.
    /// </summary>
    [System.Serializable]
    public struct AxialGridConfiguration
    {
        public uint Radius;
    }

    /// <summary>
    /// HexagonConfiguration defines the overall configuration for a hexagon grid.
    /// </summary>
    [System.Serializable]
    public struct HexagonConfiguration
    {
        /// <summary>
        /// Orientation of the hexagons.
        /// </summary>
        public HexagonOrientation Orientation;
        /// <summary>
        /// Coordinate system used for the hexagon grid.
        /// </summary>
        public HexagonCoordinateSystem CoordinateSystem;
        /// <summary>
        /// OffsetGridConfiguration if using Offset coordinate system.
        /// </summary>
        public OffsetGridConfiguration OffsetGrid;
        /// <summary>
        /// AxialGridConfiguration if using Axial coordinate system.
        /// </summary>
        public AxialGridConfiguration AxialGrid;
        /// <summary>
        /// HexRadius defines the radius of each hexagon in the grid.
        /// </summary>
        public float HexRadius;
    }
}