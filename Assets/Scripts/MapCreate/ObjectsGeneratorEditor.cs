using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ObjectsGenerator))]
[CanEditMultipleObjects]
public class ObjectsGeneratorEditor : Editor
{
    ObjectsGenerator value;

    //SerializedProperty ObjectsGenerator;
    private void OnEnable()
    {
        //var ObjectsGenerator = serializedObject.FindProperty("ObjectsGenerator");
        value = (ObjectsGenerator)target;

    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ButtonEvent();
    }

    void ButtonEvent()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        //GUILayout.Space((EditorGUIUtility.currentViewWidth / 2) - 60);

        if (GUILayout.Button("오브젝트 생성", GUILayout.Width(100), GUILayout.Height(50)))
        {
            value.CreatePosRaycast();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}
