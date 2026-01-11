using UnityEditor;
using UnityEngine;

namespace Cozy.Hexagons
{
    [CustomPropertyDrawer(typeof(HexagonConfiguration))]
    public class HexagonConfigurationEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, 
                label, 
                true
            );

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                y = DrawProperty(position, property, "Orientation", y);
                y = DrawProperty(position, property, "CoordinateSystem", y);
                y = DrawProperty(position, property, "HexRadius", y);

                var coordinateSystemProp = property.FindPropertyRelative("CoordinateSystem");
                if (coordinateSystemProp != null)
                {
                    if ((HexagonCoordinateSystem)coordinateSystemProp.enumValueIndex == HexagonCoordinateSystem.Axial)
                    {
                        DrawProperty(position, property, "AxialGrid", y);
                    }
                    else if ((HexagonCoordinateSystem)coordinateSystemProp.enumValueIndex == HexagonCoordinateSystem.Offset)
                    {
                        DrawProperty(position, property, "OffsetGrid", y);
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private float DrawProperty(Rect position, SerializedProperty parent, string propertyName, float yPos)
        {
            var prop = parent.FindPropertyRelative(propertyName);
            if (prop != null)
            {
                float height = EditorGUI.GetPropertyHeight(prop, true);
                EditorGUI.PropertyField(new Rect(position.x, yPos, position.width, height), prop, true);
                return yPos + height + EditorGUIUtility.standardVerticalSpacing;
            }
            return yPos;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) 
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            height += GetPropHeight(property, "Orientation");
            height += GetPropHeight(property, "CoordinateSystem");
            height += GetPropHeight(property, "HexRadius");

            var coordinateSystemProp = property.FindPropertyRelative("CoordinateSystem");
            if (coordinateSystemProp != null)
            {
                switch ((HexagonCoordinateSystem)coordinateSystemProp.enumValueIndex)
                {
                    case HexagonCoordinateSystem.Axial:
                        height += GetPropHeight(property, "AxialGrid");
                        break;
                    case HexagonCoordinateSystem.Offset:
                        height += GetPropHeight(property, "OffsetGrid");
                        break;
                }
            }

            return height;
        }

        private float GetPropHeight(SerializedProperty parent, string propertyName)
        {
            var prop = parent.FindPropertyRelative(propertyName);
            return prop != null ? EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing : 0;
        }
    }
}