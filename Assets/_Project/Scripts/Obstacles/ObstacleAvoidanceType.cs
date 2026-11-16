namespace Hitrush.Obstacles
{
    public enum ObstacleAvoidanceType
    {
        None,           // Cannot be avoided - instant death
        Jump,           // Jump over
        Slide,          // Slide under
        LaneChange,     // Change lanes
        Any             // Any action works
    }
}
