using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Unit {
    Vector3 attackTargetPosition;
    GameObject attackTargetUnit;

    private void UpdateWeaponTargetAngles() { //updates weapon's desired heading and pitch to point at target (respecting object orientation)
        Vector3 targetPosition = attackTargetPosition;
        if (attackTargetUnit != null) {
            attackTargetPosition = attackTargetUnit.transform.position;
        }

        Vector3 objectToTarget = (targetPosition - transform.position).normalized;
        float targetPitch = Mathf.Asin(objectToTarget.y);
        float targetHeading = Mathf.Atan2(objectToTarget.z, objectToTarget.x);
        targetHeading = targetHeading - transform.rotation.eulerAngles.y;
    }

    private void PerformAttackJob() {
        if (attackTargetUnit == null && attackTargetPosition == null) {
            return;
        }

        WeaponState weapon = currentState.DefaultWeapon();

        if (weapon != null) {
            if (weapon.ReadyToFireAtPosition(attackTargetUnit)) {
                //TODO: do fire
            } else {
                weapon.PerformTargeting(Time.deltaTime);
            }
        }
    }
}