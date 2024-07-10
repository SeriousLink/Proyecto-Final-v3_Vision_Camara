using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Premio : MonoBehaviour
{

    public Transform medidas;
    private void Start()
    {
        MoverPosicionInicial();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Presa"))
        {
            MoverPosicionInicial();
        }
    }
    private void MoverPosicionInicial()
    {
        bool posicionEncontrada = false;
        int intentos = 100;
        Vector3 posicionPotencial = Vector3.zero;

        while (!posicionEncontrada && intentos >= 0)
        {
            intentos--;
            posicionPotencial= new Vector3 ( transform.parent.position.x + UnityEngine.Random.Range((-medidas.localScale.x/2)+3, (medidas.localScale.x/2)-3),
            2, transform.parent.position.z + UnityEngine.Random.Range((-medidas.localScale.z/2)+3, medidas.localScale.z/2)-3);

            Collider[] colliders = Physics.OverlapSphere(posicionPotencial, 0.05f);
            if (colliders.Length == 0)
            {
                transform.position = posicionPotencial;
                posicionEncontrada = true;

            }
            
        }

    }
}