using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EffectFactory {
    

    //pure effects

    public static Effect BasicDamageEffect(float amount) {
        Effect e = Effect.PermanentEffect("health", "physical", -amount);
        return e;
    }

    public static Effect BasicSlowdownEffect(float amount) {
        Effect e = Effect.TemporaryEffect("currentState.template.parametersTemplate.defaultMovementSettings.maxSpeed", "group_id_here", 10.0f, 0, 0.5f);
        e.stackability = StackabilityType.Power;
        return e;
    }

    //effects in containers

    public static EffectContainer DirectHitContainer(Effect e) {
        EffectContainer container = new EffectContainer();

        container.containedEffect = e;

        container.isAreaEffect = false;
        container.areaRadius = 0.0f;

        return container;
    }

    public static EffectContainer AOEHitContainer(Effect e, float radius = 1.0f) {
        EffectContainer container = new EffectContainer();

        container.containedEffect = e;

        container.isAreaEffect = true;
        container.areaRadius = radius;

        return container;
    }
}
