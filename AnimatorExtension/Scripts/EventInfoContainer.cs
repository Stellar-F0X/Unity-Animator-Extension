using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimatorExtension.Parameters
{
    internal class EventInfoContainer
    {
        private List<Type> _customParams = new List<Type>();
        private List<int> _eventNameHashes = new List<int>();
        private List<string> _eventNames = new List<string>();
        private List<EAnimationEventParameter> _paramTypes = new List<EAnimationEventParameter>();
        private Dictionary<int, Type> _customParamTypes = new Dictionary<int, Type>();

        
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
        
        
        public void AddInfo(AnimationEventAttribute attribute)
        {
            this.count++;
            
            this._eventNames.Add(attribute.eventName);
            this._paramTypes.Add(attribute.eventParameter);
            this._customParams.Add(attribute.customParameterType);
            this._eventNameHashes.Add(Extension.StringToHash(attribute.eventName));
        }


        
        public Type FindTypeByHash(int hashCode)
        {
            if (_customParamTypes.TryGetValue(hashCode, out Type type))
            {
                return type;
            }

            return null;
        }
        

        public void Clear()
        {
            this.count = 1;
            
            this._eventNames.Clear();
            this._paramTypes.Clear();
            this._customParams.Clear();
            this._customParamTypes.Clear();
            
            this._eventNames.Add("None");
            this._paramTypes.Add(EAnimationEventParameter.Void);
            this._customParams.Add(null);
            this._eventNameHashes.Add(0);
        }
        

        public void Build()
        {
            paramTypes = _paramTypes.ToArray();
            
            eventNames = new string[count];
            eventNameHashes = new int[count];
            customParamTypes = new Type[count];

            for (int i = 0; i < count; i++)
            {
                this.eventNames[i] = this._eventNames[i];
                this.customParamTypes[i] = this._customParams[i];
                this.eventNameHashes[i] = this._eventNameHashes[i];
                
                this._customParamTypes.Add(this.eventNameHashes[i], this.customParamTypes[i]);
            }
        }
    }
}