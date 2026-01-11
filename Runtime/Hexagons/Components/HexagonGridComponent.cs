using UnityEngine;

namespace Cozy.Hexagons.Components
{
    public class HexagonGridComponent : MonoBehaviour
    {
        [SerializeField]
        private HexagonConfiguration configuration;

        public HexagonGrid Grid { get; private set; }

        private void Awake()
        {
            Grid = new HexagonGrid();
            Grid.BuildFromConfiguration(configuration);
        }

        private void OnDrawGizmosSelected()
        {
            Grid ??= new HexagonGrid();
            Grid.BuildFromConfiguration(configuration);

            Gizmos.color = Color.yellow;
            
            Grid.ForEach((hex) =>
            {
                var (xHex, yHex) = HexagonMath.FromHex[configuration.Orientation](hex, configuration.HexRadius);
                DrawHexOutline(xHex, yHex);
                return true;
            });
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