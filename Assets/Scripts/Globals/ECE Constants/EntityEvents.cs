﻿/// <summary>
/// Holds constants for broadcasting events via EntityEmitter.
/// </summary>
public class EntityEvents
{
    public const string Update = "update";
    public const string FixedUpdate = "fixed_update";
    public const string LateUpdate = "late_update";

    public const string PrimaryFire = "primary_fire";
    public const string SecondaryFire = "secondary_fire";

	public const string Parry = "parry";
	public const string Blink = "blink";

    public const string FreezeRotation = "freeze_rotation";
	public const string ResumeRotation = "resume_rotation";

	public const string Move = "move";
    public const string Stop = "stop";

    public const string Hurt = "hurt";
    public const string Recovered = "recovered";
    public const string Dead = "dead";

    public const string Aggro = "aggro";
    public const string Deaggro = "deaggro";
    public const string TargetUpdated = "target_updated";
    public const string TargetPositionUpdated = "target_position_updated";

    public const string DirectionChanged = "direction_changed";

    public const string WaypointReached = "waypoint_reached";
    public const string ClearWaypoint = "clear_waypoint";
    public const string SetWaypoint = "set_waypoint";
}
