using System;
using System.Collections;
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
    public GameObject autoTarget;

    Coroutine proximityChecker;

    private void DoSetOrder(UnitOrder order) {
        this.order = order;

        if (order == UnitOrder.Idle) {
            nextMovementTarget = transform.position;
            movementTargetObject = null;
            attackTargetUnit = null;
            attackTargetPosition = null;
            orientationTarget = null;
            proximityChecker = StartCoroutine(CheckForEnemies());
        } else {
            if (proximityChecker != null) {
                StopCoroutine(proximityChecker);
                proximityChecker = null;
            }
        }

        if (order == UnitOrder.Attack) {
            movementTargetObject = null;
            autoTarget = null;
        }

        if (order == UnitOrder.Move) {
            attackTargetPosition = null;
            attackTargetUnit = null;
            autoTarget = null;
        }

        UpdateTargetsWithinCurrentOrder();
    }

    private void UpdateTargetsWithinCurrentOrder() {
        switch (order) {
            case UnitOrder.Move:
                if (movementTargetObject != null) {
                   nextMovementTarget = movementTargetObject.GetComponent<Renderer>().bounds.center;
                }
                break;
            case UnitOrder.Attack:
                    AdjustPositionAndOrientationForAttack();
                break;
            case UnitOrder.Idle:
                if (autoTarget != null) {
                    AdjustPositionAndOrientationForAttack();
                }
                break;
            default:
                break;
        }
    }

    private void AdjustPositionAndOrientationForAttack() {
        //check if we even need to do anything
        if (attackTargetPosition == null && attackTargetUnit == null && autoTarget == null) {
            SetOrder(UnitOrder.Idle); //can happen when attacked unit was destroyed
            return;
        }

        //check if the target unit (or point) within attack range of the default weapon
        float range = currentState.DefaultWeapon().template.effectiveRange;
        Vector3 target;
        if (attackTargetPosition != null) {
            target = (Vector3)attackTargetPosition;
        } else if (attackTargetUnit != null) {
            target = attackTargetUnit.transform.position; //actually this doesn't count unit's vertical center, but that's ok for now
        } else {
            target = autoTarget.transform.position;
        }

        Vector3 direction = target - transform.position;
        float directionMagnitude2 = Vector3.SqrMagnitude(direction);
        if (directionMagnitude2 < range * range) {
            AdjustOrientationForAttack(direction);
        } else {
            //move from target towards current position by attack range: this is the nearest point suitable for firing
            Vector3 closeEnough = target - (direction * 0.95f * Mathf.Sqrt((range * range) / directionMagnitude2));
            nextMovementTarget = closeEnough;
        }
    }

    private void AdjustOrientationForAttack(Vector3 direction) {
        UnitWeaponTemplate weaponTemplate = currentState.DefaultWeapon().template;
        float desiredAngle = (weaponTemplate.maxHeading + weaponTemplate.minHeading) / 2;
        Vector3 desiredVector = Quaternion.AngleAxis(desiredAngle, transform.up) * direction;
        orientationTarget = desiredVector;
    }

    IEnumerator CheckForEnemies() {
        if (order != UnitOrder.Idle) yield break; //break execution if the unit has some actual job to do

        while (true) {
            if (autoTarget != null) yield return new WaitForSeconds(1.0f); ;

            Collider[] colliders = Physics.OverlapSphere(transform.position, currentState.DefaultWeapon().template.effectiveRange);
            foreach (Collider c in colliders) {
                Unit unit = c.gameObject.GetComponent<Unit>();
                if (unit != null) {

                    if (unit != this && unit.faction != 0 && unit.faction != faction) {
                        autoTarget = c.gameObject;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}