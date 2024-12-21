using System;
using UnityEngine;

namespace AnimatorExtension
{
    [Serializable]
    public class AnimationSpeedInfo
    {
        public MinMax controlRange;
        public AnimationCurve speedCurve;
    }
}