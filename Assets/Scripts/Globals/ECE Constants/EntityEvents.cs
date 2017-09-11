/// <summary>
/// Holds constants for broadcasting events via EntityEmitter.
/// </summary>
public class EntityEvents
{
    public const string Update = "update";
    public const string FixedUpdate = "fixed_update";
    public const string LateUpdate = "late_update";

    public const string Hurt = "hurt";
    public const string Recovered = "recovered";
    public const string Dead = "dead";

    public const string Aggro = "aggro";
    public const string Deaggro = "deaggro";
    public const string TargetUpdated = "target_updated";

    public const string WaypointReached = "waypoint_reached";
    public const string ClearWaypoint = "clear_waypoint";
    public const string SetWaypoint = "set_waypoint";
}
