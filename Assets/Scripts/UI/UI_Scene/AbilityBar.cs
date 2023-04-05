using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : UI_Base
{

    Image bar;
    Fish fish;
    public override void Init()
    {
        bar = GetComponent<Image>();
        fish = FindObjectOfType<PlayerController>().GetComponent<Fish>();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        bar.fillAmount = (float)fish.abilityGage / fish.abilityGageMax;
    }
}
