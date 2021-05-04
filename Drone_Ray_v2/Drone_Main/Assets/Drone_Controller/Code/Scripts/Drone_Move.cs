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
        // 게임이 새로 시작될 때 입력되는 값
        gameObject.transform.position = droneInitPos;
        gameObject.transform.rotation = droneInitRot;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        // 가속도, 중력가속도 이런거 초기화 해주기
        rb1.velocity = Vector3.zero;
        rb2.velocity = Vector3.zero;
        rb3.velocity = Vector3.zero;
        rb4.velocity = Vector3.zero;
        rb1.angularVelocity = Vector3.zero;
        rb2.angularVelocity = Vector3.zero;
        rb3.angularVelocity = Vector3.zero;
        rb4.angularVelocity = Vector3.zero;

        // 극좌표값을 사용하기위해 설정
        var theta1 = Random.Range(-200f, 200f);
        var theta2 = Random.Range(-200f, 200f);
        var radius = Random.Range(25f, 25f);

        // 초기화되는 목표의 좌표값 새로 설정
        goal.transform.position = droneInitPos + new Vector3(radius * Mathf.Sin(theta1) * Mathf.Cos(theta2),
                                                            radius * Mathf.Sin(theta1) * Mathf.Sin(theta2),
                                                            radius * Mathf.Cos(theta1));

        // obstacle.transform.position = droneInitPos + (goal.transform.position - droneInitPos) * Random.Range(0.3f, 0.8f)
        //                               + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)); //Noise term
        // 이전과 지금의 거리값을 
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
        // 어떻게 행동할지 선택하자
        act0 = Mathf.Clamp(vectorAction[0], 0f, 1f);
        act1 = Mathf.Clamp(vectorAction[1], 0f, 1f);
        act2 = Mathf.Clamp(vectorAction[2], 0f, 1f);
        act3 = Mathf.Clamp(vectorAction[3], 0f, 1f);


        // Vector3 의 형태를 가진 함수를 만들어준다.
        // 그러고 x,y,z 값을 입력해줌
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

        // 각 엔진별로 힘을 주도록 설정
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
        //            Debug.Log("겟");


        //        }
        //        if (hits[i].collider.gameObject.tag == "wall")
        //        {
        //            SetReward(-5f);
        //            EndEpisode();
        //            Debug.Log("쿵");

        //        }
        //    }



        //    // 둘이 접촉하여 거리가 1.5f 이하되면 점수를 10점 얻고 success 함
        //if ((goal.transform.position - gameObject.transform.position).magnitude < 4f)
        //{
        //    SetReward(+10f);
        //    EndEpisode();
        //    Debug.Log("Success.");
        //}
        // 재시작을 위한 조건들을 달아준다
        // 1. x,y,z 값이 50이상 차이나게 되는 경우
        // 2. 드론의 y값이 0.2 이하로 떨어지는 경우
        // 3. 드론의 up의 y가 0.5 이하로 떨어지는 경우
        // 4. y가 0.2 이하일때는 그냥 끝나도록함
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
        //    // 이부분은 완벽히 이해는 안됐는데 일단 끝나는 타이밍에 목표와 드론의 거리를 계산해서
        //    // 상점을 계산해서줌
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
