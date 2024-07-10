using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.SideChannels;
using Unity.VisualScripting;
using Unity.MLAgents.Actuators;
using System.Runtime.CompilerServices;

public class agentML_Preseguidor : Agent
{

    [SerializeField]
    private Transform target;

    public bool training = true;


    public override void Initialize()
    {
        if(!training) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
      
        MoverPosicionInicial();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moverx = actions.ContinuousActions[0];
        float moverz = actions.ContinuousActions[1];
        
        transform.position += new Vector3((moverx * Time.deltaTime),0,(moverz * Time.deltaTime));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //El vector3 ocupa 3 observaciones
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
        sensor.AddObservation(Vector3.Distance(transform.position, target.position));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float moverx = Input.GetAxis("Horizontal");
        float moverz = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(moverx, 0f, moverz);

        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = movimiento.x;
        continuousActionsOut[1] = movimiento.z;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Presa"))
        {
            if(training)
            {
                AddReward(0.5f);
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("borde"))
        {
            if(training)
            {
                AddReward(-0.1f);
            }
        }
    }

    private void MoverPosicionInicial()
    {
        bool posicionEncontrada = false;
        int intentos = 100;
        Vector3 posicionPotencial = Vector3.zero;

        while (!posicionEncontrada || intentos >= 0)
        {
            print(intentos);
            intentos--;
            posicionPotencial= new Vector3 ( transform.parent.position.x + UnityEngine.Random.Range(-4f,4f),
            2, transform.parent.position.z + UnityEngine.Random.Range(-4f,4f));

        
            Collider[] colliders = Physics.OverlapSphere(posicionPotencial, 0.05f);
            print(colliders.Length);

            if (colliders.Length == 0)
            {
                print("primero"+posicionEncontrada);
                transform.position = posicionPotencial;
                posicionEncontrada = true;
                print("segundo" + posicionEncontrada);

            }
            
        }

    }
}
