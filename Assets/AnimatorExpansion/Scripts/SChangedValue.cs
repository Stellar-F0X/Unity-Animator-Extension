namespace AnimatorExpansion
{
    public struct SChangedValue
    {
        public SChangedValue(bool isChanged, float changedValue)
        {
            this.isChanged = isChanged;
            this.changedValue = changedValue;
        }
        
        public bool isChanged;
        public float changedValue;
    }
}