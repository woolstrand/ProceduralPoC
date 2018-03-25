using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


public enum UnitOrder {
    Idle,
    Move,
    Attack,
    AttackMove
}

public partial class Unit {
    //Combination of current order and movement and attack target and object allows to implement a bit more complex behaviours like automatic orientation or chasing
    public UnitOrder order;

    private void UpdateTargetsWithinCurrentOrder() {
        switch (order) {
            case UnitOrder.Move:
                if (movementTargetObject != null) {
                   nextMovementTarget = movementTargetObject.GetComponent<Renderer>().bounds.center;
                }
                break;
            case UnitOrder.Attack: {

                    //check if we even need to do anything
                    if (attackTargetPosition == null && attackTargetUnit == null) {
                        SetOrder(UnitOrder.Idle); //can happen when attackd unit was destroyed
                        return;
                    }

                    //check if the target unit (or point) within attack range of the default weapon
                    float range = currentState.DefaultWeapon().template.effectiveRange;
                    Vector3 target;
                    if (attackTargetPosition != null) {
                        target = (Vector3)attackTargetPosition;
                    } else {
                        target = attackTargetUnit.transform.position; //actually this doesn't count unit's vertical center, but that's ok for now
                    }

                    Vector3 direction = target - transform.position;
                    float directionMagnitude2 = Vector3.SqrMagnitude(direction);
                    if (directionMagnitude2 < range*range) {
                        //enemy is already within attack range
                        /*                        Vector3 directionProjection = Vector3.ProjectOnPlane(direction, Vector3.up);
                                                Vector3 forwardProjection = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
                                                UnitWeaponTemplate weaponTemplate = currentState.DefaultWeapon().template;
                                                float angle = Vector3.Angle(forwardProjection, directionProjection) * Mathf.Deg2Rad;
                                                if (angle > weaponTemplate.maxHeading || angle < weaponTemplate.minHeading) {*/

                        UnitWeaponTemplate weaponTemplate = currentState.DefaultWeapon().template;
                        float desiredAngle = (weaponTemplate.maxHeading + weaponTemplate.minHeading) / 2;
                        Vector3 desiredVector = Quaternion.AngleAxis(desiredAngle, transform.up) * direction;
                        orientationTarget = desiredVector;

                        //}

                    } else {
                        //move from target towards current position by attack range: this is the nearest point suitable for firing
                        Vector3 closeEnough = target - (direction * 0.95f * Mathf.Sqrt((range * range) / directionMagnitude2));
                        nextMovementTarget = closeEnough;
                    }
                }
                break;
            default:
                break;
        }
    }
}