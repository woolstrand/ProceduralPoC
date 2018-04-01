using System.Collections.Generic;

using ProceduralToolkit;
using UnityEngine;

class UnitMeshGenerator {

    List<Mesh> meshes;
    List<Material> materials;

    Mesh currentMesh;

    public GameObject CreateGameObject(float size = 1.0f, int complexity = 5) {
        GameObject go = new GameObject();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        MeshFilter mf = go.AddComponent<MeshFilter>();

        MeshDraft md = NewBlock(size, complexity);
        materials = new List<Material> { NewMaterial() };

        FlattenBottom(md);

        mf.mesh = md.ToMesh();
        mf.mesh.RecalculateNormals();
        mr.materials = materials.ToArray();

        return go;
    }

    private void FlattenBottom(MeshDraft md) {
        for (int i = 0; i < md.vertexCount; i++) {
            if (md.vertices[i].y < 0.01f) {
                md.vertices[i] = md.vertices[i].y * new Vector3(1, 0, 1) + new Vector3(0, 0.01f, 0);
            }
        }
    }

    private MeshDraft NewBlock(float size, int minComponents) {

        MeshDraft md = new MeshDraft();

        int componentIndex = 0;
        while (componentIndex < minComponents || RandomE.Chance(0.3f)) {
            MeshDraft part; float partSize = size / 2;
            if (componentIndex == 0) {
                partSize = size / 6;
            }
            switch (Random.Range(0, 5)) {
                case 0:
                    part = MeshDraft.Cube(Random.Range(0.2f * partSize, partSize));
                    part.Paint(RandomE.color);
                    break;
                case 1:
                    part = MeshDraft.Cylinder(Random.Range(0.2f * partSize, partSize), Random.Range(3, 24), Random.Range(0.0f, partSize));
                    part.Paint(RandomE.color);
                    break;
                case 2:
                    part = MeshDraft.Sphere(Random.Range(0.2f * partSize, partSize), Random.Range(3, 24), Random.Range(3, 24));
                    part.Paint(RandomE.color);
                    break;
                case 3:
                    part = MeshDraft.Pyramid(Random.Range(0.2f * partSize, partSize), Random.Range(3, 24), Random.Range(0.0f, partSize));
                    part.Paint(RandomE.color);
                    break;
                case 4:
                    part = NewBlock(size / 2, 1);
                    break;
                default:
                    part = MeshDraft.Cube(Random.Range(0.2f * partSize, partSize));
                    part.Paint(RandomE.color);
                    break;
            }

            Vector3 charSize = new Vector3(size / 4, size / 4, size / 4);

            part = part.Scale(RandomE.Range(Vector3.zero, new Vector3(2, 2, 2)));
            part = part.Rotate(Quaternion.AngleAxis(Random.Range(0, Mathf.PI * 2), RandomE.insideUnitCube));

            if (componentIndex > 0 || RandomE.Chance(0.1f)) {
                part = part.Move(RandomE.Range(new Vector3(-size / 4, 0, -size / 4), new Vector3(size / 4, size / 4, size / 4)));
            }

            if (RandomE.Chance(0.2f)) {
                switch (Random.Range(0, 1)) {
                    case 0: //round clone
                        int copies = Random.Range(2, 10);
                        MeshDraft result = new MeshDraft();
                        Quaternion rotation = Quaternion.AngleAxis(Random.Range(Mathf.PI / (2 * copies), Mathf.PI * 2 / copies), RandomE.insideUnitCube);
                        for (int i = 0; i < copies; i ++) {
                            result.Add(part);
                            result.Rotate(rotation);
                        }
                        part = result;
                        break;
                    case 1:
                        part = part.Add(part.Scale(new Vector3(-1.0f, 1.0f, 1.0f)));
                        break;
                    default:
                        break;
                }
            }

            md.Add(part);
            componentIndex++;
        }

        return md;
    }

    private Material NewMaterial() {
        Shader s = Shader.Find("Procedural Toolkit/Vertex Color/Diffuse");
        Material m = new Material(s);
        m.color = RandomE.color;
        return m;
    }

}

