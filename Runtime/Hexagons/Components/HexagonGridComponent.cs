using UnityEngine;

namespace Cozy.Hexagons.Components
{
    public class HexagonGridComponent : MonoBehaviour
    {
        [SerializeField]
        private HexagonConfiguration configuration;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            
            if (configuration.CoordinateSystem == HexagonCoordinateSystem.Axial)
            {
                int radius = (int)configuration.AxialGrid.Radius;
                for (int q = -radius; q <= radius; q++)
                {
                    int r1 = Mathf.Max(-radius, -q - radius);
                    int r2 = Mathf.Min(radius, -q + radius);
                    for (int r = r1; r <= r2; r++)
                    {
                        DrawAxialGrid(q, r);
                    }
                }
            }
            else if (configuration.CoordinateSystem == HexagonCoordinateSystem.Offset)
            {
                int width = (int)configuration.OffsetGrid.Width;
                int height = (int)configuration.OffsetGrid.Height;
                for (int col = 0; col < width; col++)
                {
                    for (int row = 0; row < height; row++)
                    {
                        DrawOffsetGrid(col, row, configuration.OffsetGrid.OffsetParity);
                    }
                }
            }
        }
        
        private void DrawAxialGrid(int q, int r)
        {
            var (x, y) = HexagonMath.FromHex[configuration.Orientation](new Hexagon(q, r), configuration.HexRadius);
            DrawHexOutline(x, y);
        }
        
        private void DrawOffsetGrid(int col, int row, HexagonOffsetParity parity)
        {
            int q, r;

            if (configuration.Orientation == HexagonOrientation.PointyTop)
            {
                int parityInt = parity == HexagonOffsetParity.Odd ? 1 : 0;
                q = col - ((row - parityInt) / 2);
                r = row;
            }
            else
            {
                int parityInt = parity == HexagonOffsetParity.Odd ? 1 : 0;
                q = col;
                r = row - ((col - parityInt) / 2);
            }

            var (x, y) = HexagonMath.FromHex[configuration.Orientation](new Hexagon(q, r), configuration.HexRadius);
            DrawHexOutline(x, y);
        }
        
        private void DrawHexOutline(float xHex, float yHex)
        {
            Vector3 position = transform.position;
            for (int corner = 0; corner < 6; corner++)
            {
                var (x1, y1) = HexagonMath.GetCorner(configuration.HexRadius, corner, configuration.Orientation);
                var (x2, y2) = HexagonMath.GetCorner(configuration.HexRadius, (corner + 1) % 6, configuration.Orientation);

                Gizmos.DrawLine(
                    new Vector3(position.x + xHex + x1, position.y, position.z + yHex + y1),
                    new Vector3(position.x + xHex + x2, position.y, position.z + yHex + y2)
                );
            }
        }
    }
}