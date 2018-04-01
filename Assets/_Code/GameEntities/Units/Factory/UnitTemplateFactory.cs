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

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 250.0f, weapons: new List<UnitWeaponTemplate> { weaponTemplate });
        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }

    public static UnitTemplate defaultUnitTemplate2() {
        UnitMovementSettings movement = new UnitMovementSettings(maxSpeed: 10.0f, maxAngularSpeed: 100.0f, maxAcceleration: 1.0f);

        UnitTemplate projectileUnitTemplate = DefaultProjectileUnitTemplate2();


        UnitWeaponProjectileTemplate projectile = new UnitWeaponProjectileTemplate(projectileUnitTemplate);
        UnitWeaponTemplate weaponTemplate = new UnitWeaponTemplate(projectile, reloadTime: 1.5f, targetingTime: 0.5f,
            minHeading: -1.5f, maxHeading: 1.5f);
        weaponTemplate.barrelOrigin = new Vector3(0, 1.5f, 0);

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 30.0f, weapons: new List<UnitWeaponTemplate> { weaponTemplate });
        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }

    public static UnitTemplate pigeonTemplate() {
        UnitMovementSettings movement = new UnitMovementSettings(maxSpeed: 2.0f, maxAngularSpeed: 0.5f, maxAcceleration: 0.1f);



        Effect hit = EffectFactory.BasicDamageEffect(200.0f);
        EffectContainer hitContainer = EffectFactory.DirectHitContainer(hit);

        UnitWeaponProjectileTemplate projectile = new UnitWeaponProjectileTemplate(new List<EffectContainer> { hitContainer }, 30);

        UnitWeaponTemplate weaponTemplate = new UnitWeaponTemplate(projectile, reloadTime: 10.0f, targetingTime: 0.0f,
            minHeading: -2.5f, maxHeading: 2.5f, minPitch: -2, maxPitch: 2, angularSpeed: 1000, effectiveRange: 30.0f);
        weaponTemplate.barrelOrigin = new Vector3(0, 0, 0);

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 10000.0f, weapons: new List<UnitWeaponTemplate> { weaponTemplate });
        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }

    public static UnitTemplate DefaultProjectileUnitTemplate() {
        UnitMovementSettings movement = new UnitMovementSettings(maxSpeed: 3.0f, maxAngularSpeed: 1.0f, maxAcceleration: 2.5f, minSpeed: 15.0f, lockedVertically: false);

        //        Effect weaponEffect = EffectFactory.BasicDamageEffect(80);
        Effect weaponEffect = EffectFactory.BasicSlowdownEffect(44);
        EffectContainer container = EffectFactory.AOEHitContainer(weaponEffect, 5.0f);

        Effect selfhitEffect = EffectFactory.BasicDamageEffect(10);
        EffectContainer selfhit = EffectFactory.DirectHitContainer(selfhitEffect);
        selfhit.target = ContainerTarget.Self;

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 10.0f);
        parameters.isControllable = false;
        parameters.isSelectable = false;
        parameters.AddEffectForEvent("collision", selfhit);
        parameters.AddEffectForEvent("destruction", container);

        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }

    public static UnitTemplate DefaultProjectileUnitTemplate2() {
        UnitMovementSettings movement = new UnitMovementSettings(maxSpeed: 4.0f, maxAngularSpeed: 1.0f, maxAcceleration: 2.5f, minSpeed: 15.0f, lockedVertically: false);

        Effect weaponEffect = EffectFactory.BasicDamageEffect(80);
        EffectContainer container = EffectFactory.AOEHitContainer(weaponEffect, 1.5f);

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 10.0f);
        parameters.isControllable = false;
        parameters.isSelectable = false;
        parameters.AddEffectForEvent("collision", container);
        parameters.AddEffectForEvent("destruction", container);

        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }
}
