using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Any effect is delivered in a container. The container identifies WHICH units are affected, and the efefct itself identifies HOW those units are affected.

public struct EffectType {
    public string effectTypeName { get; private set; }
    public int effectTypeId { get; private set; }

    public EffectType(string name, int id) {
        effectTypeName = name;
        effectTypeId = id;
    }
}

public class EffectContainer {
    public Effect containedEffect;

    public bool isAreaEffect;
    public float areaRadius;

    public void ApplyEffect(GameObject targetUnit, Vector3 applicationPoint) {
        if (targetUnit != null && isAreaEffect == false) {
            Unit unit = targetUnit.GetComponent<Unit>();
            if (unit != null) {
                unit.ApplyEffect(containedEffect);
            }
        }

        if (isAreaEffect == true) {
            Collider[] colliders = Physics.OverlapSphere(applicationPoint, areaRadius);
            foreach (Collider c in colliders) {
                Unit unit = c.gameObject.GetComponent<Unit>();
                if (unit != null) {
                    unit.ApplyEffect(containedEffect);
                }
            }

            GameObject visualizer = GameObject.Instantiate(Resources.Load("TemporaryUtilities/AreaEffectVisualizer")) as GameObject;
            visualizer.GetComponent<AreaEffectVisualizer>().ttl = 1.0f;
            visualizer.GetComponent<AreaEffectVisualizer>().radius = areaRadius;
            visualizer.transform.position = applicationPoint;
        }
    }

    public void ApplyEffect(Vector3 targetPoint) {

    }
}


    public class Effect {
    public string keyPath { get; private set; }//which parameter is changed by the effect

    public float a { get; private set; }  //newValue = a*oldValue + b;
    public float b { get; private set; } 

    public EffectType effectType { get; private set; }  //type of effect applied. use it to determine armor influence on effect application

    //permanent effect changes a value itself and is dismissed. non-permanent is added to object's effects list and persists there until expired
    public bool isPermanent { get; private set; }

    public Effect(string keyPath, EffectType type, float b, float a = 1.0f, bool permanent = true) {
        this.keyPath = keyPath;
        this.effectType = type;
        this.a = a;
        this.b = b;
        this.isPermanent = permanent;
    }
}
