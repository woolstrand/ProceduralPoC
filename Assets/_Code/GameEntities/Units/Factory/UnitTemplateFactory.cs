using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTemplateFactory  {

    //some templates for testing
    public static UnitTemplate defaultUnitTemplate() {
        UnitMovementSettings movement = new UnitMovementSettings(5.0f, 3.0f, 0.1f);

        Effect weaponEffect = EffectFactory.BasicDamageEffect(10);
        EffectContainer container = EffectFactory.DirectHitContainer(weaponEffect);
        UnitWeaponProjectileTemplate projectile = new UnitWeaponProjectileTemplate(new List<EffectContainer> { container }, effectiveLength: 10.0f);
        UnitWeaponTemplate weaponTemplate = new UnitWeaponTemplate(projectile, reloadTime: 0.3f, targetingTime: 0.5f,
            minHeading: -0.5f, maxHeading: 0.5f);
        weaponTemplate.barrelOrigin = new Vector3(0, 0.5f, 0);

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 100.0f, weapons: new List<UnitWeaponTemplate> { weaponTemplate });
        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }

    public static UnitTemplate defaultUnitTemplate2() {
        UnitMovementSettings movement = new UnitMovementSettings(maxSpeed: 15.0f, maxAngularSpeed: 100.0f, maxAcceleration: 10.0f);

        Effect weaponEffect = EffectFactory.BasicDamageEffect(10);
        EffectContainer container = EffectFactory.AOEHitContainer(weaponEffect, 5.0f);

        UnitTemplate projectileUnitTemplate = DefaultProjectileUnitTemplate();


        UnitWeaponProjectileTemplate projectile = new UnitWeaponProjectileTemplate(projectileUnitTemplate);
        UnitWeaponTemplate weaponTemplate = new UnitWeaponTemplate(projectile, reloadTime: 3.5f, targetingTime: 0.5f,
            minHeading: -1.5f, maxHeading: 1.5f);
        weaponTemplate.barrelOrigin = new Vector3(0, 1.5f, 0);

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 30.0f, weapons: new List<UnitWeaponTemplate> { weaponTemplate });
        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }

    public static UnitTemplate DefaultProjectileUnitTemplate() {
        UnitMovementSettings movement = new UnitMovementSettings(maxSpeed: 25.0f, maxAngularSpeed: 10.0f, maxAcceleration: 10.0f, minSpeed: 5.0f, lockedVertically: false);

        Effect weaponEffect = EffectFactory.BasicDamageEffect(80);
        EffectContainer container = EffectFactory.AOEHitContainer(weaponEffect, 5.0f);

        
        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 10.0f);
        parameters.AddEffectForEvent("collision", container);

        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }
}
