using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class SO_Fish : ScriptableObject
{
    public List<FishData> _data = new List<FishData>();
    [System.Serializable]
    public class FishData
    {
        public Sprite fishSprite;
        public string name;
        public float moveSpeed;
        public int size;
    }

}


[CustomEditor(typeof(SO_Fish))]
public class GuiTest : Editor
{
    SO_Fish value;
    int index;

    private void OnEnable()
    {
        value = (SO_Fish)target;
        index = 0;
        if (value._data.Count == 0)
        {
            AddList();
        }
    }

    public override void OnInspectorGUI()
    {
        ButtonEvent();
        PageView();
    }


    void ButtonEvent()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        //GUILayout.Space((EditorGUIUtility.currentViewWidth / 2) - 60);

        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
        {
            RemoveList();
        }
        if (GUILayout.Button("<<", GUILayout.Width(30), GUILayout.Height(30)))
        {
            PreviousPage();
        }
        if (GUILayout.Button(">>", GUILayout.Width(30), GUILayout.Height(30)))
        {
            NextPage();
        }
        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
        {
            AddList();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }


    void PageView()
    {

        EditorGUILayout.LabelField($"Page : {index}");

        EditorGUILayout.BeginVertical();

        value._data[index].fishSprite = (Sprite)EditorGUILayout.ObjectField("이미지", value._data[index].fishSprite, typeof(Sprite), true
            , GUILayout.MinWidth(250), GUILayout.MinHeight(250));
        value._data[index].name = (string)EditorGUILayout.TextField("이름", value._data[index].name);
        //value.contents = (string)EditorGUILayout.TextArea(value.contents, GUILayout.MinHeight(50), GUILayout.MinWidth(100));

        value._data[index].moveSpeed = (float)EditorGUILayout.FloatField("속도", value._data[index].moveSpeed);
        value._data[index].size = (int)EditorGUILayout.IntField("크기", value._data[index].size);

        EditorGUILayout.EndVertical();

        if (GUI.changed) EditorUtility.SetDirty(target);
    }

    void NextPage()
    {
        if (index + 1 < value._data.Count)
        {
            index++;
        }
    }
    void PreviousPage()
    {
        if (index > 0)
        {
            index--;
        }
    }
    void AddList()
    {
        value._data.Add(new SO_Fish.FishData());
    }
    void RemoveList()
    {
        if (index < value._data.Count)
        {
            value._data.RemoveAt(index);
            index--;
        }
    }
}
