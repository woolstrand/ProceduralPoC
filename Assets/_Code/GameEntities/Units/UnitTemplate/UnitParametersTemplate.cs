using System.Collections;
using System.Collections.Generic;
using System;

public class UnitParametersTemplate : Object {

    public UnitMovementSettings defaultMovementSettings { get; private set; } //movement settings are separates as they may depends on area or landscape
    public List<UnitWeaponTemplate> weapons;

    private Dictionary<string, List<EffectContainer>> triggeredEffects;

    public float maximumHealth { get; private set; }

    public UnitParametersTemplate() {
        defaultMovementSettings = new UnitMovementSettings();
        weapons = new List<UnitWeaponTemplate>();
        triggeredEffects = new Dictionary<string, List<EffectContainer>>();
        maximumHealth = 100;
    }

    public UnitParametersTemplate(UnitMovementSettings defaultMovementSettings, float maximumHealth, List<UnitWeaponTemplate> weapons = null) {
        triggeredEffects = new Dictionary<string, List<EffectContainer>>();
        this.maximumHealth = maximumHealth;
        this.defaultMovementSettings = defaultMovementSettings;
        if (weapons != null) {
            this.weapons = new List<UnitWeaponTemplate>(weapons);
        } else {
            this.weapons = new List<UnitWeaponTemplate>();
        }
    }

    public void AddEffectForEvent(string eventName, EffectContainer effect) {
        List<EffectContainer> list;
        if (triggeredEffects.ContainsKey(eventName)) {
            list = triggeredEffects[eventName];
            list.Add(effect);
        } else {
            list = new List<EffectContainer>{ effect };
            triggeredEffects.Add(eventName, list);
        }
    }

    public List<EffectContainer> EffectsForEvent(string eventName) {
        if (!triggeredEffects.ContainsKey(eventName)) {
            return null;
        } else {
            return triggeredEffects[eventName];
        }
    }

}
