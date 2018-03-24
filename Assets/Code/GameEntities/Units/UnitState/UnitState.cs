using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitState {

    public UnitStateTemplate template { get; private set; }
    public List<WeaponState> weapons;


    public UnitState(UnitStateTemplate template) {
        this.template = template;
        weapons = new List<WeaponState>();

        foreach (UnitWeaponTemplate weaponTemplate in this.template.parametersTemplate.weapons) {
            WeaponState weaponState = new WeaponState(weaponTemplate);
            weapons.Add(weaponState);
        }
    }

    public UnitMovementSettings currentMovementSettings() {
        return template.parametersTemplate.defaultMovementSettings;
    }

    public WeaponState DefaultWeapon() {
        if (weapons != null && weapons.Count > 0) {
            return weapons[0];
        } else {
            return null;
        }
    }

}

