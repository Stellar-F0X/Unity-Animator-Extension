namespace AnimatorExtension.Parameters
{
    public enum EAnimationEventParameter
    {
        Void,
        Int,
        Float,
        Bool,
        String,
        Vector2,
        Vector3,
        Quaternion,
        GameObject,
        Color,
        AnimationCurve,
        CurveResult, //TODO: Curve.Evaluate(normalizedTime)을 곱한 값을 반환.
        LayerMask,
        Tag,
        Customization,
        ScriptableObject
    };
}