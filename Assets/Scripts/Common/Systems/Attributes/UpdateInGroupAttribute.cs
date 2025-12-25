using System;

namespace Game.Common.Systems.Attributes
{
    public class UpdateInGroupAttribute : Attribute
    {
        public readonly Type SystemType;

        public UpdateInGroupAttribute(Type systemType)
        {
            if (!typeof(AbstractSystemGroup).IsAssignableFrom(systemType))
            {
                throw new Exception("Only AbstractSystemGroup type supported");
            }

            SystemType = systemType;
        }
    }
}