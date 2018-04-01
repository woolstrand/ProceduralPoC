using System.Collections.Generic;
using UnityEngine;

using ProceduralToolkit;

public enum MeshClass {
    Projectile,
    HeavyProjectile,
    Unit,
    Building,
    Goliath
}

public class GameObjectsProvider {

    private static GameObjectsProvider instance;
    private static object thisLock = new object();


    private Dictionary<string, GameObject> templates;

    public static GameObjectsProvider Default() {
        lock (thisLock) { 
            if (instance == null) {
                instance = new GameObjectsProvider();
            }
        }
        return instance;
    }

    public static GameObject GameObjectForId(string id) {
        GameObject go = GameObject.Instantiate(Default().templates[id]);
        go.SetActive(true);
        return go;
    }

    public static GameObject CreateGameObjectAndTemplate(string id, MeshClass meshClass = MeshClass.Unit) {
        if (Default().templates.ContainsKey(id)) {
            return GameObjectForId(id);
        } else {
            Default().CreateGameObjectTemplate(id, meshClass);
            return GameObjectForId(id);
        }
    }

    public GameObjectsProvider() {
        templates = new Dictionary<string, GameObject>();
    }

    public GameObject CreateGameObjectTemplate(string id, MeshClass meshClass) {
        UnitMeshGenerator g = new UnitMeshGenerator();
        GameObject go;
        switch (meshClass) {
            case MeshClass.Projectile:
                go = g.CreateGameObject(0.2f, 2);
                break;
            case MeshClass.HeavyProjectile:
                go = g.CreateGameObject(0.5f, 5);
                break;
            case MeshClass.Unit:
                go = g.CreateGameObject(1.0f, 5);
                break;
            case MeshClass.Building:
                go = g.CreateGameObject(2.5f, 10);
                break;
            case MeshClass.Goliath:
                go = g.CreateGameObject(10.0f, 25);
                break;
            default:
                go = g.CreateGameObject();
                break;
        }
        go.SetActive(false);
        templates.Add(id, go);
        return go;
    }

    

}