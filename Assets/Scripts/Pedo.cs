using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Pedo : MonoBehaviour
{

    public agentML_Presa presa;
    public bool detectado;
    public GameObject parent;
    public void Start()
    {
        StartCoroutine(pedoTimer());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Cazador")
        {
           //other.GetComponent<agentML_Cazador>().atontado();
           presa.AddReward(5f);
           detectado = true;
        }
    }

    IEnumerator pedoTimer()
    {
        yield return new WaitForSeconds(3);
        Destroy(parent);
    }
}