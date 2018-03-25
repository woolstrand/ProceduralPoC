using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;



public class UnitWeaponTemplate {

    public float minHeading { get; private set; }
    public float maxHeading { get; private set; }
    public float minPitch { get; private set; }
    public float maxPitch { get; private set; }
    public float angularSpeed { get; private set; } //pitch and heading speeds are the same, will split if it's really needed at some point

    public float targetingTime { get; private set; } //minimum targeting time. actual targeting time could be more due to finite angular speed
    public float reloadTime { get; private set; }

    public float effectiveRange { get; private set; } //if target is beyond this range unit won't even try to shoot

    public UnitWeaponProjectileTemplate projectile;
    public Vector3 barrelOrigin; //point relative to unit's center from where child unit or casted ray is spawned. defaults to object's geometrical center.

    public UnitWeaponTemplate(UnitWeaponProjectileTemplate projectile,
        float reloadTime = 1.0f, float targetingTime = 0.5f,
        float minHeading = -0.1f, float maxHeading = 0.1f,
        float minPitch = -1.0f, float maxPitch = 1.0f,
        float angularSpeed = float.PositiveInfinity,
        float effectiveRange = 20.0f) {

        this.projectile = projectile;

        this.reloadTime = reloadTime;
        this.targetingTime = targetingTime;
        this.minHeading = minHeading;
        this.maxHeading = maxHeading;
        this.minPitch = minPitch;
        this.maxPitch = maxPitch;
        this.angularSpeed = angularSpeed;

        if (projectile.effectiveLength > 0) {
            this.effectiveRange = projectile.effectiveLength;
        } else {
            this.effectiveRange = effectiveRange;
        }

    }



}
