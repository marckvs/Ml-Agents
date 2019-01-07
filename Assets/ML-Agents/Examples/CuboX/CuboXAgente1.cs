using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboXAgente1 : Agent{


    public GameObject goal;
    public GameObject floor;

    public Color initialColor;
    public float speed = 10f;

    private float acumulativeReward = 0;
    private Material mat;
    private float rewardMultiplier = 2f;
    private bool isGoalNear = false;
    private float threshold = 10f; 

    void Start()
    {
        mat = floor.GetComponent<Renderer>().material;
        initialColor = mat.color;
        speed = 1f;
    }
    public override void AgentReset()
    {
        gameObject.transform.localPosition = Vector3.zero - Vector3.right * 7f;
        goal.transform.localPosition = ObtenerPosicionObjetivo(CuboXAcademia.separacionObjeto);
        mat.color = initialColor;
    }

    private Vector3 ObtenerPosicionObjetivo(float objectSeparation)
    {
        Vector3 position = Vector3.zero;
        position = new Vector3(objectSeparation, goal.transform.localPosition.y, transform.localPosition.z);
        /*if (isGoalNear == false && objectSeparation > threshold)
        {
            position = new Vector3(3f, goal.transform.localPosition.y, transform.localPosition.z);
            isGoalNear = true;
        }
        else
        {    
            position = new Vector3(Random.Range(objectSeparation - 0.45f, objectSeparation), goal.transform.localPosition.y, transform.localPosition.z);
            isGoalNear = false;   
        }*/
        return transform.localPosition + position;
    }

    public override void CollectObservations()
    {
        Vector3 relativePosition = goal.transform.localPosition - transform.localPosition;

        AddVectorObs(relativePosition.x / floor.transform.localScale.x);
        AddVectorObs(relativePosition.z / floor.transform.localScale.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("objetivo")){
            AddReward(10f);
            acumulativeReward += 10;
            mat.color = Color.green;
            Done();
        }
        if (other.CompareTag("obstaculo"))
        {
            AddReward(-30f);
            acumulativeReward -= 30;
            mat.color = Color.red;
            Done();
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if(brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            float newX = transform.localPosition.x + (vectorAction[0]*speed*Time.deltaTime);
            Mathf.Clamp(newX, -7f, 24f);

            float newZ = transform.localPosition.z + (vectorAction[1] * speed * Time.deltaTime);
            Mathf.Clamp(newZ, -3.5f, 3.5f);

            transform.localPosition = new Vector3(newX, 0f, newZ);
        }
    }

    void Update()
    {
        checkRayCast();
    }

    public void checkRayCast()
    {
        RaycastHit hitFront;
        RaycastHit hitLeft;
        RaycastHit hitRight;

        LayerMask layermaskObstacle = 1 << LayerMask.NameToLayer("obstacles");
        LayerMask layermaskWall = 1 << LayerMask.NameToLayer("wall");

        //Rayo Frontal

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 2.5f, Color.blue);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitFront, 2.5f, layermaskObstacle))
        {
            AddReward((-1 / hitFront.distance) * rewardMultiplier);
            acumulativeReward += ((-1 / hitFront.distance) * (rewardMultiplier));
        }

        //Rayo izquierdo

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1.5f, Color.red);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitLeft, 1.5f, layermaskWall))
        {
            AddReward((-1 / hitLeft.distance) * rewardMultiplier/10);
            acumulativeReward += ((-1 / hitLeft.distance) * rewardMultiplier/10);
            Debug.Log("hit");
        }

        //Rayo derecho

        Debug.DrawRay(transform.position, -1*transform.TransformDirection(Vector3.forward) * 1.5f, Color.green);

        if (Physics.Raycast(transform.position, -1*transform.TransformDirection(Vector3.forward), out hitRight, 1.5f, layermaskWall))
        {
            AddReward((-1 / hitRight.distance) * (rewardMultiplier/10));
            acumulativeReward += ((-1 / hitRight.distance) * rewardMultiplier/10);
        }

        Debug.Log(acumulativeReward);
    }

}
