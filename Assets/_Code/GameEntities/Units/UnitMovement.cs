using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Unit {

    public Vector3 nextMovementTarget;
    public GameObject movementTargetObject; //if I want to chase someone
    private Vector3? orientationTarget; //If any particular orientaion is needed to fire
    public float speed { get; private set; }


    private void PerformMovementJob() {

        //Arrange unit heading towards the next target point if it exists
        if (nextMovementTarget != transform.position) {

            Vector3 targetDirection = (nextMovementTarget - transform.position);
            float distanceToTarget = targetDirection.magnitude;
            if (currentState.currentMovementSettings().isLockedVertically == true) { targetDirection.y = 0; }


            //Movement in progress: the following block is executed only if the unit hasn't reached it's target yet
            targetDirection.Normalize();

            //Determine desired unit orientation: towards target
            float angle = Vector3.Angle(targetDirection, transform.forward);
            
            //If rotation can't be done at once, rotate gradually
            float maxRotation = currentState.currentMovementSettings().maxAngularSpeed * Time.deltaTime;
            if (Math.Abs(angle) > maxRotation && speed >= currentState.currentMovementSettings().minSpeed) {
                Vector3 nextForward = Vector3.RotateTowards(transform.forward, targetDirection, maxRotation, 0);
                if (currentState.currentMovementSettings().isLockedVertically == true) { nextForward.y = 0; }

                transform.forward = nextForward;
            } else { //needed rotation is smaller than maximum rotation step, so just switch to rotation needed
                transform.forward = targetDirection; //keep orientation
            }

            //Start moving if there is a chance to finish rotation while traveling
            if (distanceToTarget / currentState.currentMovementSettings().maxSpeed > angle / currentState.currentMovementSettings().maxAngularSpeed) { 
                AccelerateToMaximumSpeed(Time.deltaTime);
            }

            if (speed > 0) {
                if (Math.Abs(angle) > 90 && Math.Abs(angle) < 135) {
                    DecelerateToHalfSpeed(Time.deltaTime);
                } else if (Math.Abs(angle) >= 135) {
                    DecelerateToMinimumSpeed(Time.deltaTime);
                }

                Vector3 movement = Vector3.forward * speed * Time.deltaTime;

                if (movement.sqrMagnitude < (transform.position - nextMovementTarget).sqrMagnitude ||
                    currentState.currentMovementSettings().minSpeed > 0) {
                    transform.Translate(movement);
                } else {
                    transform.position = nextMovementTarget; //eliminating jitter around the target
                    StopMovement();

                    if (order == UnitOrder.Move || order == UnitOrder.AttackMove) { SetOrder(UnitOrder.Idle); }
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
        } else {
            //Weapon-related rotation: if the unit needs some specific rotation in order to attack it's target, it sets 'orientationTarget' property
            //and then rotation routines orient unit accordingly. This is done only if there is no movement in progress.
            if (orientationTarget != null) {
                Vector3 targetDirection = (Vector3)orientationTarget;

                float angle = Vector3.Angle(targetDirection, transform.forward);

                float maxRotation = currentState.currentMovementSettings().maxAngularSpeed * Time.deltaTime;
                if (Math.Abs(angle) > maxRotation) {
                    Vector3 nextForward = Vector3.RotateTowards(transform.forward, targetDirection, maxRotation, 0);
                    if (currentState.currentMovementSettings().isLockedVertically == true) { nextForward.y = 0; }

                    transform.forward = nextForward;

                } else { //rotation speed is enough to finish rotation
                    transform.forward = targetDirection;
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