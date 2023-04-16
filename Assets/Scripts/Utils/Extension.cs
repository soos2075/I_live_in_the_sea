using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    //public static void AddUIEvent(this GameObject go, Action<PointerEventData> act, Define.MouseEvent event_type = Define.MouseEvent.Click)
    //{
    //    UI_Base.AddUIEvent(go, act, event_type);
    //}
    //public static void AddUIEvent_TowerSelect(this GameObject go, Action<PointerEventData, int> act, int num)
    //{
    //    UI_Base.AddUIEvent_TowerSelect(go, act, num);
    //}
    //public static void AddUIEvent_StageSelect(this GameObject go, Action<PointerEventData, int> act, int num)
    //{
    //    UI_Base.AddUIEvent_StageSelect(go, act, num);
    //}


    public static T GetComponentOrParent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetComponentOrParent<T>(go);
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

}
