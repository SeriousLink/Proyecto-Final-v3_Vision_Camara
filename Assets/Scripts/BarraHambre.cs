using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.UI;

public class BarraHambre : MonoBehaviour
{

    public Slider barraHambre;
    public Slider rellenoHambre;
    public Slider vaciadoHambre;
    public float hambreMax;
    public float hambre;
    public float hambre2;

    private float lerp=0.025f;
    private float prueba= 100;
    void Start()
    {
        hambre=hambreMax;
        hambre=hambre2;
    }

    // Update is called once per frame
    void Update()
    {
        if(prueba<=1f)prueba=100;
        if( hambre!=hambre2 )
        {        
            hambre=Mathf.Lerp(hambre,hambre2,lerp);
            
            if(hambre>hambre2)
            {
                barraHambre.value = hambre2;
                rellenoHambre.value=hambre2;
                vaciadoHambre.value = Mathf.Lerp(vaciadoHambre.value,hambre2,lerp);
            }

            if(hambre<hambre2)
            {
                rellenoHambre.value = hambre2;                
                barraHambre.value = Mathf.Lerp(barraHambre.value,hambre2,lerp);
                vaciadoHambre.value = Mathf.Lerp(vaciadoHambre.value,hambre2,lerp);

            }
        }

        if ( Input.GetKeyDown(KeyCode.Space))
        {
            cambiarComida(-10);
        }

        else if(Input.GetKeyDown(KeyCode.D))
        {
            cambiarComida(25);
        }
    }

    public void cambiarComida(float hambreJugador)
    {
        hambre2=hambreJugador;
        if(hambre2>100) hambre2 = 100;
        else if(hambre2 < 0) hambre2 = 0;
    }
    
    public void Maximo()
    {
        hambre=100;
        hambre2=100;
        barraHambre.value=100;
        rellenoHambre.value=100;
        vaciadoHambre.value=100;
    }
}
