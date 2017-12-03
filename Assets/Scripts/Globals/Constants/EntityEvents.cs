/// <summary>
/// Holds constants for broadcasting events via EntityEmitter.
/// </summary>
public class EntityEvents
{
    public const string Update = "update";
    public const string FixedUpdate = "fixed_update";
    public const string LateUpdate = "late_update";

    public const string PrimaryFire = "primary_fire";
    public const string SecondaryFire = "secondary_fire";

    public const string FreezeRotation = "freeze_rotation";
	public const string ResumeRotation = "resume_rotation";

	public const string Move = "move";
    public const string Stop = "stop";

    // 'Stun' is both a status effect and the name for the state which the
    // entity enters upon being damaged. It refers to total disconnect between
    // input and action (or AI and action). 
    public const string Stun = "stun";
    public const string Unstun = "unstun";

    // Gonna see if we can get by without "damaged" and "healed."
    public const string HealthChanged = "health_changed";

	public const string Invulnerable = "invuln";
	public const string Vulnerable = "vuln";

	// 'Busy' is emitted by actions when they've begun successfully firing. Unlike
	// 'stun', busy state allows (reduced) movement.
	public const string Busy = "busy";
	public const string Available = "available";

    // Adding Hurt for audio component, so OnDamage will fire Hurt AND Stun. Maybe less than ideal, reconsider later on.
    public const string Hurt = "hurt";
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
