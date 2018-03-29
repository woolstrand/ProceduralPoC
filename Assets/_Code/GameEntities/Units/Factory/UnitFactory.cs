using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class for creating custom gameobject units
public class UnitFactory  {

    //created unit with chosen template and with mesh based on named asset
    static public GameObject CreateUnit(UnitTemplate template, string modelName, int faction) {
        GameObject o = new GameObject("Unit (" + modelName + ") " + ((int)(Random.value * 100)).ToString());
        o.tag = "unit";

        Unit u = o.AddComponent<Unit>();

        Mesh[] meshes = Resources.LoadAll<Mesh>("Models/" + modelName);
        Material[] materials = Resources.LoadAll<Material>("Models/" + modelName);

        Rigidbody rb = o.AddComponent<Rigidbody>();
        //MeshCollider mc = o.AddComponent<MeshCollider>();
        BoxCollider sc = o.AddComponent<BoxCollider>();
        MeshFilter mf = o.AddComponent<MeshFilter>();
        MeshRenderer mr = o.AddComponent<MeshRenderer>();

        //mc.convex = true;

        rb.useGravity = false;
        rb.mass = 10;
        rb.drag = 10;
        rb.angularDrag = 10;

        mf.mesh = meshes[0];
        mr.materials = materials;


        //mc.sharedMesh = mf.mesh;
        sc.size = mr.bounds.size;

        u.faction = faction;
        u.template = template;
        u.InitializeInternalData();

        return o;
    }
}
