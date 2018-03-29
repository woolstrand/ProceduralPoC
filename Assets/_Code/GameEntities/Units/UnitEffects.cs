using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// How does effect system work:
//
// Each effect changes a parameter. Each change is described by a NEWVALUE = a * OLDVALUE + b.
// Permanent efefcts change parameter once and are destroyed. You use this effects to decrease health, permanently changing attack range nd so on.
// Permanent effect can be applied to both unit's and template's parameters.
// Temporary effects change some parameter for a period of time. When time is up, parameter is restored back to it's original value.
// Due to this fact, it's not recommended to apply temporary effects on parameters which tends to change over time as it will lead to 
// unpredictable behavior when the parameter is restored, and also the effect will be applied for just a single frame until recalculation
// of live unit's parameters starts.
// Temporary parameters can be stacked if they have the same type and are applied to the same parameter.
//
// Internal mechanics of temporary effects:
// Each time temporary effect gets into a pipeline, Unit object searches for similar effects. If found, then effects are stacked. Otherwise effect is
// just added to the list. After efefct being added or stacked, recalculation launches. For each keypath affected, recalculation stores original 
// parameter value in a dictionary (where keypath is the key and original value is a part of the value), and the original parameter value is replaced 
// with recalculated one (with all suitable effects applied to it one by one). Thus you can immediately get live parameter value with all effects
// applied at no additional cost.
// After that the shortest effect's duration is stored into nearestExpiration field of the Effects Set for this keypath (global nearestRecalcTime is 
// also updated). When this time comes, expiration check is launched, expired effect is removed and the new value for this keypath is recalculated 
// (based on the stored initial value and active effects). If no active effects are affecting  this keypath, all records for this keypath are deleted 
// and the original value is restored.
// If a permanent effect should change parameter which is already affected by a temporary effect, this permanent effect changes stored parameter, and
// the new value is recalculated for current state of this parameter.

//TODO: restore all values on state change and reapply for the new state.

struct KeypathEffectsSet {
    public object originalValue;
    public List<OngoingEffect> ongoingEffects;
    public float nearestExpirationTime;
}

public partial class Unit {

    private Dictionary<string, KeypathEffectsSet> ongoingEffects;
    private float nearestRecalcTime;

    private void StartEffects() {
        ongoingEffects = new Dictionary<string, KeypathEffectsSet>();
        nearestRecalcTime = float.PositiveInfinity;
    }
    
    public void ApplyEffect(Effect e) {
        if (e.isPermanent) {

            float value = e.b;

            if (e.a != 0) {
                float oldValue = (float)GetPropertyValue(e.keyPath);
                value += oldValue * e.a;
            }

            SetPropertyValue(e.keyPath, value);

        } else {
            if (ongoingEffects.ContainsKey(e.keyPath)) {
                List<OngoingEffect> effectsForKeypath = ongoingEffects[e.keyPath].ongoingEffects;

                foreach (OngoingEffect ongoingEffect in effectsForKeypath) {
                    if (ongoingEffect.e.effectType.Equals(e.effectType)) {
                        ongoingEffect.e.CombineWithEffect(e);
                        break;
                    }
                    //There is no possibility that duration will be shortened, so we don't need to recalculate expiration time. 
                    //In the worst case we just will have one unnecessary recalculation. If we add explicit recalculation here, we'll have
                    //extra recalculation _each_ time.
                }
            } else {
                List<OngoingEffect> effects = new List<OngoingEffect> { new OngoingEffect(e, Time.time) };

                object value = GetPropertyValue(e.keyPath);

                KeypathEffectsSet set = new KeypathEffectsSet();
                set.ongoingEffects = effects;
                set.originalValue = value;
                set.nearestExpirationTime = Time.time + e.duration;

                ongoingEffects.Add(e.keyPath, set);

            }

            RecalculateValueAndExpirationForKeypath(e.keyPath);
        }
    }

    //Does all effect-maintainance job:
    // - checks if we do need some changes
    // - if yes, determines keypath that needs update
    // - updates effects and values state for this keypath

    private void UpdateEffectsState() {
        if (Time.time < nearestRecalcTime) return; //no update needed yet

        List<string> keysCopy = new List<string>(ongoingEffects.Keys);
        foreach (string keyPath in keysCopy) {
            KeypathEffectsSet effectsSet = ongoingEffects[keyPath];
            if (effectsSet.nearestExpirationTime <= Time.time) {
                EliminateExpiredEffectsForKeypath(keyPath);
                if (ongoingEffects.ContainsKey(keyPath)) {
                    //if this key still exists (was not removed after effect expiration)
                    RecalculateValueAndExpirationForKeypath(keyPath);
                }
            }
        }
    }

    //Eliminates expired effects and restores original value if no effects left. DOES NOT recalculate expiration time.
    private void EliminateExpiredEffectsForKeypath(string keyPath) {
        KeypathEffectsSet effectsSet = ongoingEffects[keyPath];
        
        List<OngoingEffect> effectsForKeypath = effectsSet.ongoingEffects;
        foreach (OngoingEffect oe in effectsForKeypath.ToArray()) {
            if (oe.appliedAt + oe.e.duration <= Time.time) {
                effectsForKeypath.Remove(oe);
            }
        }

        if (effectsForKeypath.Count == 0) {
            SetPropertyValue(keyPath, effectsSet.originalValue);
            ongoingEffects.Remove(keyPath);
        }

    }

    //Recalculates value and expiration time for a specific keypath
    private void RecalculateValueAndExpirationForKeypath(string keypath) {

        object value;
        float nearestRecalcTime = float.PositiveInfinity;

        KeypathEffectsSet effectsSet = ongoingEffects[keypath];
        value = effectsSet.originalValue;

        List<OngoingEffect> effectsForKeypath = effectsSet.ongoingEffects;
        if (value is float) {
            foreach (OngoingEffect oe in effectsForKeypath) {

                value = (float)value * oe.e.a + oe.e.b;

                if (oe.appliedAt + oe.e.duration < nearestRecalcTime) {
                    nearestRecalcTime = oe.appliedAt + oe.e.duration;
                }
            }
        } else if (value is bool) {
            foreach (OngoingEffect oe in effectsForKeypath) {

                if (oe.e.a == 0) { //since a and b are (SHOULD BE) explicitly set to -1 or 0 in case of working with bool, we may not worry about float precision
                    value = (oe.e.b != 0);
                } else if (oe.e.a == -1) {
                    value = !(bool)value;
                }

                if (oe.appliedAt + oe.e.duration < nearestRecalcTime) {
                    nearestRecalcTime = oe.appliedAt + oe.e.duration;
                }
            }
        }

        SetPropertyValue(keypath, value);

        effectsSet.nearestExpirationTime = nearestRecalcTime;
        if (nearestRecalcTime < this.nearestRecalcTime) {
            this.nearestRecalcTime = nearestRecalcTime;
        }
    }

    //Extending reflection capabilities: recursive getters/setters
    private object GetPropertyValue(string propertyName) {
        object obj = this;

        foreach (var prop in propertyName.Split('.').Select(s => obj.GetType().GetProperty(s)))
            obj = prop.GetValue(obj, null);

        return obj;
    }

    private void SetPropertyValue(string propertyName, object value) {
        object obj = this;
        object parentObj = null;
        PropertyInfo lastProp = null;

        foreach (var prop in propertyName.Split('.').Select(s => obj.GetType().GetProperty(s))) {
            parentObj = obj;
            obj = prop.GetValue(obj, null);
            lastProp = prop;
        }

        if (lastProp != null) {
            lastProp.SetValue(parentObj, value, null);
        }
    }
} 