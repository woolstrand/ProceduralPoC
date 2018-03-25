using System.Collections;
using System.Collections.Generic;
using System;

public class UnitParametersTemplate : Object {

    public UnitMovementSettings defaultMovementSettings { get; private set; } //movement settings are separates as they may depends on area or landscape
    public List<UnitWeaponTemplate> weapons;
    public float maximumHealth { get; private set; }

    public UnitParametersTemplate() {
        defaultMovementSettings = new UnitMovementSettings();
        weapons = new List<UnitWeaponTemplate>();
        maximumHealth = 100;
    }

    public UnitParametersTemplate(UnitMovementSettings defaultMovementSettings, float maximumHealth, List<UnitWeaponTemplate> weapons = null) {
        this.maximumHealth = maximumHealth;
        this.defaultMovementSettings = defaultMovementSettings;
        if (weapons != null) {
            this.weapons = new List<UnitWeaponTemplate>(weapons);
        } else {
            this.weapons = new List<UnitWeaponTemplate>();
        }
    }

}
