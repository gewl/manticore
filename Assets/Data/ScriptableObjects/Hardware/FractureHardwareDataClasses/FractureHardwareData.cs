using System;

[Serializable]
public abstract class FractureHardwareData : HardwareData {

    protected abstract float BaseDamageDealt { get; }
    protected abstract float DamageDealtMomentumIncrement { get; }
    
    public virtual float GetDamageDealt(int currentMomentum)
    {
        return BaseDamageDealt + (DamageDealtMomentumIncrement * currentMomentum);
    }

    protected abstract float BaseRange { get; }
    protected abstract float RangeMomentumIncrement { get; }

    public virtual float GetRange(int currentMomentum)
    {
        return BaseRange + (RangeMomentumIncrement * currentMomentum);
    }

    protected abstract float BaseFragmentationSpeedModifier { get; }
    protected abstract float FragmentationSpeedModifierMomentumIncrease { get; }

    public virtual float GetFragmentationSpeed(int currentMomentum)
    {
        return BaseFragmentationSpeedModifier + (FragmentationSpeedModifierMomentumIncrease * currentMomentum);
    }
}
