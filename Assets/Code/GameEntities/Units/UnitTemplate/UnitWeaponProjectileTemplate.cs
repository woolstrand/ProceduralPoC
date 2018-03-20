using System.Collections;
using System.Collections.Generic;
using System;


enum ProjectileType {
    Direct, //direct influence on target (e.g. psi-storm)
    Linear, //very fast projectile which spawns effect on first unit collided with
    SubUnit //projectile is a unit itself which is spawned and then moves according to it's internal mechanics.
}

public class UnitWeaponProjectileTemplate : Object {
    ProjectileType projectileType;

    float effectiveLength; //limits collider length in case type is linear
    UnitTemplate projectileUnitTemplate; //in case of type is subunit

    List<Effect> effects; //effects applied to collided object or directly to area/point

    public UnitWeaponProjectileTemplate(UnitTemplate projectileUnitTemplate) {
        projectileType = ProjectileType.SubUnit;
        this.projectileUnitTemplate = projectileUnitTemplate;
    }

    public UnitWeaponProjectileTemplate(List<Effect> effects, float effectiveLength = 25.0f) {
        projectileType = ProjectileType.Linear;
        this.effectiveLength = effectiveLength;
        this.effects = new List<Effect>(effects);
    }

    public UnitWeaponProjectileTemplate(List<Effect> effects) {
        projectileType = ProjectileType.Direct;
        this.effects = new List<Effect>(effects);
    }
}
