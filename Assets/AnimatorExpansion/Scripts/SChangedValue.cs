namespace AnimatorExpansion
{
    public struct SChangedValue
    {
        public SChangedValue(bool isChanged, float value)
        {
            this.isChanged = isChanged;
            this.value = value;
        }
        
        public readonly bool isChanged;
        public readonly float value;
    }
}