using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchovy : Fish
{

    protected override void Initialize()
    {
        Initialize_Stat(1, 5, 2, 100, 10);
        Initialize_Ability(abilityType.Keep, 240, 4, 0.4f);
    }

    protected override void AbilityStart()
    {
        moveSpeed = 8f;
        abilityGage--;
    }

    protected override void AbilityOver()
    {
        moveSpeed = 5;
    }




}
