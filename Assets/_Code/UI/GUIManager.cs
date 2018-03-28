using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class GUIManager : MonoBehaviour {

    public List<GameObject> selectedObjects;
    public Rect selectionRect;
    public IOrderResponder orderResponder;

    private GUIStyle styleHealthGreen;
    private GUIStyle styleHealthRed;

    private Texture2D selectionTexture;
    private Texture buttonMoveTexture;
    private Texture buttonAttackTexture;
    private Texture buttonStopTexture;




    public void Start() {
    }

    void InitGUI() {
        styleHealthRed = new GUIStyle(GUI.skin.box);
        styleHealthRed.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 1f));
        styleHealthGreen = new GUIStyle(GUI.skin.box);
        styleHealthGreen.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 1f));


        selectionTexture = MakeTex(2, 2, new Color(0f, 1f, 0f, 0.2f));
        buttonMoveTexture = Resources.Load("Textures/move100") as Texture;
        buttonStopTexture = Resources.Load("Textures/stop100") as Texture;
        buttonAttackTexture = Resources.Load("Textures/attack100") as Texture;

        GUI.backgroundColor = Color.white;
    }


    void OnGUI() {
        if (styleHealthRed == null) {
            InitGUI();
        }
        foreach (GameObject go in selectedObjects) {
            if (go == null) continue;
            Unit u = go.GetComponent<Unit>();

            var barPos = Camera.main.WorldToScreenPoint(go.transform.position);
            barPos.y = Screen.height - barPos.y;
            float healthNormalized = u.health / u.currentState.template.parametersTemplate.maximumHealth;
            float healthLength = 60.0f * healthNormalized;

            GUIUtils.DrawScreenRect(new Rect(barPos.x - 30, barPos.y - 50, healthLength, 4.0f), new Color(1.0f - healthNormalized, healthNormalized, 0));
        }        if (selectionRect != Rect.zero) {
            Rect rect = new Rect(selectionRect.x, Screen.height - selectionRect.y, selectionRect.width, -selectionRect.height);
            GUI.DrawTexture(rect, selectionTexture);
            DrawScreenRectBorder(rect, 1.0f, Color.green);
        }
        if (selectedObjects.Count > 0) {
            Rect controlZone = new Rect(Screen.width - 300, Screen.height - 300, 290, 290);
            DrawScreenRectBorder(controlZone, 2.0f, Color.white);
            if (GUI.Button(new Rect(controlZone.x + 10, controlZone.y + 10, 50, 50), new GUIContent(buttonAttackTexture, "Attack"))) {
                orderResponder.DidSelectAttack();
            }

            if (GUI.Button(new Rect(controlZone.x + 70, controlZone.y + 10, 50, 50), new GUIContent(buttonMoveTexture, "Move"))) {
                orderResponder.DidSelectMove();
            }

            if (GUI.Button(new Rect(controlZone.x + 130, controlZone.y + 10, 50, 50), new GUIContent(buttonStopTexture, "Stop"))) {
                orderResponder.DidSelectStop();
            }
        }
    }

    private Texture2D MakeTex(int width, int height, Color col) {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i) {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color) {
        GUIUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        GUIUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        GUIUtils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        GUIUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}


static class GUIUtils {
    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture {
        get {
            if (_whiteTexture == null) {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color) {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }
}