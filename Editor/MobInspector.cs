using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(Mob))]
public class MobInspector : Editor
{

    string[] optionList;
    int componentIndex;
    int lastIndex = -1;

    public MobInspector()
        : base()
    {
        optionList = Enum.GetNames(typeof(Gender));
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Mob mob = (Mob)target;
        EditorGUILayout.LabelField("Unit Name", mob.UnitName);
        componentIndex = EditorGUILayout.Popup("Gender", componentIndex, optionList);
        if (componentIndex != lastIndex)
        {
            lastIndex = componentIndex;
            mob.Gender = (Gender)Enum.Parse(typeof(Gender), optionList[componentIndex]);
            EditorUtility.SetDirty(mob);
        }
    }
}
