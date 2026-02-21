using UnityEngine;

namespace Cozy.Hexagons.Components
{
    public class HexagonGridComponent : MonoBehaviour
    {
        [SerializeField]
        private HexagonConfiguration configuration;

        public HexagonGrid Grid { get; private set; }

        public HexagonConfiguration Config => configuration;

        private void Awake()
        {
            Grid = new HexagonGrid();
            Grid.BuildFromConfiguration(configuration);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            Grid ??= new HexagonGrid();
            Grid.BuildFromConfiguration(configuration);
            Grid.ForEach((hex) =>
            {
                var (xHex, yHex) = HexagonMath.FromHex(hex.Q, hex.R, configuration.HexRadius, configuration.Orientation);
                DrawHexOutline(xHex, yHex);
                return true;
            });
        }
        
        private void DrawHexOutline(float xHex, float yHex)
        {
            var position = transform.position;

            var apothem = configuration.HexRadius * HexagonMath.HalfSqrt3;
            var edgeNormals = HexagonMath.EdgeNormals[configuration.Orientation];

            for (int i = 0; i < 6; i++)
            {
                var (x1, y1) = HexagonMath.GetCorner(configuration.HexRadius, i, configuration.Orientation);
                var (x2, y2) = HexagonMath.GetCorner(configuration.HexRadius, (i + 1) % 6, configuration.Orientation);

                Gizmos.DrawLine(
                    new Vector3(position.x + xHex + x1, position.y, position.z + yHex + y1),
                    new Vector3(position.x + xHex + x2, position.y, position.z + yHex + y2)
                );
            }
        }
    }
}