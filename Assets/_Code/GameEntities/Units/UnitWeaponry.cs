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

        Vector3 targetProjection = Vector3.ProjectOnPlane(objectToTarget, transform.up);
        Vector3 forwardProjection = Vector3.ProjectOnPlane(transform.forward, transform.up);

        float targetHeading = Vector3.Angle(forwardProjection, targetProjection) * Mathf.Deg2Rad;

        float targetPitch = Mathf.Asin(objectToTarget.y);

        if (targetHeading > Mathf.PI) targetHeading -= Mathf.PI;
        if (targetHeading < -Mathf.PI) targetHeading += Mathf.PI;

        currentState.DefaultWeapon().UpdateTargetAngles(targetHeading, targetPitch);
    }

    private void PerformAttackJob() {
        if (attackTargetUnit == null && attackTargetPosition == null) {
            return;
        }

        WeaponState weapon = currentState.DefaultWeapon();

        if (weapon != null) {

            //selecting target position depending on what is set as target
            Vector3 targetPosition;
            if (attackTargetUnit != null) {
                targetPosition = attackTargetUnit.GetComponent<Renderer>().bounds.center;
            } else {
                targetPosition = (Vector3)attackTargetPosition;
            }

            //Aim before shooting. Aiming speed should be enough to follow a moving target.
            weapon.PerformTargeting(Time.deltaTime);

            if (weapon.ReadyToFire()) {
                Debug.Log(this.gameObject.name + " ready to fire");
                switch (weapon.template.projectile.projectileType) {
                    case ProjectileType.Direct:
                        if (attackTargetUnit != null) {
                            FireDirectProjectileAtTarget(weapon.template.projectile, attackTargetUnit);

                            weapon.Fire(); //this object is a data object, so it doesn't fire by itself, it only moves inner state from "ready" to "reloading"
                        }
                        break;
                    case ProjectileType.Linear:
                        if (Math.Pow(weapon.template.projectile.effectiveLength, 2) >
                            Vector3.SqrMagnitude(targetPosition - transform.position)) {
                            Debug.Log(this.gameObject.name + " in range, firing");

                            FireLinearProjectileAtTarget(weapon, targetPosition);
                            weapon.Fire(); //this object is a data object, so it doesn't fire by itself, it only moves inner state from "ready" to "reloading"
                            
                        }
                        break;
                    case ProjectileType.SubUnit:
                        FireSubUnitProjectileAtTarget(weapon, attackTargetUnit.transform.position);
                        weapon.Fire();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void FireDirectProjectileAtTarget(UnitWeaponProjectileTemplate projectile, GameObject target) {
        foreach (EffectContainer c in projectile.effects) {
            c.ApplyEffect(target);
        }
    }

    private void FireLinearProjectileAtTarget(WeaponState weapon, Vector3 target) {
        Vector3 firingOrigin = transform.position + weapon.template.barrelOrigin;
        RaycastHit[] hits = Physics.RaycastAll(firingOrigin, target - firingOrigin, weapon.template.projectile.effectiveLength);
        RaycastHit? nearestHit = null;
        float minDistance = weapon.template.projectile.effectiveLength + 1;
        
        foreach (RaycastHit hit in hits) {
            if (hit.distance < minDistance && hit.transform.gameObject != this.gameObject) {
                minDistance = hit.distance;
                nearestHit = hit;
            }
        }

        Vector3 hitReceiver = target;
        if (nearestHit != null) {
            GameObject hitObject = ((RaycastHit)nearestHit).transform.gameObject;

            foreach (EffectContainer c in weapon.template.projectile.effects) {
                c.ApplyEffect(hitObject);
            }
            hitReceiver = ((RaycastHit)nearestHit).point;
            Debug.Log(this.gameObject.name + " shoot and hit " + hitObject.name + ", applying an effect");
        }

        //since the projectile could hit another target on it's way, let's draw ray to the actual hit receiver
        PerformFiringVisualsForWeapon(weapon, hitReceiver);

    }

    private void FireSubUnitProjectileAtTarget(WeaponState weapon, Vector3 target) {
        var projectileUnit = Instantiate(GameObject.Find("Projectile"));
        projectileUnit.transform.position = transform.position + weapon.template.barrelOrigin;
        projectileUnit.transform.rotation = Quaternion.LookRotation(target - projectileUnit.transform.position);
        var unitDesc = projectileUnit.GetComponent<Unit>();
        unitDesc.template = weapon.template.projectile.projectileUnitTemplate;
        unitDesc.InitializeInternalData();

        unitDesc.SetMovementTarget(target);
    }

    //visuals
    private void PerformFiringVisualsForWeapon(WeaponState weapon, Vector3 realTarget) {
        

        Action gizmoAction = delegate {
            Gizmos.color = Color.red;

            int count = 5;
            float width = 0.1f;
            Camera c = Camera.current;
            Vector3 p1 = transform.position + weapon.template.barrelOrigin;
            Vector3 p2 = realTarget;
            Vector3 v1 = (p2 - p1).normalized; // line direction
            Vector3 v2 = (c.transform.position - p1).normalized; // direction to camera
            Vector3 n = Vector3.Cross(v1, v2); // normal vector

            for (int i = 0; i < count; i++) {
                Vector3 o = n * width * ((float)i / (count - 1) - 0.5f);
                Gizmos.DrawLine(p1 + o, p2 + o);
            }
        };

        gizmoActions.Add(gizmoAction);
    }
}