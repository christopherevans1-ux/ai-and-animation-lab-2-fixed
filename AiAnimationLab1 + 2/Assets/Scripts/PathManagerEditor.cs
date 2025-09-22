using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathManagerEditor : Editor
{
    [SerializeField]
    PathManager pathManager;

    [SerializeField]
    List<WayPoint> thePath;
    List<int> toDelete;

    WayPoint selectedPoint = null;
    bool doRepaint = true;

    private void OnSceneGUI()
    {
        thePath = pathManager.GetPath();
        DrawPath(thePath);
    }

    private void OnEnable()
    {
        pathManager = target as PathManager;
        toDelete = new List<int>();
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        thePath = pathManager.GetPath();

        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Path");

        DrawGUIforPoints();

        //button for adding a point to the path
        if(GUILayout.Button("Add point to path"))
        {
            pathManager.CreateAddPoint();
        }

        EditorGUILayout.EndVertical();
        SceneView.RepaintAll();
    }

    void DrawGUIforPoints()
    {
        if(thePath != null && thePath.Count > 0)
        {
            for(int i=0; i < thePath.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                WayPoint p = thePath[i];

                Color c = GUI.color;
                if (selectedPoint == p) GUI.color = Color.green;

                Vector3 oldPos = p.GetPos();
                Vector3 newPos = EditorGUILayout.Vector3Field("", oldPos);

                if (EditorGUI.EndChangeCheck()) p.SetPos(newPos);

                //delete button
                if(GUILayout.Button("-", GUILayout.Width(25)))
                {
                    toDelete.Add(i); //do something for deletion
                }

                GUI.color = c;
                EditorGUILayout.EndHorizontal();
            }
        }
        if(toDelete.Count > 0)
        {
            foreach (int i in toDelete)
                thePath.RemoveAt(i); //remove from the path
            toDelete.Clear(); // clear the delete list for next time
        }
    }

    public void DrawPath(List<WayPoint> path)
    {
        if(path != null)
        {
            int current = 0;
            foreach(WayPoint wp in path)
            {
                //draw current point
                doRepaint = DrawPoint(wp);
                int next = (current + 1) % path.Count;
                WayPoint wpnext = path[next];

                DrawPathLine(wp, wpnext);

                //advance counter
                current += 1;
            }
        }
        if (doRepaint) Repaint();
    }

    public void DrawPathLine(WayPoint p1, WayPoint p2)
    {
        //draw a line between current and next point
        Color c = Handles.color;
        Handles.color = Color.grey;
        Handles.DrawLine(p1.GetPos(), p2.GetPos());
        Handles.color = c;
    }

    public bool DrawPoint(WayPoint p)
    {
        bool isCharged = false;
        if(selectedPoint == p)
        {
            Color c = Handles.color;
            Handles.color = Color.green;

            EditorGUI.BeginChangeCheck();
            Vector3 oldpos = p.GetPos();
            Vector3 newpos = Handles.PositionHandle(oldpos, Quaternion.identity);

            float handleSize = HandleUtility.GetHandleSize(newpos);

            Handles.SphereHandleCap(-1, newpos, Quaternion.identity, 0.25f * handleSize, EventType.Repaint);
            if (EditorGUI.EndChangeCheck())
            {
                p.SetPos(newpos);
            }

            Handles.color = c;
        }
        else
        {
            Vector3 currPos = p.GetPos();
            float handleSize = HandleUtility.GetHandleSize(currPos);
            if (Handles.Button(currPos, Quaternion.identity, 0.25f * handleSize, 0.25f * handleSize, Handles.SphereHandleCap))
            {
                isCharged = true;
                selectedPoint = p;
            }
        }

        return isCharged;
    }
}
