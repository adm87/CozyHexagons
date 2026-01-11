using Cozy.Hexagons.Components;
using UnityEngine;

namespace Cozy.Hexagons.Examples
{
    public class HexagonExample : MonoBehaviour
    {
        [SerializeField]
        private HexagonGridComponent GridComponent;

        private Vector3? hitPoint;

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                hitPoint = hitInfo.point;
            }
            else
            {
                hitPoint = null;
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || hitPoint == null)
            {
                return;
            }
            
            Hexagon inputHex = HexagonMath.ToHex[GridComponent.Config.Orientation](
                hitPoint.Value.x - transform.position.x,
                hitPoint.Value.z - transform.position.z,
                GridComponent.Config.HexRadius
            );

            if (GridComponent.Grid.TryGetHexagon(inputHex, out Hexagon hoverHex))
            {
                var (xHex, yHex) = HexagonMath.FromHex[GridComponent.Config.Orientation](hoverHex, GridComponent.Config.HexRadius);
                
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector3(transform.position.x + xHex, transform.position.y, transform.position.z + yHex),0.2f);

                for (int neighbor = 0; neighbor < 6; neighbor++)
                {
                    int dq, dr;

                    switch (GridComponent.Config.CoordinateSystem)
                    {
                        case HexagonCoordinateSystem.Axial:
                            (dq, dr) = HexagonMath.AxialNeighbors[neighbor];
                            break;
                        case HexagonCoordinateSystem.Offset:
                            (dq, dr) = HexagonMath.OffsetNeighbors[GridComponent.Config.Orientation][GridComponent.Config.OffsetGrid.OffsetParity][neighbor];
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException($"Unsupported HexagonCoordinateSystem: {GridComponent.Config.CoordinateSystem}");
                    }

                    Hexagon neighborHex = new(hoverHex.Q + dq, hoverHex.R + dr);

                    if (GridComponent.Grid.TryGetHexagon(neighborHex, out Hexagon validNeighborHex))
                    {
                        var (nxHex, nyHex) = HexagonMath.FromHex[GridComponent.Config.Orientation](validNeighborHex, GridComponent.Config.HexRadius);
                        
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(new Vector3(transform.position.x + nxHex, transform.position.y, transform.position.z + nyHex),0.1f);
                    }
                }
            }
        }
    }
}