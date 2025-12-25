using System;

namespace Game.Common.Systems.Attributes
{
    public class UpdateBeforeAttribute : Attribute
    {
        public readonly Type SystemType;

        public UpdateBeforeAttribute(Type systemType)
        {
            if (!typeof(AbstractSystem).IsAssignableFrom(systemType))
            {
                throw new Exception("Only AbstractSystem type supported");
            }

            SystemType = systemType;
        }
    }
}