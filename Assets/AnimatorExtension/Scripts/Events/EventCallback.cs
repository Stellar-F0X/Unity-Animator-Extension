using AnimatorExpansion.Parameters;

namespace AnimatorExpansion
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