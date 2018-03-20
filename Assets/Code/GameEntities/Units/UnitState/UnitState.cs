using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitState {

    public readonly UnitStateTemplate template;

    public float speed { get; private set; }


    public UnitState(UnitStateTemplate template) {
        this.template = template;
    }

    public UnitMovementSettings currentMovementSettings() {
        return template.parametersTemplate.defaultMovementSettings;
    }

    public void accelerateToMaximumSpeed(float deltaTime) {
        float maxSpeed = currentMovementSettings().maxSpeed;
        if (speed < maxSpeed) {
            speed += currentMovementSettings().maxAcceleration;
            if (speed > maxSpeed) {
                speed = maxSpeed;
            }
        }
    }

    public void stop() {
        speed = 0;
    }


}

