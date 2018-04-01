using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class for creating custom gameobject units
public class UnitFactory  {

    //created unit with chosen template and with mesh based on named asset
    static public GameObject CreateUnit(UnitTemplate template, string modelName, int faction, MeshClass meshClass = MeshClass.Unit) {

        GameObject o = GameObjectsProvider.CreateGameObjectAndTemplate(modelName, meshClass);

        o.name = "Unit (" + modelName + ") " + ((int)(Random.value * 100)).ToString();

        o.tag = "unit";

        Unit u = o.AddComponent<Unit>();

//        Mesh[] meshes = Resources.LoadAll<Mesh>("Models/" + modelName);
 //       Material[] materials = Resources.LoadAll<Material>("Models/" + modelName);

        Rigidbody rb = o.AddComponent<Rigidbody>();

        //        BoxCollider sc = o.AddComponent<BoxCollider>();
        //        sc.size = o.GetComponent<MeshRenderer>().bounds.size;
        MeshCollider c = o.AddComponent<MeshCollider>();
        c.convex = true;
        c.sharedMesh = o.GetComponent<MeshFilter>().mesh;

        rb.useGravity = false;
        rb.mass = 10;
        rb.drag = 10;
        rb.angularDrag = 10;


        u.faction = faction;
        u.template = template.Copy();
        //u.template = template;
        u.InitializeInternalData();

        return o;
    }
}
