using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public GameObject UI_Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
            }
            return root;
        }
    }

    int _sortOrder = 10;


    public T ShowSceneUI<T>(string name)
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = go.GetComponent<T>();
        go.transform.SetParent(UI_Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name)
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popupUI = go.GetComponent<T>();
        go.transform.SetParent(UI_Root.transform);

        return popupUI;
    }

}
