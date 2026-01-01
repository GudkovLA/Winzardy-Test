using System;

namespace Game.Components
{
    public struct Fraction
    {
        public FractionMask AlliesMask;
        public FractionMask EnemiesMask;
    }

    [Flags]
    public enum FractionMask
    {
        None = 0,
        Player = 1,
        Monster = 2
    }
}