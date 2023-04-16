using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    Fish fish;
    public List<string> preyList;
    public int preyLayer { get; set; }

    public bool showInteractRadius;

    private void Awake()
    {
        foreach (var str in preyList)
        {
            preyLayer |= LayerMask.GetMask(str);
        }
    }

    void Start()
    {
        fish = GetComponent<Fish>();
    }

    void Update()
    {
        EatingFood();
        //Debug.DrawRay(head.position, transform.right * eatingDistance);
    }

    private void OnDrawGizmos()
    {
        if (showInteractRadius)
        {
            Gizmos.DrawWireSphere(fish.Pos_Head.position, fish.InteractRadius);
        }
    }

    void EatingFood()
    {
        var hits = Physics.OverlapSphere(fish.Pos_Head.position, fish.InteractRadius, preyLayer);
        foreach (var prey in hits)
        {
            prey.GetComponentInParent<Prey>().Attacked(1);
            break;
            //Debug.Log(prey);
        }
    }
}
