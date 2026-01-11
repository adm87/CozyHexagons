namespace Cozy.Hexagons
{
    using Hexagons = System.Collections.Generic.Dictionary<long, Hexagon>;
    using HexagonItrFunc = System.Func<Hexagon, bool>;
    using UnityEngine;

    /// <summary>
    /// HexagonGrid represents a collection of hexagons arranged in a grid.
    /// 
    /// It stores hexagon coordinates and can be initialized from a config or programmatically.
    /// It provides methods to add, remove, and iterate over hexagons in the grid.
    /// It does not store geometric information (i.e., size, orientation) about the hexagons.
    /// </summary>
    public class HexagonGrid
    {
        private Hexagons hexagons;

        public HexagonGrid()
        {
            hexagons = new Hexagons();
        }

        public HexagonGrid(Hexagons hexagons)
        {
            this.hexagons = hexagons;
        }

        /// <summary>
        /// AddHexagon adds a hexagon to the grid.
        /// </summary>
        /// <param name="hex"></param>
        public void AddHexagon(Hexagon hex)
        {
            long id = HexagonEncoder.Encode(hex);
            hexagons[id] = hex;
        }

        /// <summary>
        /// AddHexagon adds a hexagon to the grid.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        public void AddHexagon(int q, int r)
        {
            long id = HexagonEncoder.Encode(new Hexagon(q, r));
            hexagons[id] = new Hexagon(q, r);
        }

        /// <summary>
        /// RemoveHexagon removes a hexagon from the grid.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public bool RemoveHexagon(Hexagon hex)
        {
            long id = HexagonEncoder.Encode(hex);
            return hexagons.Remove(id);
        }

        /// <summary>
        /// RemoveHexagon removes a hexagon from the grid.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool RemoveHexagon(int q, int r)
        {
            long id = HexagonEncoder.Encode(new Hexagon(q, r));
            return hexagons.Remove(id);
        }

        /// <summary>
        /// Clear removes all hexagons from the grid.
        /// </summary>
        public void Clear()
        {
            hexagons.Clear();
        }

        /// <summary>
        /// TryGetHexagon attempts to retrieve a hexagon by its encoded ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hexagon"></param>
        /// <returns></returns>
        public bool TryGetHexagon(long id, out Hexagon hexagon)
        {
            return hexagons.TryGetValue(id, out hexagon);
        }

        /// <summary>
        /// TryGetHexagon attempts to retrieve a hexagon by its axial coordinates.
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="foundHex"></param>
        /// <returns></returns>
        public bool TryGetHexagon(Hexagon hex, out Hexagon foundHex)
        {
            long id = HexagonEncoder.Encode(hex);
            return hexagons.TryGetValue(id, out foundHex);
        }

        /// <summary>
        /// TryGetHexagon attempts to retrieve a hexagon by its axial coordinates.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <param name="foundHex"></param>
        /// <returns></returns>
        public bool TryGetHexagon(int q, int r, out Hexagon foundHex)
        {
            long id = HexagonEncoder.Encode(new Hexagon(q, r));
            return hexagons.TryGetValue(id, out foundHex);
        }

        /// <summary>
        /// ForEach iterates over each hexagon in the grid, applying the provided function.
        /// 
        /// If the function returns false, the iteration stops.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool ForEach(HexagonItrFunc func)
        {
            foreach (var hex in hexagons.Values)
            {
                if (!func(hex))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// BuildFromConfiguration constructs the hexagon grid based on the provided configuration.
        /// </summary>
        /// <param name="configuration"></param>
        public void BuildFromConfiguration(HexagonConfiguration configuration)
        {
            Clear();

            switch (configuration.CoordinateSystem)
            {
                case HexagonCoordinateSystem.Axial:
                    hexagons = BuildAxialGrid(configuration.AxialGrid);
                    break;
                case HexagonCoordinateSystem.Offset:
                    hexagons = BuildOffsetGrid(configuration.Orientation, configuration.OffsetGrid);
                    break;
            }
        }

        /// <summary>
        /// BuildAxialGrid builds a hexagon grid using axial coordinates based on the provided configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Hexagons BuildAxialGrid(AxialGridConfiguration config)
        {
            Hexagons hexagons = new();

            int radius = (int)config.Radius;
            for (int q = -radius; q <= radius; q++)
            {
                int r1 = Mathf.Max(-radius, -q - radius);
                int r2 = Mathf.Min(radius, -q + radius);
                for (int r = r1; r <= r2; r++)
                {
                    long id = HexagonEncoder.Encode(new Hexagon(q, r));
                    hexagons[id] = new Hexagon(q, r);
                }
            }

            return hexagons;
        }

        /// <summary>
        /// BuildOffsetGrid builds a hexagon grid using offset coordinates based on the provided orientation and configuration.
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static Hexagons BuildOffsetGrid(HexagonOrientation orientation, OffsetGridConfiguration config)
        {
            Hexagons hexagons = new();

            int width = (int)config.Width;
            int height = (int)config.Height;
            int parity = config.OffsetParity == HexagonOffsetParity.Odd ? 1 : 0;

            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    int q, r;

                    switch (orientation)
                    {
                        case HexagonOrientation.PointyTop:
                            q = col - ((row - parity) / 2);
                            r = row;
                            break;

                        case HexagonOrientation.FlatTop:
                            q = col;
                            r = row - ((col - parity) / 2);
                            break;

                        default:
                            throw new System.ArgumentOutOfRangeException($"Unsupported HexagonOrientation: {orientation}");
                    }

                    Hexagon hexagon = new(q, r);

                    long id = HexagonEncoder.Encode(hexagon);
                    hexagons[id] = hexagon;
                }
            }

            return hexagons;
        }
    }
}