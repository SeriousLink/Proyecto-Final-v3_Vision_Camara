using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boca : MonoBehaviour
{
    public agentML_Cazador cazador;

    // Start is called before the first frame update
    void Start()
    {
        cazador = GetComponentInParent<agentML_Cazador>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Presa") && cazador.training)
        {
            cazador.presaDetectada(other.GetComponent<agentML_Presa>());
        }
    }
}
