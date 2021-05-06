## ML-Agent 구성

ml-agent Git 홈페이지 : https://github.com/Unity-Technologies/ml-agents

홈페이지에서 파일 clone

### Window
1. 가상환경 생성
https://github.com/Unity-Technologies/ml-agents/blob/release_15_docs/docs/Using-Virtual-Environment.md  참고
    - 폴더 생성후 해당 폴더로 이동 
    ex) cd C:\Users\pnu\Desktop\ML-Kit\ML

    1-1. Pip 설치
        - curl https://bootstrap.pypa.io/get-pip.py -o get-pip.py
        - python3 get-pip.py
        - pip3 -V
        - pip3 install mlagents

    1-2. 윈도우 설정
        - 가상환경 폴더 생성 : md python-envs
        -  새 환경 생성 : python -m venv python-envs\sample-env
        - 환경 활성화 : python-envs\sample-env\Scripts\activate
        - pip 버전 업그레이드 : pip install --upgrade pip
        - 환경 비활성화 : deactivate

   

2. agent 설치 확인
    - mlagents-learn Unity 로고가 나오면 설치 완료

    안나올경우 파이토치 설치
        - pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html


### using Anaconda
1. 아나콘다 실행
    - 가상환경 생성
        - CMD, PowerShell, Jupyter 등 설치

2. 가상환경실행 후 ml-agent가 있는 폴더로 이동
    - curl https://bootstrap.pypa.io/get-pip.py -o get-pip.py
	- python3 get-pip.py
	- pip3 -V
	- pip3 install mlagents
	- pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html


## 모델 학습
3.1 알고리즘 확인
mlagent/config/ppo로 이동 .yaml 파일에서 파라미터 값을 변경하고 학습

    - 해당 폴더로 이동 cd C:\Users\pnu\Desktop\ML-Kit\ML\ml-agents
	ex) 
	- mlagents-learn config/ppo/Lee.yaml --run-id=first3DBallRun  cmd에 입력 후 플레이 버튼을 누르면 강화학습을 시작 유니티와 텐서플로를 연결해주는 통신 슬롯 (run id는 내가 원하는 걸로 변경가능)

3.2 모델 확인
ml-agents / results에 들어가면 내가 지정한 id이름으로 폴더가 생성되어있음
폴더 안에 .nn으로 만들어진 모델 파일이 생성됨

3.3 모델 적용
- 해당 프로젝트 경로에 모델 붙여넣기
- Agent를 클릭 후 Behavior Parameters Model에 만들 모델을 적용

3.4 Tensor Board로 확인
- tensorboard --logdir results
	해당 로컬호스트에서 확인 가능 (정확도와 손실함수를 확인가능)

## ML-Agent 적용
1. Unity 환경 구성
- Package Manager에서 ML Agents 다운

2. 물리엔진 적용 Rigidbody
	- Agent만 적용	

3. 충돌 인식 collider 
	- Target과 Agent 둘다 적용

4. Agent에 Decision Requester를 컴포넌트
- Decision Requester (결정을 요청) : 10 (숫자가 낮을수록 자주 결정을 내려야하므로 더 많은 학습시간이 필요)
- Behavior Parameters 
	- Space size : 레이어의 개수 : 8
	- Type : Continuous 
	- Size : 액션 사이즈의 수? : 2
	- Model : 학습한 모델 적용

5. Agent (Script)
Agent를 더블 클릭 후 기본인것을 확인 
(바로 제거 안됨) : Script에 MLAgent와 연동되어있는게 없기 때문에

새로운 스크립트를 만들고 해당 코드를 작성
using Unity.MLAgents; // 유니티와 텐서를 연동해줌
using Unity.MLAgents.Sensors; // 오브젝에 센서를 가해서 위치를 찾는 기능

새로운 스크립트를 Agent에 상속
	public class RollerAgent : Agent

6. 훈련하는 과정
	- 1. cmd 창에서 가상환경 실행
		- 해당 폴더로 이동 cd C:\Users\pnu\Desktop\ML-Kit\ML
		- 가상환경 실행 : python-envs\ML-env\Scripts\activate
	- 2. cd C:\Users\pnu\Desktop\ML-Kit\ML\ml-agents (ml-agents가 있는 파일)
	- 3. config/ppo에 있는 .yaml 파일을 복사하여 나의 학습에 맞게 설정
	- 4. 강화학습  mlagents-learn config/ppo/rollerball_config.yaml --run-id=RollerBall # id는 사용자 정의 가능
		- mlagents-learn --force : 재실행


* 현재 보고 있는 위치로 카메라를 고정
- Main Camera로 이동
	- GameObject => Alien with View

## Agent 함수 
    - Initialize() : 초기화 작업을 위해 한번 호출되는 메소드
        Rigidbody 컴포넌트의 속도(Velocity)
        타겟의 위치  private Transform tr;
        자신의 위치  private Rigidbody rb;

    - OnEpisodeBegin() : 에피소드(학습단위)가 시작할때마다 호출
        OnEpisodeBegin 메소드는 학습이 시작할 때 마다 한번씩 호출되는 메소드로 에이전트의 초기화 및 환경을 재설정한다.
        - 물리력 초기화
                rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;		

    - CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor) :
        환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
        관측 데이터의 size(갯수)인 8을 Behaviour Parameters 컴포넌트의 Vector Observation / Space Size 속성으로 설정한다.


    - OnActionReceived(float[] vectorAction) : 브레인(정책)으로 부터 전달 받은 행동을 실행하는 메소드

    - Heuristic : 개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)


    ## Decision Requester 컴포넌트
    Decision Requester는 에이전트가 어떻게 행동해야 할지 정책에 결정을 요청하는 컴포넌트다. 
    정책은 에이전트가 주변 환경정보를 수집하고 관찰한 정보를 토대로 학습된것을 의미한다.

    ## OnCollisionEnter(Collision coll)
    에이전트가 타겟에 도달했을 때 + 보상을 주고 Dead Zone 영역에 충돌하면 - 보상을 준다. 
    충돌 여부는 유니티의 충돌 콜백함수 (OnCollisionEnter)에서 처리한다.
        - SetReward() : 이전 보상값을 지우고 현재의 보상값으로 대치한다. 이는 누적된 보상값이 필요없을 경우에 사용한다.
        - AddReward() : 보상을 받고 바로 에피소드가 종료시키지 않고 계속해서 학습해야 하는 환경에서 사용한다.

    ## 진행 순서
    1. Start
    2. OnEpisodeBegin
        2.1 CollectObservation
        2.2 OnActionReceived
        2.3 Update
        - Is done?