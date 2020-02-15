using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCube : MonoBehaviour
{
    private Rigidbody rigibody_;
    private Collider mycollider_;
    private ICameraProperties camera_;

    [Header("Variaveis de Alocação")]
    public GameObject fakeCube;
    public Transform[] allcubes;

    private float sucessPercentage_;
    private Vector3 target_;
    private float impulseY_;
    private float speed_;

    private bool moviment_;
    private bool break_;
    private bool capture_;
    private bool moveCapture_;

    private Collider col2_, coliderMonster_;
    private Transform bigcube_, smallcube_;
    private Vector3 monsterPositionCap_;
    private Quaternion monsterRotationCap_;
    private Vector3 monsterScaleCap_;
    private bool afterCapture_;
    private bool canCollide_ = true;
    private bool monsterBreakFree_;
    private float shakeValue_ = 1;
    private Transform previewTarget_;
    private bool setshake_;
    private bool feedbackBool_;
    private float randomHelper_;
    private CaptureSystem captureSystem_;

    #region propties getter and setter
    // Esconder causa comportamento indefinido
    public float sucessPercentage { get { return sucessPercentage_; } set { sucessPercentage_ = value; } }
    [HideInInspector] public Vector3 target { get { return target_; } set { target_ = value; } }
    [HideInInspector] public float impulseY { get { return impulseY_; } set { impulseY_ = value; } }
    [HideInInspector] public float speed { get { return speed_; } set { speed_ = value; } }
    #endregion

    private void Construt(ICameraProperties newCamera)
    {
        camera_ = newCamera;
    }

    #region Funções MonoBehaviour de Execução
    private void Awake()
    {
        Construt (Camera.main.GetComponent<ICameraProperties>());

        InitializingAwake();
    }

    private void Update()
    {
        Abduction(col2_);
        AfterCaptureMonster();
        MonsterGetOut();
        StopDistanceControl();
    }

    private void FixedUpdate()
    {
        PhysicsControl();
    }

    private void OnEnable()
    {
        InitializaingOnEnable();
    }

    private void OnDisable()
    {
        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;
    }
    #endregion

    #region Funções de Inicialização 
    private void InitializingAwake()
    {
        moviment_ = true;
        canCollide_ = true;
        rigibody_ = GetComponent<Rigidbody>();
        mycollider_ = GetComponent<Collider>();
        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;
        bigcube_ = transform.GetChild(1);
        smallcube_ = transform.GetChild(0);
        smallcube_.localScale = Vector3.one;
        captureSystem_ = FindObjectOfType<CaptureSystem>();
    }

    private void InitializaingOnEnable()
    {
        shakeValue_ = 1;
        rigibody_ = GetComponent<Rigidbody>();
        smallcube_.localScale = Vector3.one;
        smallcube_.gameObject.SetActive(true);
        bigcube_.gameObject.SetActive(false);
        rigibody_.useGravity = false;
        canCollide_ = true;
        break_ = false;
        rigibody_.velocity = Vector3.zero;
        moviment_ = true;
        captureSystem_ = FindObjectOfType<CaptureSystem>();
    }
    #endregion

    #region Movimentação do Cubo Antes das Colisões
    private void StopDistanceControl() // Verifica a distância do Cubo até seu Alvo
    {
        if (Vector3.Distance(transform.position, target_) <= 1.5f && moviment_ && !capture_)
        {
            moviment_ = false;
            rigibody_.useGravity = false;
        }
    }

    private void PhysicsControl() // Física no FixedUpdate sobre a Movimentação do Cubo até o alvo;
    {
        if (rigibody_.useGravity && moviment_ && !capture_)
        {
            rigibody_.velocity = (target_ - transform.position).normalized * speed_ + new Vector3(0, impulseY_, 0);
            transform.Rotate(-900 * Time.deltaTime, 0, 0, Space.Self);
        }
    }
    #endregion

    #region Interação com o Cubo após Colisões
    private IEnumerator StopCube(Collider col) //Função que controla a Execução do Cubo e puxa a Abudação do Monstro
    {
        mycollider_.enabled = false;
        yield return new WaitForSeconds(1f);
        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;
        rigibody_.freezeRotation = true;
        transform.LookAt(coliderMonster_.transform);
        Debug.Log("Toca Animação");
        bigcube_.GetComponent<Animator>().Play("Capturing", -1, 0);
        yield return new WaitUntil(() => bigcube_.GetComponent<CaptureHelper>().canGo);
        moveCapture_ = false;
        capture_ = true;
        col2_ = col;
        yield break;
    }

    private void Abduction(Collider coll)
    {
        if (capture_)
        {
            coll.GetComponent<Rigidbody>().useGravity = false;
            coll.transform.localScale = Vector3.Lerp(coll.transform.localScale, Vector3.zero, 8f * Time.deltaTime);
            coll.transform.position = Vector3.Lerp(coll.transform.position, transform.position, 5f * Time.deltaTime);

            if (coll.transform.localScale == Vector3.zero)
            {
                coll.transform.SetParent(transform);
                capture_ = false;
                coll.transform.gameObject.SetActive(false);
                rigibody_.useGravity = true;
                mycollider_.enabled = true;
                afterCapture_ = true;
            }
        }
    } // Função que controla a interação Cubo/Monstro

    private void BreakCube()
    {
        rigibody_.velocity = Vector3.zero;
        rigibody_.useGravity = true;
        moviment_ = false;
        GameObject t = Pooling.InstantiatePooling(fakeCube, bigcube_.transform.position,
            bigcube_.transform.rotation);
        transform.gameObject.SetActive(false);
        break_ = true;

        captureSystem_.capturingProcess = false;
    } //Função que controla a interação Cubo/(Chão e Parede)

    private void FalseBreakCube()
    {
        rigibody_.velocity = Vector3.zero;
        rigibody_.useGravity = true;
        moviment_ = false;
        GameObject t = Pooling.InstantiatePooling(fakeCube, bigcube_.transform.position,
        bigcube_.transform.rotation);
        mycollider_.enabled = false;
        captureSystem_.capturingProcess = false;
        break_ = true;
    }

    private void OnTriggerStay(Collider col)
    {
        col2_ = col;

        if (canCollide_)
        {
            if ((col.gameObject.name == "Ground" || col.gameObject.name == "Wall") && !break_ && !capture_)
            {
                if (!afterCapture_)
                {
                    BreakCube();
                    
                    if (captureSystem_.cuboQuantidade > 0)
                    {
                        captureSystem_.CaptureInstantiate();
                        captureSystem_.capturingProcess = false;
                    }

                    else
                    {
                        captureSystem_.ExitCaptureMode();
                        captureSystem_.capturingProcess = false;
                    }

                    captureSystem_.capturingProcess = false;
                }
                else
                {
                    Debug.Log("Oi");
                    mycollider_.isTrigger = false;

                    if (camera_.GetTarget() != transform)
                    {
                        camera_.SetCameraMode(CameraController.CameraMode.Capturing);
                        previewTarget_ = camera_.GetTarget();
                        camera_.SetTarget(transform);
                    }

                    StartCoroutine(ShakeItOff(coliderMonster_));
                }
            }
            else if (col.gameObject.tag == "Enemy" && !capture_)
            {
                monsterScaleCap_ = col.transform.localScale;
                monsterRotationCap_ = col.transform.rotation;
                monsterPositionCap_ = col.transform.position;
                coliderMonster_ = col;
                col.GetComponent<MonsterBase>().beenCapture = true; // freeza navmesh
                moviment_ = false;
                rigibody_.velocity = Vector3.zero;
                rigibody_.AddForce(-(col.transform.position - transform.position) * 7 + new Vector3(0, 35, 0), ForceMode.Impulse);
                rigibody_.AddTorque(Vector3.forward * -5, ForceMode.Impulse);
                StartCoroutine(StopCube(col));
            }
        }
    }  //Verifica Onde o Cubo bateu
    #endregion

    #region Interação com o Cubo Pós Captura
    private void AfterCaptureMonster()
    {
        if (afterCapture_)
        {
            Quaternion tempRotation_ = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, tempRotation_, 5f * Time.deltaTime);
        }
    }

    private void MonsterGetOut()
    {
        if (monsterBreakFree_ && coliderMonster_)
        {
            Debug.Log("Hmmmm");
            coliderMonster_.transform.localScale = Vector3.Lerp(coliderMonster_.transform.localScale, Vector3.one,
                5 * Time.deltaTime);

            coliderMonster_.transform.position =
                new Vector3(coliderMonster_.transform.position.x, monsterPositionCap_.y, coliderMonster_.transform.position.z);

            Quaternion tempRotation_ = Quaternion.Euler(0, coliderMonster_.transform.rotation.eulerAngles.y, 0);

            coliderMonster_.transform.rotation = tempRotation_;

            if (coliderMonster_.transform.localScale == monsterScaleCap_)
            {
                monsterBreakFree_ = false;
                coliderMonster_ = null;
            }
        }
    }

    private IEnumerator ShakeItOff(Collider col)
    {
        afterCapture_ = false;
        capture_ = false;
        canCollide_ = false;
     

        yield return new WaitForSeconds(1);

        if (!setshake_){

            feedbackBool_ = GetInOrOutMonsterChance(sucessPercentage_);
            randomHelper_ = Random.Range(1, 4);
            setshake_ = true;
        }

       
        bigcube_.GetComponent<Animator>().Play("ShakeCube", -1, 0);
        yield return new WaitForSeconds(1);


        if (feedbackBool_ && shakeValue_  <= 2)
        {
            shakeValue_++;
            StartCoroutine(ShakeItOff(coliderMonster_));
        }

        else if (feedbackBool_ && shakeValue_ > 2)
        {
            bigcube_.GetComponent<Animator>().Play("DissolveCubo", -1, 0);
            yield return new WaitForSeconds(1);
            camera_.SetCameraMode(CameraController.CameraMode.FollowPlayer);
            camera_.SetTarget(previewTarget_);

            #region Acesso ao CaptureSystem
            if (captureSystem_.cuboQuantidade > 0) captureSystem_.CaptureInstantiate();
            else captureSystem_.ExitCaptureMode();
            captureSystem_.capturingProcess = false;
            #endregion
            setshake_ = false;
            yield break;
        }

        if (!feedbackBool_ && shakeValue_ == randomHelper_)
        {
            col.GetComponent<Rigidbody>().useGravity = true;
            col.transform.parent = null;
            col.gameObject.SetActive(true);
            monsterBreakFree_ = true;
            Debug.Log("Work" + col.name);
            FalseBreakCube();
            camera_.SetCameraMode(CameraController.CameraMode.FollowPlayer);
            camera_.SetTarget(previewTarget_);
            yield return new WaitUntil(() => !monsterBreakFree_);
            col.GetComponent<MonsterBase>().beenCapture = false;
            #region Acesso ao CaptureSystem
            if (captureSystem_.cuboQuantidade > 0) captureSystem_.CaptureInstantiate();
            else captureSystem_.ExitCaptureMode();
            captureSystem_.capturingProcess = false;
            #endregion
            setshake_ = false;
            yield break;
        }

        else if(!feedbackBool_ && shakeValue_ != randomHelper_)
        {
            shakeValue_++;
            StartCoroutine(ShakeItOff(coliderMonster_));
        }
    }

    private bool GetInOrOutMonsterChance(float chance_)
    {
        float randomValue_ = Random.Range(1, 101);

        if (randomValue_ > chance_)
            return false;
        else
            return true;
    }
    #endregion
}
