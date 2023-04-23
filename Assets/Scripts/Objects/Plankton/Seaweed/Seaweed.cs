using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seaweed : Plankton
{
    //? Moss,Caral,Kelp를 각각 스크립트로 나눌지 하나로 할지는 추후 결정


    public int maximumPreyCount;
    public int currentPreyCount;

    public Prey[] preyArray;
    public GameObject preyObj;

    void Start()
    {
        maximumPreyCount = transform.childCount;
        preyArray = new Prey[maximumPreyCount];

        for (int i = 0; i < maximumPreyCount; i++)
        {
            CreatePrey(i);
        }
    }


    IEnumerator CreatePreyCoroutine(int num, float timer)
    {
        yield return new WaitForSeconds(timer);
        CreatePrey(num);
    }

    void CreatePrey(int num)
    {
        if (!preyArray[num])
        {
            preyArray[num] = Instantiate(preyObj, transform.GetChild(num)).GetComponent<Prey>();
            preyArray[num].transform.rotation = Quaternion.LookRotation(Vector3.forward);
            preyArray[num].eatenEvent = () => EatenEvent(num);
            currentPreyCount++;
            Debug.Log($"{num} 번 먹이 신규생성");
        }
        else if (preyArray[num] && !preyArray[num].gameObject.activeInHierarchy)
        {
            preyArray[num].Regrown();
            preyArray[num].eatenEvent = () => EatenEvent(num);
            currentPreyCount++;
            Debug.Log($"{num} 번 먹이리필");
        }
    }

    void EatenEvent(int num)
    {
        currentPreyCount--;
        StartCoroutine(CreatePreyCoroutine(num, Random.Range(5,10)));
    }
}
