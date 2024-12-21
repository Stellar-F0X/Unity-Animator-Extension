using AnimatorExtension.Parameters;

namespace AnimatorExtension
{
    public abstract class EventCallback
    {
        public EventCallback(EParameterType parameterType)
        {
            this.parameterType = parameterType;
        }
        
        public EParameterType parameterType;
    }
}