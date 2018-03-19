using System.Collections;
using System.Collections.Generic;
using System;

public class UnitParametersTemplate : Object {

    public UnitMovementSettings defaultMovementSettings { get; private set; } //movement settings are separates as they may depends on area or landscape
    private List<UnitWeaponTemplate> weapons;

    public UnitParametersTemplate() {
        defaultMovementSettings = new UnitMovementSettings();
        weapons = new List<UnitWeaponTemplate>();
    }

}
