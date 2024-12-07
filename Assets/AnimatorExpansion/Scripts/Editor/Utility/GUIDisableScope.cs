using System;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    public class GUIDisableScope : IDisposable
    {
        public GUIDisableScope(bool enabled)
        {
            GUI.enabled = !enabled;
        }
        
        public void Dispose()
        {
            GUI.enabled = true;
            GC.SuppressFinalize(this);
        }
    }
}