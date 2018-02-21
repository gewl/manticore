public class EntityStatHandler : EntityComponent {

    float BaseMoveSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }
    float BaseDamageDealtModifier { get { return 1.0f; } }
    float BaseDamageReceivedModifier { get { return 1.0f; } }

    protected override void Subscribe() { }

    protected override void Unsubscribe() { }

    public float GetMoveSpeed()
    {
        float calculatedMoveSpeed = entityModifierHandler.ApplyModifiersToValue(ModifierType.MoveSpeed, BaseMoveSpeed);
        return calculatedMoveSpeed;
    }

    public float GetDamageDealtModifier()
    {
        float damageDealtModifier = entityModifierHandler.ApplyModifiersToValue(ModifierType.DamageDealt, BaseDamageDealtModifier);
        return damageDealtModifier;
    }

    public float GetDamageReceivedModifier()
    {
        float damageReceivedModifier = entityModifierHandler.ApplyModifiersToValue(ModifierType.DamageReceived, BaseDamageReceivedModifier);
        return damageReceivedModifier;
    }
}
