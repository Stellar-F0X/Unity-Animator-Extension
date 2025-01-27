using System;

namespace AnimatorExtension.Parameters
{
    public enum EEventDispatchType
    {
        None,
        Start, //Inspector에서 Transition Mute를 하거나 해제하면 Node가 재실행되면서 Start와 Enter가 호출되니 주의해야 됨.
        Enter,
        Update,
        End,
        Exit,
        Point,
        Range
    };
}