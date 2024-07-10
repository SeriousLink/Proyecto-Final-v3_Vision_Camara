using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.SideChannels;
using Unity.VisualScripting;
using Unity.MLAgents.Actuators;
using System.Runtime.CompilerServices;
using System;

public class agentML_Cazador : Agent
{
    
    [SerializeField]
    MeshRenderer materialSuelo;

    [SerializeField]
    private Material materialAcierto;

    [SerializeField]
    private Material materialFracaso;

    public bool training = true;

    private Rigidbody rb;

    public float velocidad = 4;

    public agentML_Presa clasePresa;

    public float hambre;

    public BarraHambre barraHambre;

    IEnumerator hambreCo;

    IEnumerator atontadoCo;
    
    public Transform medidas;

    public bool atontadoBool = false;
    public override void Initialize()
    {
        if(!training) MaxStep = 0;
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator revisarHambre()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            hambre-=1;
            barraHambre.cambiarComida(hambre);
            if(hambre<70)
            {
                AddReward(-0.1f);
                if(hambre<50)
                {
                    AddReward(-0.5f);
                    if(hambre<10)
                    {
                        AddReward(-1f);
                        if(hambre<=0)
                        {
                            AddReward(-20f);                           
                            EndEpisode();
                            yield break;
                        }
                    }
                }
            }       
        }
    }

/*
    public void atontado()
    {
        atontadoCo = atontCo();
        StartCoroutine(atontadoCo);
    }

    public IEnumerator atontCo()
    {
        print("cazador empedado");
        AddReward(-1f);
        velocidad = 1;
        atontadoBool = true;
        yield return new WaitForSeconds(3f);
        print("cazador desempedado");
        atontadoBool = false;
        velocidad = 4;
        atontadoCo=null;
    }
*/
    public override void OnEpisodeBegin()
    {   
        rb.velocity = Vector3.zero;
        this.transform.rotation = new Quaternion(0,0,0,0);
        if(hambreCo==null) hambreCo = revisarHambre();
        else
        {
            StopCoroutine(hambreCo);
            hambreCo = null;
            hambreCo = revisarHambre();
        }

        /*
        if(atontadoCo!=null)
        {
            StopCoroutine(atontadoCo);
            atontadoCo=null;
        }
        atontadoBool = false;
        */
        StartCoroutine(hambreCo);
        barraHambre.Maximo();
       
        hambre=100;
        StartCoroutine(MoverPosicionInicial());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotar = actions.ContinuousActions[0];
        float moverAdelante = actions.ContinuousActions[1];
        AddReward(0.001f);
        if(actions.DiscreteActions[0]==0 && !atontadoBool)
        {
            print("sprint");
            hambre-=5;
            rb.AddForce(transform.forward * 5 ,ForceMode.Impulse);
        }

        hambre = hambre - Math.Abs(moverAdelante)*0.01f - Math.Abs(rotar)*0.01f;

        rb.MovePosition(transform.position + transform.forward * moverAdelante * velocidad * Time.deltaTime);
        transform.Rotate(0f, rotar * 3 , 0f, Space.Self);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //El vector3 ocupa 3 observaciones
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(hambre);
        //sensor.AddObservation(Vector3.Distance(transform.position, target.position));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        if(Input.GetKey(KeyCode.Space)) discreteActionsOut[0]=0;
        else discreteActionsOut[0]= 0;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("borde") && training)
        {
            materialSuelo.material = materialFracaso;
            AddReward(-5f);
        }

        else if(other.CompareTag("feromona") && training)
        {
            AddReward(3f);
            Destroy(other.gameObject);
        }
    }

        private void OnTriggerStay(Collider other)
    {

        if(other.CompareTag("borde") && training)
        {
            AddReward(-0.01f);
        }
    }

    public void presaDetectada(agentML_Presa other)
    {
            AddReward(15f);
            hambre += 50;
            if(hambre>130)hambre=130;
            barraHambre.cambiarComida(hambre);
            materialSuelo.material = materialAcierto;
            clasePresa = other.GetComponent<agentML_Presa>();
            clasePresa.AddReward(-15f);
            clasePresa.EndEpisode();
    }

    IEnumerator MoverPosicionInicial()
    {
        yield return new WaitForSeconds(0.1f);
        bool posicionEncontrada = false;
        int intentos = 100;
        Vector3 posicionPotencial = Vector3.zero;

        while (!posicionEncontrada && intentos >= 0)
        {
            int contadorCollider = 0;
            intentos--;
            posicionPotencial = new Vector3 ( transform.parent.position.x + UnityEngine.Random.Range((-medidas.localScale.x/2)+4, (medidas.localScale.x/2)-4),
            2, transform.parent.position.z + UnityEngine.Random.Range((-medidas.localScale.z/2)+4, medidas.localScale.z/2)-4);
        
            Collider[] colliders = Physics.OverlapSphere(posicionPotencial, 2f);
            foreach(Collider x in colliders)
            {
                if(x.tag == "Presa"|| x.tag == "target" || x.tag == "Cazador"  || x.tag == "decoracion")
                {
                    contadorCollider+=10;
                    break;
                }
                else
                {
                    contadorCollider++;
                }
            }

            if (contadorCollider <= 3)
            {
                transform.position = posicionPotencial;
                posicionEncontrada = true;
            } 
        }

    }
}
