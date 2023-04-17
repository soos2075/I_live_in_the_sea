using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CreateAssetMenu]
public class SO_Fish : ScriptableObject
{
    public List<FishData> _data = new List<FishData>();
    [System.Serializable]
    public class FishData
    {
        public Sprite fishSprite;
        public string name_kr;
        public string name_en;
        public string name_scientific;

        public string size_Real;
        public int size_Min;
        public int size_Max;

        //public float moveSpeed;

        public GameObject prefab;
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
        ViewAll();

        ButtonEvent();
        PageView();
    }

    void ViewAll()
    {
        EditorGUILayout.BeginHorizontal();

        float size_width = 50;
        float size_height = 50;

        float currentSpace = 100;

        for (int i = 0; i < value._data.Count; i++)
        {
            if (value._data[i].fishSprite != null)
            {
                if (GUILayout.Button(value._data[i].fishSprite.texture, GUILayout.Width(size_width), GUILayout.Height(size_height)))
                {
                    index = i;
                }
            }
            else
            {
                if (GUILayout.Button("None",GUILayout.Width(size_width), GUILayout.Height(size_height)))
                {
                    index = i;
                }
            }
            currentSpace += size_width;
            if (currentSpace > EditorGUIUtility.currentViewWidth)
            {
                currentSpace = 100;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField($"{i + 1}");
                EditorGUILayout.BeginHorizontal();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
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
        value._data[index].name_kr = (string)EditorGUILayout.TextField("한국이름", value._data[index].name_kr);
        value._data[index].name_en = (string)EditorGUILayout.TextField("영문이름", value._data[index].name_en);
        value._data[index].name_scientific = (string)EditorGUILayout.TextField("학명", value._data[index].name_scientific);
        //value.contents = (string)EditorGUILayout.TextArea(value.contents, GUILayout.MinHeight(50), GUILayout.MinWidth(100));

        value._data[index].size_Real = (string)EditorGUILayout.TextField("실제크기", value._data[index].size_Real);

        value._data[index].size_Min = (int)EditorGUILayout.IntField("최소크기", value._data[index].size_Min);
        value._data[index].size_Max = (int)EditorGUILayout.IntField("최대크기", value._data[index].size_Max);


        //value._data[index].moveSpeed = (float)EditorGUILayout.FloatField("속도", value._data[index].moveSpeed);

        value._data[index].prefab = (GameObject)EditorGUILayout.ObjectField("프리팹", value._data[index].prefab, typeof(GameObject), true);

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
