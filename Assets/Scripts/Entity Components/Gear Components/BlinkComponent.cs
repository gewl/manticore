using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkComponent : EntityComponent {

    override protected void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Blink, OnBlink);

	}

    override protected void Unsubscribe()
    {
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Blink, OnBlink);
	}

    void OnBlink()
    {
        
    }
}
