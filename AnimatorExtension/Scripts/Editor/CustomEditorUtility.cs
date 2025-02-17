using UnityEditor;
using UnityEngine;

namespace AnimatorExtension.AnimatorEditor
{
    public static class CustomEditorUtility
    {
        public static Rect CalculateVariableRect(Rect position, float widthPercentage, float horizontalOffset = 0, float beforeEmpty = 0, float subtractWidth = 0)
        {
            float width = Mathf.Clamp(position.width * widthPercentage - beforeEmpty - subtractWidth, 0, position.width);

            return new Rect(position.x + beforeEmpty + horizontalOffset, position.y, width, EditorGUIUtility.singleLineHeight);
        }


        public static  Rect CalculateConstantRect(Rect position, float fieldWidth, float horizontalOffset = 0, float beforeEmpty = 0, float subtractWidth = 0)
        {
            float width = Mathf.Clamp(fieldWidth - beforeEmpty - subtractWidth, 0, position.width);

            return new Rect(position.x + beforeEmpty + horizontalOffset, position.y, width, EditorGUIUtility.singleLineHeight);
        }
    }
}