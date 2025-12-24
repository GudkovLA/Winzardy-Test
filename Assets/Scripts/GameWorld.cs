using Arch.Core;

namespace Game
{
    public class GameWorld
    {
        private readonly World _world;

        public GameWorld()
        {
            _world = World.Create();
        }
    }
}