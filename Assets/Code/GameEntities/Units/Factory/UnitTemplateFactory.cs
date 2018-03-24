using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTemplateFactory  {

    public static UnitTemplate defaultUnitTemplate() {
        UnitMovementSettings movement = new UnitMovementSettings(5.0f, 1.0f, 0.1f);

        Effect weaponEffect = EffectFactory.BasicDamageEffect(10);
        EffectContainer container = EffectFactory.DirectHitContainer(weaponEffect);
        UnitWeaponProjectileTemplate projectile = new UnitWeaponProjectileTemplate(new List<EffectContainer> { container }, effectiveLength: 5.0f);
        UnitWeaponTemplate weaponTemplate = new UnitWeaponTemplate(projectile);

        UnitParametersTemplate parameters = new UnitParametersTemplate(movement, maximumHealth: 100.0f, weapons: new List <UnitWeaponTemplate> { weaponTemplate });
        UnitStateTemplate state = new UnitStateTemplate(parameters);
        UnitTemplate template = new UnitTemplate(state);

        return template;
    }
}
