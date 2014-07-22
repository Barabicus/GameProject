using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(RTSCamera))]
public class RTSCameraInspector : Editor
{
    private bool _controlToggle = true;

    #region Inspector Variables

    private bool controlFoldout = false;
    private bool camConfigFoldout = false;
    private bool verticalFoldout = false;
    private bool horizontalFoldout = false;
    private bool rotateFoldout = false;
    private bool tiltFoldout = false;

    private RTSCamera RTSCameraInstance;
    #endregion

    void OnEnable()
    {
        RTSCameraInstance = target as RTSCamera;
    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.BeginVertical("box");


        if (GUILayout.Button("Controls", EditorStyles.toolbarDropDown)) controlFoldout = !controlFoldout;
        if (controlFoldout)
        {
            ControlEditor();
        }

        EditorGUILayout.Separator();

        if (GUILayout.Button("Camera Configuration", EditorStyles.toolbarDropDown)) camConfigFoldout = !camConfigFoldout;
        if (camConfigFoldout)
        {
            CameraConfigEditor();
        }

        EditorGUILayout.EndVertical();
    }

    private void CameraConfigEditor()
    {

        EditorGUILayout.BeginVertical("box");

        EditorGUI.indentLevel++;

        EditorGUILayout.Separator();


        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Speed");
        RTSCameraInstance.speed = EditorGUILayout.FloatField(RTSCameraInstance.speed);
        GUILayout.EndHorizontal();

        EditorGUILayout.Separator();


        EditorGUI.indentLevel--;


        EditorGUILayout.EndVertical();
    }

    private void ControlEditor()
    {
        //VERTICAL CONTROLS/////////////////////////////////////////////////////////////
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Vertical Controls", EditorStyles.foldout)) verticalFoldout = !verticalFoldout;
        if (verticalFoldout)
        {
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Vertical Setup");
            RTSCameraInstance.verticalSetup = (RTSCamera.ControlSetup)EditorGUILayout.EnumPopup(RTSCameraInstance.verticalSetup);
            GUILayout.EndHorizontal();


            if (RTSCameraInstance.verticalSetup == RTSCamera.ControlSetup.Axis)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Vertical Axis");
                RTSCameraInstance.verticalAxis = EditorGUILayout.TextField(RTSCameraInstance.verticalAxis);
                GUILayout.EndHorizontal();
            }
            else if (RTSCameraInstance.verticalSetup == RTSCamera.ControlSetup.KeyCode)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Positive Key (Forward)");
                RTSCameraInstance.forwardKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.forwardKey);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Negative Key (Backward)");
                RTSCameraInstance.backwardKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.backwardKey);
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();


        //VERTICAL CONTROLS/////////////////////////////////////////////////////////////
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Horizontal Controls", EditorStyles.foldout)) horizontalFoldout = !horizontalFoldout;
        if (horizontalFoldout)
        {
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Horizontal Setup");
            RTSCameraInstance.horizontalSetup = (RTSCamera.ControlSetup)EditorGUILayout.EnumPopup(RTSCameraInstance.horizontalSetup);
            GUILayout.EndHorizontal();


            if (RTSCameraInstance.horizontalSetup == RTSCamera.ControlSetup.Axis)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Vertical Axis");
                RTSCameraInstance.horizontalAxis = EditorGUILayout.TextField(RTSCameraInstance.horizontalAxis);
                GUILayout.EndHorizontal();
            }
            else if (RTSCameraInstance.horizontalSetup == RTSCamera.ControlSetup.KeyCode)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Negative Key (Left)");
                RTSCameraInstance.leftKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.leftKey);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Positive Key (Right)");
                RTSCameraInstance.rightKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.rightKey);
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        //Y AXIS CONTROLS/////////////////////////////////////////////////////////////
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Rotate", EditorStyles.foldout)) rotateFoldout = !rotateFoldout;
        if (rotateFoldout)
        {
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Rotate Setup");
            RTSCameraInstance.rotateYSetup = (RTSCamera.ControlSetup)EditorGUILayout.EnumPopup(RTSCameraInstance.rotateYSetup);
            GUILayout.EndHorizontal();


            if (RTSCameraInstance.rotateYSetup == RTSCamera.ControlSetup.Axis)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Rotate Axis");
                RTSCameraInstance.rotateYAxis = EditorGUILayout.TextField(RTSCameraInstance.rotateYAxis);
                GUILayout.EndHorizontal();
            }
            else if (RTSCameraInstance.rotateYSetup == RTSCamera.ControlSetup.KeyCode)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Negative Key (Left)");
                RTSCameraInstance.rotateLeftKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.rotateLeftKey);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Positive Key (Right)");
                RTSCameraInstance.rotateRightKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.rotateRightKey);
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        //X Axis CONTROLS/////////////////////////////////////////////////////////////
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Tilt", EditorStyles.foldout)) tiltFoldout = !tiltFoldout;
        if (tiltFoldout)
        {
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Tilt Setup");
            RTSCameraInstance.rotateXSetup = (RTSCamera.ControlSetup)EditorGUILayout.EnumPopup(RTSCameraInstance.rotateXSetup);
            GUILayout.EndHorizontal();


            if (RTSCameraInstance.rotateXSetup == RTSCamera.ControlSetup.Axis)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Tilt Axis");
                RTSCameraInstance.rotateXAxis = EditorGUILayout.TextField(RTSCameraInstance.rotateXAxis);
                GUILayout.EndHorizontal();
            }
            else if (RTSCameraInstance.rotateXSetup == RTSCamera.ControlSetup.KeyCode)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Positive Key (Up)");
                RTSCameraInstance.tiltIncKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.tiltIncKey);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Negative Key (Down)");
                RTSCameraInstance.tiltDecKey = (KeyCode)EditorGUILayout.EnumPopup(RTSCameraInstance.tiltDecKey);
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
    }
}

