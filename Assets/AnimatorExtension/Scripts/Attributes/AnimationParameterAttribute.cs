using System;

namespace AnimatorExtension
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class AnimationParameterAttribute : Attribute
    {
        public AnimationParameterAttribute(string parameterPrefix = "")
        {
            this.parameterPrefix = parameterPrefix;
        }
        
        public string parameterPrefix;
    }
}