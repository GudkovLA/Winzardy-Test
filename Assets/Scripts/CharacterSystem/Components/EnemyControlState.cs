namespace Game.CharacterSystem.Components
{
    public struct EnemyControlState
    {
        public int ObstacleAvoidanceDirection;
        public float ObstacleAvoidanceDistance;
        public float DirectionChangeLastTime;
        public float MinDistanceToPlayer;
    }
}