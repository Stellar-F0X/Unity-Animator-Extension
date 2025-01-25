using System;
using System.Collections.Generic;

namespace AnimatorExtension.Parameters
{
    public class EventInfoContainer
    {
        private List<Type> _customParams = new List<Type>();
        private List<string> _eventNames = new List<string>();
        private List<EParameterType> _paramTypes = new List<EParameterType>();

        
        public int count
        {
            get;
            private set;
        }

        public string[] eventNames
        {
            get;
            private set;
        }

        public EParameterType[] paramTypes
        {
            get;
            private set;
        }
        
        public Type[] customParams
        {
            get;
            private set;
        }
        
        
        public void AddInfo(string eventName, EParameterType paramType, Type customParam)
        {
            count++;
            
            this._eventNames.Add(eventName);
            this._paramTypes.Add(paramType);
            this._customParams.Add(customParam);
        }

        public void Clear()
        {
            this._eventNames.Clear();
            this._paramTypes.Clear();
            this._customParams.Clear();
        }

        public void Build()
        {
            customParams = _customParams.ToArray();
            eventNames = _eventNames.ToArray();
            paramTypes = _paramTypes.ToArray();
        }
    }
}