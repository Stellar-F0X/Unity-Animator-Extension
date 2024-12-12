using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimatorExpansion
{
    [Serializable]
    public class AnimationSpeedInfo
    {
        public MinMax controlRange;
        public AnimationCurve speedCurve;
    }
}