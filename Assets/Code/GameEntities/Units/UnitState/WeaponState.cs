using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponState {
    public UnitWeaponTemplate template { get; private set; }

    private float targetPitch;
    private float targetHeading;

    private float pitch;
    private float heading;

    private float targetingStartedAt; //TODO: do something with float precision deterioration over time (what time span will actually cause problems?)
    private float reloadingStartedAt;


    public WeaponState(UnitWeaponTemplate template) {
        this.template = template;
        targetHeading = heading = (template.minHeading + template.maxHeading) / 2;
        targetPitch = pitch = (template.minPitch + template.maxPitch) / 2;
    }

    public void StartTargeting() {
        targetingStartedAt = Time.time;
    }

    public void UpdateTargetAngles(float targetHeading, float targetPitch) {
        this.targetHeading = targetHeading;
        this.targetPitch = targetPitch;
    }

    public void PerformTargeting(float deltaTime) {
        if (Math.Abs(targetPitch - pitch) < template.angularSpeed * deltaTime) {
            pitch = targetPitch; //to eliminate flicker
        } else {
            pitch += Math.Sign(targetPitch - pitch) * deltaTime;
        }

        if (pitch < template.minPitch) pitch = template.minPitch;
        if (pitch > template.maxPitch) pitch = template.maxPitch;

        if (Math.Abs(targetHeading - heading) < template.angularSpeed * deltaTime) {
            heading = targetHeading; //to eliminate flicker
        } else {
            heading += Math.Sign(targetHeading - heading) * deltaTime;
        }

        if (heading < template.minHeading) heading = template.minHeading;
        if (heading > template.maxHeading) heading = template.maxHeading;
    }

    public bool ReadyToFire() {
        if (Math.Abs(pitch - targetPitch) < 0.1 && Math.Abs(heading - targetHeading) < 0.1 &&
            Time.time - targetingStartedAt > template.targetingTime &&
            Time.time - reloadingStartedAt > template.reloadTime) return true;
        else return false;
    }

    //reset fire counters, initiate reloading
    public void Fire() {
        reloadingStartedAt = Time.time;
    }
}
