using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Middle))]
public class MiddleInspector : Editor {
  
    public override void OnInspectorGUI() {
        /*Middle myTarget = (Middle)target;

        myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
        EditorGUILayout.LabelField("Level", myTarget.Level.ToString());*/

        DrawDefaultInspector();
    } 

    public void OnEnable() {
        var t = (Middle)target;
        t.Src.select(t.gameObject);
        SceneView.onSceneGUIDelegate += SceneInput;
    }
    public void OnDisable() {
        SceneView.onSceneGUIDelegate -= SceneInput;
        PointListSource.CurrentEdit = null;
    }
    static bool getMousePos( Vector2 mp, ref Vector3 p ) {
        var cam = Camera.current;
        var mr = cam.ScreenPointToRay(new Vector3(mp.x, cam.pixelHeight - mp.y, 0));
        var pln = new Plane(Vector3.forward, 0);
        float d; if(pln.Raycast(mr, out d)) {
            p = mr.GetPoint(d);
            return true;
        } else return false;
    }

    bool Drag = false;
    void SceneInput(SceneView sceneview) {


        if(PointListSource.CurrentEdit != ((Middle)target).Src) {
            SceneView.onSceneGUIDelegate -= SceneInput;
        }
        var src = PointListSource.CurrentEdit;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        HandleUtility.Repaint();
       
        Event e = Event.current;
        Vector3 mp = Vector3.zero;
        switch(e.type) {
            case EventType.KeyUp:
                if(e.keyCode == KeyCode.Escape) {
                    SceneView.onSceneGUIDelegate -= SceneInput;
                    PointListSource.CurrentEdit = null;
                    return;
                }
                break;

            case EventType.MouseDown:
                if(e.button == 0 && getMousePos(e.mousePosition, ref mp) ) {
                    if(!src.pick(mp) && (e.modifiers & (EventModifiers.Control | EventModifiers.Shift)) != 0)
                        src.add(mp, e.modifiers);                                           
                }
                break;
            case EventType.MouseUp:
                Drag = false;
                if(e.button == 0)
                    PointListSource.CurrentNodeInd = uint.MaxValue;
                break;

            case EventType.MouseDrag:
                Drag = true;
                break;
        }
        if(Drag && PointListSource.CurrentNodeInd < (uint)src.Nodes.Count && getMousePos(e.mousePosition, ref mp)) {
            src.move(mp, (int)PointListSource.CurrentNodeInd, e.modifiers);
        }


       // Debug.Log("Event " + e);
    }
}

