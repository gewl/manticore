public delegate void DurationDelegate(float percentOfDurationRemaining);

public interface IRenewable
{
    RenewableTypes Type { get; }

    CooldownDelegate CooldownUpdater { get; set; }
    DurationDelegate DurationUpdater { get; set; }

    void UseRenewable();
}
