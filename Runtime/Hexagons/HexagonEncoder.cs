namespace Cozy.Hexagons
{
    /// <summary>
    /// HexagonEncoder provides methods to encode and decode hexagon coordinates to and from a unique long identifier.
    /// </summary>
    public static class HexagonEncoder
    {
        /// <summary>
        /// Encode converts axial hexagon coordinates to a unique long identifier.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static long Encode(Hexagon hex)
        {
            return ((long)hex.Q << 32) | (uint)hex.R;
        }

        /// <summary>
        /// Decode converts a unique long identifier back to axial hexagon coordinates.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Hexagon Decode(long id)
        {
            int q = (int)(id >> 32);
            int r = (int)id;
            return new Hexagon(q, r);
        }
    }
}