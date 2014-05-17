using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(Building), true)]
public class BuildingInspector : Editor
{

    string[] optionList;
    int componentIndex;

    public BuildingInspector()
        : base()
    {
        optionList = Enum.GetNames(typeof(ControlType));
    }

    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Building targetBuilding = (Building)target;
        EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
        if (targetBuilding.ControlComponents.Count > 0)
        {
            EditorGUILayout.BeginVertical("Box");
            for (int i = 0; i < targetBuilding.ControlComponents.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.LabelField(targetBuilding.ControlComponents[i].ToString());
                if (GUILayout.Button("x"))
                {
                    targetBuilding.ControlComponents.Remove(targetBuilding.ControlComponents[i]);
                    EditorUtility.SetDirty(targetBuilding);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
            componentIndex = EditorGUILayout.Popup(componentIndex, optionList);
            if (GUILayout.Button("Add Control Component"))
            {
                targetBuilding.ControlComponents.Add((ControlType)(Enum.Parse(typeof(ControlType), optionList[componentIndex])));
                EditorUtility.SetDirty(targetBuilding);
            }
        

    }

}
