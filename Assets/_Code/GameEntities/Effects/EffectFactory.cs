using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EffectFactory {
    
    public static EffectType physicalDamageType() {
        return new EffectType("physical", 1);
    }

    //pure effects

    public static Effect BasicDamageEffect(float amount) {
        Effect e = new Effect("health", physicalDamageType(), -amount);
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
