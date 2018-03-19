using System.Collections;
using System.Collections.Generic;
using System;

public class UnitMovementSettings : Object {

    public float maxSpeed; //meters per second
    public float minSpeed; //meters per second
    public float maxAcceleration; //meters per second^2
    public float maxAngularSpeed; //radians per second

    public UnitMovementSettings() {
        maxSpeed = 5.0f;
        minSpeed = 0.0f;
        maxAcceleration = 0.02f;
        maxAngularSpeed = 5.0f;

    }

}
