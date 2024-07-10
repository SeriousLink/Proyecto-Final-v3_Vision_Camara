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

public class agentML_Presa : Agent
{

    [SerializeField]
    private Material materialAcierto;

    [SerializeField]
    private Material materialFracaso;

    [SerializeField]
    private MeshRenderer materialSuelo;

    public bool training = true;

    private Rigidbody rb;

    public bool sprint = false;

    public float velocidad = 8;

    public float hambre;

    public BarraHambre barraHambre;
    
    public IEnumerator coHa;
    public IEnumerator coFe;

    public Transform medidas;

    public ParticleSystem pedo;

    public GameObject esferaFeromonas;

    bool pedoBool = true;


    public override void Initialize()
    {
        if(!training) MaxStep = 0;
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator spawnFeromonas()
    {
        while(true)
        {
            Destroy(Instantiate(esferaFeromonas,transform.position,transform.rotation).gameObject,4);
            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator revisarHambre()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
  
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
                            AddReward(-10f);
                            EndEpisode();
                            yield break;
                        }
                    }
                }
            }        
        }
    }


    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        this.transform.rotation = new Quaternion(0,0,0,0);
        if(coHa==null) coHa = revisarHambre();
        else
        {
            StopCoroutine(coHa);
            coHa = null;
            coHa = revisarHambre();
        }

        if(coFe==null) coFe = spawnFeromonas();
        else
        {
            StopCoroutine(coFe);
            coFe = null;
            coFe = spawnFeromonas();
        }
        StartCoroutine(coFe);


        StartCoroutine(coHa);

        barraHambre.Maximo();
        
        hambre=100;

        StartCoroutine(MoverPosicionInicial());
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotar = actions.ContinuousActions[0];
        float moverAdelante = actions.ContinuousActions[1];
        AddReward(0.001f);


        if(actions.DiscreteActions[0]==0)
        {
            print("pedo");
            hambre-=10;
            rb.AddForce(transform.forward * -10 ,ForceMode.Impulse);
            pedoBool=false;
            Pedo bolaPedo = Instantiate(pedo,transform.position,transform.rotation).gameObject.GetComponentInChildren<Pedo>();
            bolaPedo.presa=this;
            
            if(hambre<=0) 
            {
                AddReward(-10f);
                EndEpisode();
            }
        }

        hambre = hambre - Math.Abs(moverAdelante)*0.01f - Math.Abs(rotar)*0.01f;
        
        rb.MovePosition(transform.position + transform.forward * moverAdelante * velocidad * Time.deltaTime);
        transform.Rotate(0f, rotar * velocidad * 2 , 0f, Space.Self);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //El vector3 ocupa 3 observaciones
        sensor.AddObservation(hambre);   
        sensor.AddObservation(transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        if(Input.GetKey(KeyCode.Space)) discreteActionsOut[0]=0;
        else discreteActionsOut[0] = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("target") && training)
        {
            AddReward(5f);
            hambre+=35;
            if(hambre>130) hambre=130;
            barraHambre.cambiarComida(hambre);
            materialSuelo.material = materialAcierto;
        }

        else if(other.CompareTag("borde") && training)
        {
            AddReward(-5f);
            materialSuelo.material = materialFracaso;
        }
    }


    IEnumerator MoverPosicionInicial()
    {
        bool posicionEncontrada = false;
        int intentos = 100;
        Vector3 posicionPotencial = Vector3.zero;
        yield return new WaitForSeconds(0.05f);
        while (!posicionEncontrada && intentos >= 0)
        {
            int contadorCollider = 0;

            intentos--;
            posicionPotencial = new Vector3 ( transform.parent.position.x + UnityEngine.Random.Range((-medidas.localScale.x/2)+4, (medidas.localScale.x/2)-4),
            2, transform.parent.position.z + UnityEngine.Random.Range((-medidas.localScale.z/2)+4, medidas.localScale.z/2)-4);

            Collider[] colliders = Physics.OverlapSphere(posicionPotencial, 5f);
            foreach(Collider x in colliders)
            {
                if(x.tag == "Cazador"|| x.tag == "target" || x.tag == "Presa" || x.tag == "decoracion")
                {
                    contadorCollider+=10;
                    break;
                }
                else
                {
                    contadorCollider++;
                }
            }

            if (contadorCollider <= 5)
            {
                transform.position = posicionPotencial;
                posicionEncontrada = true;

            } 
        }
    }

}
