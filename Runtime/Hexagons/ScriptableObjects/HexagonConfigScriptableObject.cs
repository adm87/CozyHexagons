using UnityEngine;

namespace Cozy.Hexagons.ScriptableObjects
{
    [CreateAssetMenu(fileName = "HexagonConfig", menuName = "Cozy/Hexagons/Hexagon Configuration", order = 1)]
    public class HexagonConfigScriptableObject : ScriptableObject
    {
        public HexagonConfiguration Configuration;
    }
}