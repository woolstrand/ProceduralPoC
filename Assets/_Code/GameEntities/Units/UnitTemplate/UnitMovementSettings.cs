using System.Collections;
using System.Collections.Generic;
using System;

public class UnitMovementSettings : Object {

    public float maxSpeed { get; private set; } //meters per second
    public float minSpeed { get; private set; } //meters per second
    public float maxAcceleration { get; private set; } //meters per second^2
    public float maxAngularSpeed { get; private set; } //radians per second

    public bool isLockedVertically;


    public UnitMovementSettings(float maxSpeed, float maxAngularSpeed = float.PositiveInfinity, float maxAcceleration = float.PositiveInfinity, float minSpeed = 0, bool lockedVertically = true) {
        this.maxSpeed = maxSpeed;
        this.minSpeed = minSpeed;
        this.maxAngularSpeed = maxAngularSpeed;
        this.maxAcceleration = maxAcceleration;
        isLockedVertically = lockedVertically;
    }

    public UnitMovementSettings() {
        maxSpeed = 5.0f;
        minSpeed = 0.0f;
        maxAcceleration = 0.02f;
        maxAngularSpeed = 5.0f;
    }

    
}