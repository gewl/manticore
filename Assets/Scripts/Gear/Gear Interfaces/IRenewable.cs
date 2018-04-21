public delegate void DurationDelegate(float percentOfDurationRemaining);

public interface IRenewable
{
    RenewableTypes Type { get; }

    CooldownDelegate CooldownPercentUpdater { get; set; }
    CooldownDelegate CooldownDurationUpdater { get; set; }
    DurationDelegate DurationUpdater { get; set; }

    void UseRenewable();
}
