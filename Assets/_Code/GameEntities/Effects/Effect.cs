using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Any effect is delivered in a container. The container identifies WHICH units are affected, and the efefct itself identifies HOW those units are affected.

public struct OngoingEffect {
    public Effect e;
    public float appliedAt;

    public OngoingEffect(Effect e, float appliedAt) {
        this.e = e;
        this.appliedAt = appliedAt;
    }
}

public enum StackabilityType {
    None, //does not stack: overwrites all lower values of existing effect of the same id
    Duration, //adds duration, overwrites other lower values
    Power, //combines coefficients, overwrites other lower values
    DurationAndPower //combines coefficents, extends duration
}

//target used under certain circumstances (for example, to self-damage on collision)
public enum ContainerTarget {
    Other,
    Self,
    Everyone
}

public class EffectContainer {
    public Effect containedEffect;

    public ContainerTarget target = ContainerTarget.Everyone;

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

    public string effectType { get; private set; }  //type of effect applied. use it to determine armor influence on effect application
    public StackabilityType stackability;
    public float duration; //ignored for permanent effects

    //permanent effect changes a value itself and is dismissed. non-permanent is added to object's effects list and persists there until expired
    public bool isPermanent { get; private set; }

    static public Effect PermanentEffect(string keyPath, string effectType, float b, float a = 1.0f) {
        Effect e = new Effect();
        e.keyPath = keyPath;
        e.effectType = effectType;
        e.a = a;
        e.b = b;
        e.isPermanent = true;
        return e;
    }

    static public Effect TemporaryEffect(string keyPath, string effectType, float duration, float b, float a = 1.0f, StackabilityType stackability = StackabilityType.None) {
        Effect e = new Effect();
        e.keyPath = keyPath;
        e.effectType = effectType;
        e.a = a;
        e.b = b;
        e.isPermanent = false;
        e.duration = duration;
        e.stackability = stackability;
        return e;
    }

    //combines two effects according to stackability settings
    public void CombineWithEffect(Effect e) {
        switch (e.stackability) {
            case StackabilityType.None:
                a = Mathf.Sign(a) * Mathf.Max(Mathf.Abs(a), Mathf.Abs(e.a));
                b = Mathf.Sign(b) * Mathf.Max(Mathf.Abs(b), Mathf.Abs(e.b));
                duration = Mathf.Max(duration, e.duration);
                break;
            case StackabilityType.Duration:
                a = Mathf.Sign(a) * Mathf.Max(Mathf.Abs(a), Mathf.Abs(e.a));
                b = Mathf.Sign(b) * Mathf.Max(Mathf.Abs(b), Mathf.Abs(e.b));
                duration = duration + e.duration;
                break;
            case StackabilityType.Power:
                a = Mathf.Sign(a) * (Mathf.Abs(a) * Mathf.Abs(e.a));
                b = Mathf.Sign(b) * (Mathf.Abs(b) + Mathf.Abs(e.b));
                duration = Mathf.Max(duration, e.duration);
                break;
            case StackabilityType.DurationAndPower:
                a = Mathf.Sign(a) * (Mathf.Abs(a) * Mathf.Abs(e.a));
                b = Mathf.Sign(b) * (Mathf.Abs(b) + Mathf.Abs(e.b));
                duration = duration + e.duration;
                break;
        }
    }
}
