using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboXAgente : Agent{


    public GameObject goal;
    public GameObject floor;

    public Color initialColor;
    public float speed;

    private float acumulativeReward = 0;
    private Rigidbody rb;
    private Material mat;
    private BoxCollider bc;
    private float rewardMultiplier = 2f;
    private bool isGoalNear = false;
    private float threshold = 10f;
    private int jumpForce = 15;
    public bool isGrounded = false;

    RaycastHit hitFront;
    RaycastHit hitLeft;
    RaycastHit hitRight;

    void Start()
    {
        mat = floor.GetComponent<Renderer>().material;
        rb = gameObject.GetComponent<Rigidbody>();
        bc = gameObject.GetComponent<BoxCollider>();
        initialColor = mat.color;
        speed = 3f;
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

        return transform.localPosition + position;
    }

    public override void CollectObservations()
    {
        Vector3 relativePosition = goal.transform.localPosition - transform.localPosition;

        AddVectorObs(relativePosition.x / floor.transform.localScale.x);
        AddVectorObs(relativePosition.z / floor.transform.localScale.z);

        AddVectorObs(isGrounded ? 1 : 0);
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
            AddReward(-300f);
            acumulativeReward -=300;
            mat.color = Color.red;
            Done();
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        
        checkRayCast();
        checkIsGrounded();

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            switch ((int)vectorAction[0])
            {
                case 0:
                    rb.MovePosition(rb.position + (transform.right * Time.fixedDeltaTime * speed));
                    break;
                case 1:
                    rb.MovePosition(rb.position + (-transform.right * Time.fixedDeltaTime * speed));
                    break;
                case 2:
                    rb.MovePosition(rb.position + (transform.forward * Time.fixedDeltaTime * speed));
                    break;
                case 3:
                    rb.MovePosition(rb.position + (-transform.forward * Time.fixedDeltaTime * speed));
                    break;
                case 4:
                    if (isGrounded)
                    {
                        rb.velocity = Vector3.zero;
                        rb.AddRelativeForce(transform.up * jumpForce, ForceMode.Impulse);
                    }
                    break;
            }
            /*float newX = vectorAction[0];

            float newZ = vectorAction[1];

            if (isGrounded && vectorAction[2] == 1)
            {
                rb.velocity = Vector3.zero;
                rb.AddForce(transform.up * jumpForce * vectorAction[2], ForceMode.Impulse);
            }
            else
            {
                rb.MovePosition(transform.localPosition + new Vector3(newX, 0, newZ) * Time.fixedDeltaTime * speed);
            }*/
        }
    }

    public void checkRayCast()
    {
        LayerMask layermaskObstacle = 1 << LayerMask.NameToLayer("obstacles");
        LayerMask lasyermasJumpReward = 1 << LayerMask.NameToLayer("jumpReward");
        LayerMask layermaskJumpObstacle = 1 << LayerMask.NameToLayer("jumpObstacle");
        LayerMask layermaskWall = 1 << LayerMask.NameToLayer("wall");

        //Rayo Frontal

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 2.5f, Color.blue);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitFront, 2.5f, layermaskObstacle))
        {
            AddReward((-1 / hitFront.distance) * rewardMultiplier/2);
            acumulativeReward += ((-1 / hitFront.distance) * (rewardMultiplier/2));
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitFront, 2.5f, layermaskJumpObstacle))
        {
            AddReward(-(1 / hitFront.distance) * rewardMultiplier/10);
            acumulativeReward += ((1 / hitFront.distance) * (rewardMultiplier/40));
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitFront, 1f, lasyermasJumpReward))
        {
            AddReward((1 / hitFront.distance) * rewardMultiplier*2);
            acumulativeReward += ((1 / hitFront.distance) * (rewardMultiplier*2));
        }

        //Rayo izquierdo

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1.5f, Color.red);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitLeft, 1.5f, layermaskWall))
        {
            AddReward((-1 / hitLeft.distance) * rewardMultiplier/10);
            acumulativeReward += ((-1 / hitLeft.distance) * rewardMultiplier/10);
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

    private void checkIsGrounded()
    {
        isGrounded = false;
        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.down * 0.5f, bc.size / 2f, transform.rotation);
        foreach (Collider c in colliders)
        {
            if (c != null && (c.CompareTag("ground")))
            {
                isGrounded = true;
                break;
            }
        }
    }

}
