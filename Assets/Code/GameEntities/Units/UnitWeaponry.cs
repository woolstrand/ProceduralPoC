using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Unit {
    Vector3? attackTargetPosition;
    GameObject attackTargetUnit;

    private void UpdateWeaponTargetAngles() { //updates weapon's desired heading and pitch to point at target (respecting object orientation)
        if (attackTargetPosition == null && attackTargetUnit == null) return;

        Vector3 targetPosition;
        if (attackTargetPosition != null) {
            targetPosition = (Vector3)attackTargetPosition;
        } else {
            targetPosition = attackTargetUnit.transform.position;
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

            Vector3 targetPosition;
            if (attackTargetUnit != null) {
                targetPosition = attackTargetUnit.transform.position;
            } else {
                targetPosition = (Vector3)attackTargetPosition;
            }

            if (weapon.ReadyToFire()) {
                switch (weapon.template.projectile.projectileType) {
                    case ProjectileType.Direct:
                        if (attackTargetUnit != null) {
                            FireDirectProjectileAtTarget(weapon.template.projectile, attackTargetUnit);

                            weapon.Fire(); //this object is a data object, so it doesn't fire by itself, it only moves inner state from "ready" to "reloading"
                            PerformFiringVisualsForWeapon(weapon);
                        }
                        break;
                    case ProjectileType.Linear:
                        if (Math.Pow(weapon.template.projectile.effectiveLength, 2) >
                            Vector3.SqrMagnitude(targetPosition - transform.position)) {

                            FireLinearProjectileAtTarget(weapon.template.projectile, targetPosition);
                            weapon.Fire(); //this object is a data object, so it doesn't fire by itself, it only moves inner state from "ready" to "reloading"
                            PerformFiringVisualsForWeapon(weapon);
                        }
                        break;
                    case ProjectileType.SubUnit:
                        break;
                    default:
                        break;
                }
            } else if (Vector3.SqrMagnitude(transform.position - targetPosition) < Math.Pow(weapon.template.effectiveRange, 2)) {
                //aiming only within effective range    
                weapon.PerformTargeting(Time.deltaTime);
            }
        }
    }

    private void FireDirectProjectileAtTarget(UnitWeaponProjectileTemplate projectile, GameObject target) {
        foreach (EffectContainer c in projectile.effects) {
            c.ApplyEffect(target);
        }
    }

    private void FireLinearProjectileAtTarget(UnitWeaponProjectileTemplate projectile, Vector3 target) {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, target - transform.position, projectile.effectiveLength);
        GameObject nearest = null;
        float minDistance = projectile.effectiveLength + 1;
        foreach (RaycastHit hit in hits) {
            if (hit.distance < minDistance) {
                minDistance = hit.distance;
                nearest = hit.transform.gameObject;
            }
        }

        if (nearest != null) {
            foreach (EffectContainer c in projectile.effects) {
                c.ApplyEffect(nearest);
            }
        }



    }

    private void FireSubUnitProjectileAtTarget(UnitWeaponProjectileTemplate projectile, Vector3 target) {

    }

    //visuals
    private void PerformFiringVisualsForWeapon(WeaponState weapon) {
        if (attackTargetUnit == null && attackTargetPosition == null) return;

        Vector3 target;
        if (attackTargetUnit == null) {
            target = (Vector3)attackTargetPosition;
        } else {
            target = attackTargetUnit.transform.position;
        }


        Action gizmoAction = delegate {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target);
        };

        gizmoActions.Add(gizmoAction);
    }
}