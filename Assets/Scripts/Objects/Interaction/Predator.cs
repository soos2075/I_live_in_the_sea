using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    Fish fish;
    Transform head;
    public float eatingDistance;

    public List<string> preyList;
    public int preyLayer { get; set; }


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
        head = transform.GetChild(0).GetChild(1);
    }

    void Update()
    {
        EatingFood();
    }

    private void OnDrawGizmos()
    {
        if (head)
        {
            Gizmos.DrawWireSphere(head.position, eatingDistance);
        }
    }

    void EatingFood()
    {
        var hits = Physics.OverlapSphere(head.position, eatingDistance, preyLayer);
        foreach (var prey in hits)
        {
            prey.GetComponentInParent<Prey>().Attacked(1);
            break;
            //Debug.Log(prey);
        }
    }
}
