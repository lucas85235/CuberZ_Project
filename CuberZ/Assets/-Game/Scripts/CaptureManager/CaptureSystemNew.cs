using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSystemNew : MonoBehaviour
{

    #region Public and Custom Variables
    [Header("Alocation")]
    public GameObject captureCubePrefab;
    public GameObject fakeCube;
    public Transform spawnPoint;

    [Header("Cube Parameters")]
    public float cubeSpeed;
    public float cubeForceY;
    public float turnLerpSpeed;
    public float goDownDistance;

    [Header("Others")]
    public float sucessPercentage;
    [Range(0, 999)] public int cubeQuantity;
    [HideInInspector] public bool startCapturing { get; set; }
    [HideInInspector] public bool waitingForCaptureEnd { get; set; }
    [HideInInspector] public bool finishCubeCatchAnimator { get; set; }
    [HideInInspector] public bool tryToCatchKubber { get; set; }
    [HideInInspector] public Transform previewTarget { get; set; }
    [HideInInspector] public bool captureMode_ { get; set; }
    #endregion

    #region Private Variables
    private LayerMask layerMask_;
    private IInput input_;
    private PlayerController playerController_;
    private PlayerAnimation playerAnimation_;
    private CameraController cameraController_;
    private KubberzInventory kubberzInventory_;
    private GameObject cubeTemp_;
    private Vector3 cubeGoalPosistion;
    private float lastCubeGoalPointX_;
    private float lastCubeGoalPointY_;
    private float lastCubeGoalPointZ_;
    private float distanceCubeandGoal_;
    private Rigidbody rigidy_;
    private bool goCubeToGoal_;
    private bool goDown_;
    private bool turning_;
    private bool putKubberInCUBE_;
    private bool rotateCubeTorNormal;
    private bool catchKubber_;
    private float numberOfShakes_;
    private bool setShakeVariables_;
    private bool monsterBreakFree_;
    private Vector3 tempVelocity_;
    #endregion

    private void Construt(IInput newInputInterface) { input_ = newInputInterface; }

    #region Monster RecordedVariables
    private Vector3 monsterCaptureScale_;
    private Quaternion monsterCaptureRotation_;
    private Vector3 monsterCapturePosition_;
    public Collider currentMonsterColider_ { get; set; }
    #endregion

    #region Funções Lambda
    private bool CanThrowCube() => input_.ExecuteAction() && !waitingForCaptureEnd && ThrowPointRange();
    private bool CanEnterCaptureMode() => (cubeQuantity > 0 && !captureMode_ && !waitingForCaptureEnd) ? true : false;
    private bool CanExitCaptureMode() => (captureMode_ && !waitingForCaptureEnd && !monsterBreakFree_) ? true : false;
    #endregion

    #region Funções MonoBehaviour
    private void OnEnable()
    {
        //resetar toGoal e finishCubeCatchAnimatior
    }

    private void Start()
    {
        Construt(Object.FindObjectOfType<DesktopInputImpl>());

        playerAnimation_ = FindObjectOfType<PlayerAnimation>();
        playerController_ = FindObjectOfType<PlayerController>();
        cameraController_ = FindObjectOfType<CameraController>();
        kubberzInventory_ = FindObjectOfType<KubberzInventory>();

        layerMask_ = LayerMask.GetMask("Input");
    }

    private void Update()
    {
        #region Inicializa quando o cubo começa a captura
        if (startCapturing) RecordMonsterInformation(currentMonsterColider_); //Quando começa a capturar absorve a informação
        #endregion
        if (turning_) LerpPlayerFowardWithCubeGoal();
        if (goCubeToGoal_) GoCubeGoAnalytics();
        if (putKubberInCUBE_) KubberCapturingProcess(currentMonsterColider_);
        if (rotateCubeTorNormal) CubeRotateToNormal();
        if (monsterBreakFree_) MonsterBreakedFree(currentMonsterColider_);
        if(cubeTemp_ && cubeTemp_.transform.parent != null)
        {
            cubeTemp_.transform.position = spawnPoint.transform.position;
            cubeTemp_.transform.rotation = spawnPoint.transform.rotation;   
        } 

        CaptureModeEnterAndExit();
        InputCaptureModeControlState();
    }

    private void FixedUpdate()
    {
        if (goCubeToGoal_) GoCubeGoPhysics();
    }
    #endregion

    #region Capturando o Kubber

    private void FalseBreakCube()
    {
        GameObject t = Instantiate(fakeCube, cubeTemp_.transform.position, cubeTemp_.transform.rotation);
        monsterCapturePosition_ = new Vector3(cubeTemp_.transform.position.x, monsterCapturePosition_.y, cubeTemp_.transform.position.z);
        GetRigidyBody(cubeTemp_).velocity = Vector3.zero;
        GetRigidyBody(cubeTemp_).useGravity = false;
        currentMonsterColider_.transform.parent = null;
        Destroy(cubeTemp_);
        ResetCaptureSystem(false);
    }

    private void KubberCapturingProcess(Collider kubberCollider)
    {
        Debug.Log("D " + kubberCollider.name);
        kubberCollider.GetComponent<Rigidbody>().useGravity = false;
        rotateCubeTorNormal = true;
        kubberCollider.transform.localScale = Vector3.Lerp(kubberCollider.transform.localScale, Vector3.zero, 8f * Time.deltaTime);
        kubberCollider.transform.position = Vector3.Lerp(kubberCollider.transform.position, cubeTemp_.transform.position, 5f * Time.deltaTime);

        if (kubberCollider.transform.localScale == Vector3.zero)
        {
            putKubberInCUBE_ = false;
            kubberCollider.transform.gameObject.SetActive(false);
            kubberCollider.transform.SetParent(cubeTemp_.transform);
            GetRigidyBody(cubeTemp_).useGravity = true;
            cubeTemp_.GetComponent<Collider>().enabled = true;
            cubeTemp_.GetComponent<Collider>().isTrigger = false;
            tryToCatchKubber = true;
        }
    }

    private void CubeRotateToNormal()
    {

        Quaternion tempRotation_ = Quaternion.Euler(0, cubeTemp_.transform.rotation.eulerAngles.y, 0);
        cubeTemp_.transform.rotation = Quaternion.Lerp(cubeTemp_.transform.rotation, tempRotation_, 5f * Time.deltaTime);

    }

    private IEnumerator ShakeCube(Collider MonsterCollider)
    {
        #region Setando variaveis e dando Start no Shake
        previewTarget = cameraController_.GetTarget();
        cameraController_.SetTarget(cubeTemp_.transform);

        yield return new WaitForSeconds(1f);

        if (!setShakeVariables_)
        {
            rotateCubeTorNormal = false;
            goCubeToGoal_ = false;
            monsterBreakFree_ = false;

            catchKubber_ = GetInOrOutMonsterChance(sucessPercentage);
            if (!catchKubber_) numberOfShakes_ = Random.Range(1, 4);
            else numberOfShakes_ = 0;
            setShakeVariables_ = true;
        }

        cubeTemp_.GetComponent<CubeAnimations>().ShakeCube();

        yield return new WaitForSeconds(1.5f);

        #endregion

        #region Casos de Captura

        #region Caso dê certo a Captura
        if (catchKubber_ && numberOfShakes_ < 2) //Captura do Kubber foi bem sucedida mas ainda não fez todas as balançadas
        {
            numberOfShakes_++;
            StartCoroutine(ShakeCube(MonsterCollider));
        }

        else if (catchKubber_ && numberOfShakes_ >= 2) //Captura do Kubber foi bem sucedida e terminou a ultima balançada;
        {
            cubeTemp_.GetComponent<CubeAnimations>().DissolveCube();
            yield return new WaitForSeconds(1f);

            cameraController_.SetCameraMode(CameraProperties.CameraMode.FollowPlayer);
            cameraController_.SetTarget(transform.Find("CameraTarget"));

            if (cubeQuantity > 0) SpawnCubeOnHand();
            else ExitCaptureMode(false);

            startCapturing = false;

            setShakeVariables_ = false;

            playerController_.CanMove_ = true;

            ResetCaptureSystem(false);

            currentMonsterColider_.transform.parent = null;
            currentMonsterColider_.gameObject.SetActive(false);


            kubberzInventory_.AddKubberInNextEmptySlot(currentMonsterColider_.gameObject);
            playerController_.AddKubberInventoryOnTeam();

            yield break;
        }
        #endregion

        #region Caso não dê certo a Captura
        if (!catchKubber_ && numberOfShakes_ >= 2)
        {

            monsterBreakFree_ = true;
            FalseBreakCube();

            cameraController_.SetCameraMode(CameraProperties.CameraMode.FollowPlayer);
            cameraController_.SetTarget(transform.Find("CameraTarget"));

            yield return new WaitUntil(() => !monsterBreakFree_);

            startCapturing = false;

            setShakeVariables_ = false;

            yield break;
        } //Não teve sucesso na captura e acabou a animação

        else if (!catchKubber_ && numberOfShakes_ < 2) //Não teve sucesso na captura e ainda não acabou a animação
        {
            numberOfShakes_++;
            StartCoroutine(ShakeCube(MonsterCollider));
        }
        #endregion

        #endregion
    } // quem chama essa função é o proprio ao cair no chão

    private void MonsterBreakedFree(Collider monsterColliderDetected)
    {
        Debug.Log("hhggg " + monsterColliderDetected.name);

        cameraController_.SetCameraMode(CameraProperties.CameraMode.FollowPlayer);
        cameraController_.SetTarget(transform.Find("CameraTarget"));

        if (monsterColliderDetected.transform.localScale != Vector3.one)
        {
            #region Setar parâmetros normais antigos do Kubber
            monsterColliderDetected.GetComponent<Collider>().enabled = true;
            monsterColliderDetected.GetComponent<Rigidbody>().useGravity = true;
            monsterColliderDetected.transform.parent = null;
            monsterColliderDetected.gameObject.SetActive(true);
            #endregion

            #region Setar parâmetros de movimentação e rotação
            monsterColliderDetected.transform.localScale = Vector3.Lerp(
            monsterColliderDetected.transform.localScale, Vector3.one, 5 * Time.deltaTime);

            monsterColliderDetected.transform.position = monsterCapturePosition_;

            Quaternion tempRotation_ = Quaternion.Euler(0, monsterColliderDetected.transform.rotation.eulerAngles.y, 0);
            monsterColliderDetected.transform.rotation = monsterCaptureRotation_;
            #endregion
        }


        else if (monsterColliderDetected.transform.localScale.x >= 0.8f)
        {
            ResetCaptureSystem(true);
            monsterBreakFree_ = false;
            currentMonsterColider_ = null;
            Debug.Log("ue");
        }
    }

    public void AcessShakeCubeCoroutine()
    {
        StartCoroutine(ShakeCube(currentMonsterColider_));
    }

    #endregion

    #region Atacando o Cubo

    private Vector3 CubeGoalPositionCalculator()
    {
        RaycastHit hit;
        Ray mousePoint_ = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (Physics.Raycast(mousePoint_, out hit, Mathf.Infinity, layerMask_))
        {
            Debug.Log(hit.transform.name);
            cubeGoalPosistion = new Vector3(hit.point.x, mousePosition.y, hit.point.z);
            lastCubeGoalPointX_ = cubeGoalPosistion.x;
            lastCubeGoalPointY_ = cubeGoalPosistion.y;
            lastCubeGoalPointZ_ = cubeGoalPosistion.z;
        }
        else cubeGoalPosistion = new Vector3(lastCubeGoalPointX_, lastCubeGoalPointY_, lastCubeGoalPointZ_);

        Vector3 FinalPos = cubeGoalPosistion;
        distanceCubeandGoal_ = Vector3.Distance(transform.position, FinalPos);

        return FinalPos;
    }

    private void LerpPlayerFowardWithCubeGoal()
    {

        Vector3 vectorWithoutY = new Vector3(cubeGoalPosistion.x, playerController_.transform.position.y, cubeGoalPosistion.z);
        Vector3 final = vectorWithoutY - playerController_.transform.position;
        playerController_.transform.forward = Vector3.Lerp(playerController_.transform.forward, final, turnLerpSpeed * Time.deltaTime);
        turning_ = (playerController_.transform.forward == final) ? false : true;
    }

    private void GoCubeGoPhysics() //FixedUpdate do Cubo
    {
        if (!startCapturing && cubeTemp_)
        {
            cubeTemp_.transform.Rotate(-900 * Time.deltaTime, 0, 0, Space.Self);

            if (!goDown_) //Faz o arco quando está longe do target
                GetRigidyBody(cubeTemp_).velocity = (tempVelocity_ - cubeTemp_.transform.position).normalized * cubeSpeed + new Vector3(0, cubeForceY, 0);

            else //Vai direto para o target ao estar perto dele
                GetRigidyBody(cubeTemp_).velocity = (tempVelocity_ - cubeTemp_.transform.position).normalized * cubeSpeed;
        }
    }

    private void GoCubeGoAnalytics() //Update do Cubo
    {
        #region distancia do cubo até o objetivo
        if (cubeTemp_)
        {
            float d = Vector3.Distance(cubeTemp_.transform.position, cubeGoalPosistion); //Calcula distancia do cubo pro target
            goDown_ = (d <= goDownDistance) ? true : false; //recebe true ou false dependendo da distancia
        }
        #endregion

        #region velocidade do cubo
        tempVelocity_ = new Vector3(cubeGoalPosistion.x, 0, cubeGoalPosistion.z); //Recebe o valor do objetivo excluindo o Y
        #endregion
    }

    private void RecordMonsterInformation(Collider currentCollider)
    {
        monsterCaptureRotation_ = currentCollider.transform.rotation;
        monsterCapturePosition_ = currentCollider.transform.position;
        goCubeToGoal_ = false;

        GetRigidyBody(cubeTemp_).velocity = Vector3.zero;
        GetRigidyBody(cubeTemp_).AddForce((currentCollider.transform.position - transform.position) * 2 + new Vector3(0, 45, 0), ForceMode.Impulse);
        GetRigidyBody(cubeTemp_).AddTorque(Vector3.forward * -3, ForceMode.Impulse);
        startCapturing = false;
        StartCoroutine(WaitTimerToStopCubeOnAir(currentCollider));
    } //Momento exato que bate no Kubber

    #endregion

    #region Funções puxadas por animação

    public void ThrowCube() // Ataca o cubo pela animação
    {
        cubeTemp_.transform.SetParent(null);
        goCubeToGoal_ = true;
        GetRigidyBody(cubeTemp_).useGravity = true;
        cubeTemp_.GetComponent<Collider>().enabled = true;
        cubeTemp_.GetComponent<CubeAnimations>().DecreaseCaptureCube();
        cubeTemp_.transform.GetChild(1).gameObject.SetActive(true);
        cubeTemp_.GetComponent<CubeAnimations>().ExpandFakeCube();
    }

    #endregion

    #region CaptureMode ChangeState Functions

    private void InputCaptureModeControlState()
    {
        if (captureMode_ && CanThrowCube()) //Estando no modo de captura a parte para capturar
        {
            CubeGoalPositionCalculator();
            playerController_.CanMove_ = false;
            waitingForCaptureEnd = true;
            turning_ = true;
            playerAnimation_.ThrowCube();
        }
    }

    public void ResetCaptureSystem(bool spawnNewCube)
    {
        playerController_.CanMove_ = true;
        turning_ = false;
        waitingForCaptureEnd = false;
        startCapturing = false;
        goDown_ = false;
        rotateCubeTorNormal = false;
        goCubeToGoal_ = false;
        finishCubeCatchAnimator = false;
        catchKubber_ = false;
        tryToCatchKubber = false;
        if (spawnNewCube && cubeQuantity > 0) SpawnCubeOnHand();
        else if (spawnNewCube && cubeQuantity <= 0) ExitCaptureMode(false);

    }

    private void EnterInCaptureMode()
    {
        captureMode_ = true;
    }

    private void ExitCaptureMode(bool incrmentCube)
    {
        captureMode_ = false;
        cubeTemp_.transform.parent = null;
        Destroy(cubeTemp_);
        cubeTemp_ = null;
        if (incrmentCube) cubeQuantity++;
    }

    private void CaptureModeEnterAndExit()
    {
        if (input_.EnterInCaptureMode() && CanEnterCaptureMode())
        {
            SpawnCubeOnHand();
            EnterInCaptureMode();
        }

        else if (input_.EnterInCaptureMode() && CanExitCaptureMode())
        {
            ExitCaptureMode(true);
        }
    } //Função principal no Update

    private void SpawnCubeOnHand() //Apenas para o cubo ficar na mão do personagem.
    {
        if (cubeQuantity > 0)
        {
            if (cubeTemp_) Destroy(cubeTemp_);
            cubeTemp_ = null;
            cubeTemp_ = Instantiate(captureCubePrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            cubeTemp_.GetComponent<CaptureCubeNew>().alreadyInUse = false;
            cubeTemp_.GetComponent<CaptureCubeNew>().canShakeCube = false;
            cubeTemp_.GetComponent<Collider>().enabled = false;
            cubeTemp_.transform.SetParent(spawnPoint.transform);
            cubeTemp_.SetActive(true);
            GetRigidyBody(cubeTemp_).useGravity = false;

            cubeQuantity--;
        }

        else
        {
            ExitCaptureMode(false);
        }
    }
    #endregion

    #region Funções Úteis

    private Rigidbody GetRigidyBody(GameObject obj) => obj.GetComponent<Rigidbody>();

    private bool ThrowPointRange(float a = 15f, float b = 80f)
    {
        if (cubeTemp_)
        {
            return Vector3.Distance(cubeTemp_.transform.position, CubeGoalPositionCalculator()) >= a &&
                    Vector3.Distance(cubeTemp_.transform.position, CubeGoalPositionCalculator()) <= b;
        }

        else return false;

    }

    private IEnumerator WaitTimerToStopCubeOnAir(Collider currentCollider)
    {
        startCapturing = false;
        currentCollider.enabled = false;
        yield return new WaitForSeconds(1f);
        GetRigidyBody(cubeTemp_).useGravity = false;
        GetRigidyBody(cubeTemp_).velocity = Vector3.zero;
        GetRigidyBody(cubeTemp_).angularVelocity = Vector3.zero;
        GetRigidyBody(cubeTemp_).freezeRotation = true;
        cubeTemp_.transform.LookAt(currentMonsterColider_.transform);
        cubeTemp_.GetComponent<CubeAnimations>().Capturing();
        yield return new WaitUntil(() => finishCubeCatchAnimator);
        putKubberInCUBE_ = true;
        yield break;
    }

    private bool GetInOrOutMonsterChance(float chance_)
    {
        float randomValue_ = Random.Range(1, 101);

        if (randomValue_ > chance_) return false;
        else return true;
    }

    #endregion
}
