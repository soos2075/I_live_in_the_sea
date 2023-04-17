using System;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    public int currentHP;
    public int maxHp;

    public Action eatenEvent;

    public List<string> predatorList;
    public int predatorLayer { get; set; }

    private void Awake()
    {
        foreach (var str in predatorList)
        {
            predatorLayer |= LayerMask.GetMask(str);
        }
    }
    void Start()
    {
        if (isFade)
        {
            mat = gameObject.GetComponentOrChild<MeshRenderer>().material;
        }
        currentHP = maxHp;
    }

    void Update()
    {
        FadeOut();
    }


    public void Attacked(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            if (eatenEvent != null)
            {
                eatenEvent.Invoke();
            }
            eatenEvent = null;
            gameObject.SetActive(false);
        }
    }

    public void Regrown()
    {
        gameObject.SetActive(true);
        currentHP = maxHp;
    }


    public bool isFade;
    public Material mat;
    void FadeOut()
    {
        if (isFade)
        {
            float hp = (float)currentHP / maxHp;
            mat.SetFloat("_hp", Mathf.Clamp(hp, 0.25f, 0.75f));
        }
    }

}
