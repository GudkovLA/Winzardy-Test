using System;

namespace Game.Common.Systems.Attributes
{

    public class UpdateAfterAttribute : Attribute
    {
        public readonly Type SystemType;

        public UpdateAfterAttribute(Type systemType)
        {
            if (!typeof(AbstractSystem).IsAssignableFrom(systemType))
            {
                throw new Exception("Only AbstractSystem type supported");
            }

            SystemType = systemType;
        }
    }
}