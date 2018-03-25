using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Unit {

    private Vector3 nextMovementTarget;
    public float speed { get; private set; }


    private void PerformMovementJob() {
        //Arrange unit heading towards the next target point if it exists
        if (nextMovementTarget != transform.position) {

            Vector3 targetDirection = (nextMovementTarget - transform.position);
            targetDirection.y = 0;

            if (targetDirection.sqrMagnitude > 0.1) { //we need to rotate and move unit only if there is some real path ahead
                targetDirection.Normalize();

                float angle = Vector3.Angle(targetDirection, transform.forward);

                float maxRotation = currentState.currentMovementSettings().maxAngularSpeed * Time.deltaTime;
                if (Math.Abs(angle) > maxRotation) {
                    Vector3 nextForward = Vector3.RotateTowards(transform.forward, targetDirection, maxRotation, 0);
                    nextForward.y = 0;

                    transform.forward = nextForward;
                } else { //rotation is acceptable to start moving
                    transform.forward = targetDirection; //keep orientation
                    AccelerateToMaximumSpeed(Time.deltaTime);
                }

                if (speed > 0) {
                    if (Math.Abs(angle) > 90 && Math.Abs(angle) < 135) {
                        DecelerateToHalfSpeed(Time.deltaTime);
                    } else if (Math.Abs(angle) >= 135) {
                        DecelerateToMinimumSpeed(Time.deltaTime);
                    }

                    Vector3 movement = Vector3.forward * speed * Time.deltaTime;

                    if (movement.sqrMagnitude < (transform.position - nextMovementTarget).sqrMagnitude) {
                        transform.Translate(movement);
                    } else {
                        transform.position = nextMovementTarget; //eliminating jitter around the target
                        StopMovement();
                    }

                    if (attackTargetPosition != null || attackTargetUnit != null) {
                        UpdateWeaponTargetAngles(); //update target angles in case they changed during ongoing movement
                    }
                }

                if (attackTargetUnit == null) {
                    UpdateWeaponTargetAngles();
                    //update target angles if the unit moves itself
                    //if attackTargetUnit is set, it will be done in Update, so no need to do it twice.
                    //should move this probably to separate moving and firing. _needReaim flag is enough.
                }
            }


        }
    }

    private void AccelerateToMaximumSpeed(float deltaTime) {
        float maxSpeed = currentState.currentMovementSettings().maxSpeed;
        if (speed < maxSpeed) {
            speed += currentState.currentMovementSettings().maxAcceleration;
            if (speed > maxSpeed) {
                speed = maxSpeed;
            }
        }
    }

    private void DecelerateToHalfSpeed(float deltaTime) {
        float minSpeed = currentState.currentMovementSettings().maxSpeed / 2;
        minSpeed = Math.Min(minSpeed, currentState.currentMovementSettings().minSpeed);

        if (speed > minSpeed) {
            speed -= currentState.currentMovementSettings().maxAcceleration;
            if (speed < minSpeed) {
                speed = minSpeed;
            }
        }
    }

    private void DecelerateToMinimumSpeed(float deltaTime) {
        float minSpeed = currentState.currentMovementSettings().minSpeed;
        if (speed > minSpeed) {
            speed -= currentState.currentMovementSettings().maxAcceleration;
            if (speed < minSpeed) {
                speed = minSpeed;
            }
        }
    }

    private void StopMovement() {
        speed = currentState.currentMovementSettings().minSpeed;
    }

}