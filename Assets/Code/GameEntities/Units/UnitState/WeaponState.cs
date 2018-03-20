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

    public void StartTargeting() {
        targetingStartedAt = Time.time;
    }

    public void PerformTargeting(float deltaTime) {
        if (Math.Abs(targetPitch - pitch) < template.angularSpeed * deltaTime) {
            pitch = targetPitch;
        } else {
            pitch += Math.Sign(targetPitch - pitch) * deltaTime;
            if (pitch < template.minPitch) pitch= template.minPitch;
            if (pitch > template.maxPitch) pitch = template.maxPitch;
        }

        if (Math.Abs(targetHeading - heading) < template.angularSpeed * deltaTime) {
            heading = targetHeading;
        } else {
            heading += Math.Sign(targetHeading - heading) * deltaTime;
            if (heading < template.minHeading) heading = template.minHeading;
            if (heading > template.maxHeading) heading = template.maxHeading;
        }
    }

    public bool ReadyToFireAtPosition(GameObject target) {
        if (Math.Abs(pitch - targetPitch) < 0.1 && Math.Abs(heading - targetHeading) < 0.1 &&
            Time.time - targetingStartedAt > template.targetingTime) return true;
        else return false;
    }

    //fire at specific game object. homing projectiles will track it.
    public void fire(GameObject target) {
        //if (template.)
    }

    //fire at specific point of game space.
    public void fire(Vector3 target) {
        if (Time.time - reloadingStartedAt > template.reloadTime) {
            reloadingStartedAt = Time.time;
        }
    }
}
