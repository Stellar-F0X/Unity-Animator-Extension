using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimatorExtension.Parameters
{
    public class EventInfoContainer
    {
        private List<Type> _customParams = new List<Type>();
        private List<string> _eventNames = new List<string>();
        private List<EAnimationEventParameter> _paramTypes = new List<EAnimationEventParameter>();

        
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

        public EAnimationEventParameter[] paramTypes
        {
            get;
            private set;
        }
        
        public Type[] customParamTypes
        {
            get;
            private set;
        }

        public int[] eventNameHashes
        {
            get;
            private set;
        }
        
        
        public void AddInfo(string eventName, EAnimationEventParameter param, Type customParam)
        {
            this.count++;
            
            this._eventNames.Add(eventName);
            this._paramTypes.Add(param);
            this._customParams.Add(customParam);
        }

        public Type FindTypeByHash(int hashCode)
        {
            for (int i = 0; i < this._paramTypes.Count; i++)
            {
                if (eventNameHashes[i] == hashCode)
                {
                    return customParamTypes[i];
                }
            }

            return null;
        }

        public void Clear()
        {
            this.count = 0;
            
            this._eventNames.Clear();
            this._paramTypes.Clear();
            this._customParams.Clear();
        }

        public void Build()
        {
            customParamTypes = _customParams.ToArray();
            paramTypes = _paramTypes.ToArray();
            
            eventNames = new string[count];
            eventNameHashes = new int[count];

            for (int i = 0; i < count; i++)
            {
                this.eventNames[i] = this._eventNames[i];
                this.eventNameHashes[i] = Extension.StringToHash(this._eventNames[i]);
            }
        }
    }
}