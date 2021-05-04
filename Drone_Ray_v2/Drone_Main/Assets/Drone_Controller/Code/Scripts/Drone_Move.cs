using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class Drone_Move : Agent
{

    public GameObject goal;
    public float StartingHeightMin = 0f;
    public float StartingHeightMax = 1f;
    public float UpForce = 10f;


    Vector3 goalInitPos;
    Quaternion InitRot;
    Rigidbody rBody;


    Vector3 droneInitPos;
    Quaternion droneInitRot;

    public GameObject RF_Engine;
    public GameObject LB_Engine;
    public GameObject RB_Engine;
    public GameObject LF_Engine;

    Rigidbody rb1;
    Rigidbody rb2;
    Rigidbody rb3;
    Rigidbody rb4;

    float preDist;
    float curDist;

    float act0;
    float act1;
    float act2;
    float act3;

    public override void Initialize()
    {
        goalInitPos = goal.transform.position;

        droneInitPos = gameObject.transform.position;
        droneInitRot = gameObject.transform.rotation;
        rBody = gameObject.GetComponent<Rigidbody>();

        rb1 = RF_Engine.GetComponent<Rigidbody>();
        rb2 = LB_Engine.GetComponent<Rigidbody>();
        rb3 = RB_Engine.GetComponent<Rigidbody>();
        rb4 = LF_Engine.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // ������ ���� ���۵� �� �ԷµǴ� ��
        gameObject.transform.position = droneInitPos;
        gameObject.transform.rotation = droneInitRot;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        // ���ӵ�, �߷°��ӵ� �̷��� �ʱ�ȭ ���ֱ�
        rb1.velocity = Vector3.zero;
        rb2.velocity = Vector3.zero;
        rb3.velocity = Vector3.zero;
        rb4.velocity = Vector3.zero;
        rb1.angularVelocity = Vector3.zero;
        rb2.angularVelocity = Vector3.zero;
        rb3.angularVelocity = Vector3.zero;
        rb4.angularVelocity = Vector3.zero;

        // ����ǥ���� ����ϱ����� ����
        var theta1 = Random.Range(-200f, 200f);
        var theta2 = Random.Range(-200f, 200f);
        var radius = Random.Range(25f, 25f);

        // �ʱ�ȭ�Ǵ� ��ǥ�� ��ǥ�� ���� ����
        goal.transform.position = droneInitPos + new Vector3(radius * Mathf.Sin(theta1) * Mathf.Cos(theta2),
                                                            radius * Mathf.Sin(theta1) * Mathf.Sin(theta2),
                                                            radius * Mathf.Cos(theta1));

        // obstacle.transform.position = droneInitPos + (goal.transform.position - droneInitPos) * Random.Range(0.3f, 0.8f)
        //                               + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)); //Noise term
        // ������ ������ �Ÿ����� 
        preDist = (goal.transform.position - gameObject.transform.position).magnitude;

        // Debug.Log("OnEpisodeBegin");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(goal.transform.position - gameObject.transform.position);
        //sensor.AddObservation(obstacle.transform.position - gameObject.transform.position);
        //sensor.AddObservation(gameObject.transform.up);
        // sensor.AddObservation(gameObject.transform.forward);
        // sensor.AddObservation(Vector3.Distance(goalInitPos, rBody.transform.position));
        if (goal != null)
        {
            sensor.AddObservation(goal.transform.position - gameObject.transform.position);
            sensor.AddObservation(gameObject.transform.rotation);
            sensor.AddObservation(rBody.velocity);
            sensor.AddObservation(rBody.angularVelocity);
            // Debug.Log("CollectObservations");
        }

    }



    public override void OnActionReceived(float[] vectorAction)
    {
        // Debug.Log("OnActionReceived");
        // ��� �ൿ���� ��������
        act0 = Mathf.Clamp(vectorAction[0], 0f, 1f);
        act1 = Mathf.Clamp(vectorAction[1], 0f, 1f);
        act2 = Mathf.Clamp(vectorAction[2], 0f, 1f);
        act3 = Mathf.Clamp(vectorAction[3], 0f, 1f);


        // Vector3 �� ���¸� ���� �Լ��� ������ش�.
        // �׷��� x,y,z ���� �Է�����
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];
        controlSignal.z = vectorAction[2];

        //controlSignal.x = Mathf.Clamp(controlSignal.x, 0f, 1f);
        //controlSignal.y = Mathf.Clamp(controlSignal.y, 0f, 1f);
        //controlSignal.z = Mathf.Clamp(controlSignal.z, 0f, 1f);


        //rb1.AddForce(RF_Engine.transform.up * (act0 * 10));
        //rb2.AddForce(LB_Engine.transform.up * (act1 * 10));
        //rb3.AddForce(RB_Engine.transform.up * (act2 * 10));
        //rb4.AddForce(LF_Engine.transform.up * (act3 * 10));

        // �� �������� ���� �ֵ��� ����
        rb1.AddForce(controlSignal * UpForce);
        rb2.AddForce(controlSignal * UpForce);
        rb3.AddForce(controlSignal * UpForce);
        rb4.AddForce(controlSignal * UpForce);

        AddReward(+0.001f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Cube crashed into the block wall
        if (collision.gameObject.CompareTag("wall"))
        {
            SetReward(-1f);
            EndEpisode();
            Debug.Log(-1);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("target"))
        {
            SetReward(+10f);
            Debug.Log(+10);
        }
    }


    private void Update()
    {
        //    RaycastHit[] hits = Physics.RaycastAll(new Ray(droneInitPos, (gameObject.transform.position - droneInitPos)));
        //    for (int i = 0; i < hits.Length; ++i)
        //    {
        //        if (hits[i].collider.gameObject.tag == "target")
        //        {
        //            SetReward(+10f);
        //            EndEpisode();
        //            Debug.Log("��");


        //        }
        //        if (hits[i].collider.gameObject.tag == "wall")
        //        {
        //            SetReward(-5f);
        //            EndEpisode();
        //            Debug.Log("��");

        //        }
        //    }



        //    // ���� �����Ͽ� �Ÿ��� 1.5f ���ϵǸ� ������ 10�� ��� success ��
        //if ((goal.transform.position - gameObject.transform.position).magnitude < 4f)
        //{
        //    SetReward(+10f);
        //    EndEpisode();
        //    Debug.Log("Success.");
        //}
        // ������� ���� ���ǵ��� �޾��ش�
        // 1. x,y,z ���� 50�̻� ���̳��� �Ǵ� ���
        // 2. ����� y���� 0.2 ���Ϸ� �������� ���
        // 3. ����� up�� y�� 0.5 ���Ϸ� �������� ���
        // 4. y�� 0.2 �����϶��� �׳� ����������
        if (Mathf.Abs(gameObject.transform.position.x - droneInitPos.x) > 50f ||
            Mathf.Abs(gameObject.transform.position.z - droneInitPos.z) > 50f ||
            gameObject.transform.position.y - droneInitPos.y > 50f ||
            gameObject.transform.position.y < 0.2f ||
            gameObject.transform.up.y < 0.5f
            )
        //(obstacle.transform.position - gameObject.transform.position).magnitude < 0.3f)
        {
            SetReward(-5f);
            EndEpisode();
            Debug.Log("Failed.");
        }
        else if (goal.transform.position.y < 0.2f)
        {
            EndEpisode();
            Debug.Log("reset");
        }
        //    // �̺κ��� �Ϻ��� ���ش� �ȵƴµ� �ϴ� ������ Ÿ�ֿ̹� ��ǥ�� ����� �Ÿ��� ����ؼ�
        //    // ������ ����ؼ���
        //    else
        //    {
        //        var reward = (preDist - curDist);
        //        var distance = Vector3.Distance(goal.transform.position, gameObject.transform.position);
        //        curDist = distance;
        //        SetReward(reward);
        //        preDist = curDist;
        //    }
    }
}
