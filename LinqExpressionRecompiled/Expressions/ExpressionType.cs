namespace System.Linq.Expressions
{
    using System;

    [__DynamicallyInvokable]
    public enum ExpressionType
    {
        [__DynamicallyInvokable]
        Add = 0,
        [__DynamicallyInvokable]
        AddAssign = 0x3f,
        [__DynamicallyInvokable]
        AddAssignChecked = 0x4a,
        [__DynamicallyInvokable]
        AddChecked = 1,
        [__DynamicallyInvokable]
        And = 2,
        [__DynamicallyInvokable]
        AndAlso = 3,
        [__DynamicallyInvokable]
        AndAssign = 0x40,
        [__DynamicallyInvokable]
        ArrayIndex = 5,
        [__DynamicallyInvokable]
        ArrayLength = 4,
        [__DynamicallyInvokable]
        Assign = 0x2e,
        [__DynamicallyInvokable]
        Block = 0x2f,
        [__DynamicallyInvokable]
        Call = 6,
        [__DynamicallyInvokable]
        Coalesce = 7,
        [__DynamicallyInvokable]
        Conditional = 8,
        [__DynamicallyInvokable]
        Constant = 9,
        [__DynamicallyInvokable]
        Convert = 10,
        [__DynamicallyInvokable]
        ConvertChecked = 11,
        [__DynamicallyInvokable]
        DebugInfo = 0x30,
        [__DynamicallyInvokable]
        Decrement = 0x31,
        [__DynamicallyInvokable]
        Default = 0x33,
        [__DynamicallyInvokable]
        Divide = 12,
        [__DynamicallyInvokable]
        DivideAssign = 0x41,
        [__DynamicallyInvokable]
        Dynamic = 50,
        [__DynamicallyInvokable]
        Equal = 13,
        [__DynamicallyInvokable]
        ExclusiveOr = 14,
        [__DynamicallyInvokable]
        ExclusiveOrAssign = 0x42,
        [__DynamicallyInvokable]
        Extension = 0x34,
        [__DynamicallyInvokable]
        Goto = 0x35,
        [__DynamicallyInvokable]
        GreaterThan = 15,
        [__DynamicallyInvokable]
        GreaterThanOrEqual = 0x10,
        [__DynamicallyInvokable]
        Increment = 0x36,
        [__DynamicallyInvokable]
        Index = 0x37,
        [__DynamicallyInvokable]
        Invoke = 0x11,
        [__DynamicallyInvokable]
        IsFalse = 0x54,
        [__DynamicallyInvokable]
        IsTrue = 0x53,
        [__DynamicallyInvokable]
        Label = 0x38,
        [__DynamicallyInvokable]
        Lambda = 0x12,
        [__DynamicallyInvokable]
        LeftShift = 0x13,
        [__DynamicallyInvokable]
        LeftShiftAssign = 0x43,
        [__DynamicallyInvokable]
        LessThan = 20,
        [__DynamicallyInvokable]
        LessThanOrEqual = 0x15,
        [__DynamicallyInvokable]
        ListInit = 0x16,
        [__DynamicallyInvokable]
        Loop = 0x3a,
        [__DynamicallyInvokable]
        MemberAccess = 0x17,
        [__DynamicallyInvokable]
        MemberInit = 0x18,
        [__DynamicallyInvokable]
        Modulo = 0x19,
        [__DynamicallyInvokable]
        ModuloAssign = 0x44,
        [__DynamicallyInvokable]
        Multiply = 0x1a,
        [__DynamicallyInvokable]
        MultiplyAssign = 0x45,
        [__DynamicallyInvokable]
        MultiplyAssignChecked = 0x4b,
        [__DynamicallyInvokable]
        MultiplyChecked = 0x1b,
        [__DynamicallyInvokable]
        Negate = 0x1c,
        [__DynamicallyInvokable]
        NegateChecked = 30,
        [__DynamicallyInvokable]
        New = 0x1f,
        [__DynamicallyInvokable]
        NewArrayBounds = 0x21,
        [__DynamicallyInvokable]
        NewArrayInit = 0x20,
        [__DynamicallyInvokable]
        Not = 0x22,
        [__DynamicallyInvokable]
        NotEqual = 0x23,
        [__DynamicallyInvokable]
        OnesComplement = 0x52,
        [__DynamicallyInvokable]
        Or = 0x24,
        [__DynamicallyInvokable]
        OrAssign = 70,
        [__DynamicallyInvokable]
        OrElse = 0x25,
        [__DynamicallyInvokable]
        Parameter = 0x26,
        [__DynamicallyInvokable]
        PostDecrementAssign = 80,
        [__DynamicallyInvokable]
        PostIncrementAssign = 0x4f,
        [__DynamicallyInvokable]
        Power = 0x27,
        [__DynamicallyInvokable]
        PowerAssign = 0x47,
        [__DynamicallyInvokable]
        PreDecrementAssign = 0x4e,
        [__DynamicallyInvokable]
        PreIncrementAssign = 0x4d,
        [__DynamicallyInvokable]
        Quote = 40,
        [__DynamicallyInvokable]
        RightShift = 0x29,
        [__DynamicallyInvokable]
        RightShiftAssign = 0x48,
        [__DynamicallyInvokable]
        RuntimeVariables = 0x39,
        [__DynamicallyInvokable]
        Subtract = 0x2a,
        [__DynamicallyInvokable]
        SubtractAssign = 0x49,
        [__DynamicallyInvokable]
        SubtractAssignChecked = 0x4c,
        [__DynamicallyInvokable]
        SubtractChecked = 0x2b,
        [__DynamicallyInvokable]
        Switch = 0x3b,
        [__DynamicallyInvokable]
        Throw = 60,
        [__DynamicallyInvokable]
        Try = 0x3d,
        [__DynamicallyInvokable]
        TypeAs = 0x2c,
        [__DynamicallyInvokable]
        TypeEqual = 0x51,
        [__DynamicallyInvokable]
        TypeIs = 0x2d,
        [__DynamicallyInvokable]
        UnaryPlus = 0x1d,
        [__DynamicallyInvokable]
        Unbox = 0x3e
    }
}

