using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCube : MonoBehaviour
{
    private Rigidbody rigibody_;
    private Collider mycollider_;
    private CameraProperties camera_;
    private CaptureSystem captureSystem_;
    private PlayerController playerController_;
    private CubeAnimations cubeAnimations_;

    [Header("Variaveis de Alocação")]
    public GameObject fakeCube;
    private Vector3 target_;

    [SerializeField] 
    private float sucessPercentage_;
    private float impulseY_;
    private float speed_;

    private bool canMovement_;
    private bool break_;
    private bool abductionProcess;
    private bool moveCapture_;

    private Collider lastColliderDetected_;
    private Collider monsterColliderDetected_;
    private Transform fakeCube_, captureCube_;
    private Transform previewTarget_;

    private Vector3 monsterCaptureScale_;
    private Vector3 monsterCapturePosition_;
    private Quaternion monsterCaptureRotation_;    
    
    private float randomHelper_;
    private float shakeValue_ = 1;

    private bool afterCapture_;
    private bool monsterBreakFree_;
    private bool setshake_;
    private bool feedbackBool_;
    private bool canCollider_ = true;

    #region propties getter and setter
    [HideInInspector] public float sucessPercentage { get { return sucessPercentage_; } set { sucessPercentage_ = value; } }
    [HideInInspector] public Vector3 target { get { return target_; } set { target_ = value; } }
    [HideInInspector] public float impulseY { get { return impulseY_; } set { impulseY_ = value; } }
    [HideInInspector] public float speed { get { return speed_; } set { speed_ = value; } }
    #endregion

    private void Construt(CameraProperties newCamera)
    {
        camera_ = newCamera;
    }

    #region Funções MonoBehaviour de Execução
    private void Awake()
    {
        Construt(Camera.main.GetComponent<CameraProperties>());
        rigibody_ = GetComponent<Rigidbody>();
        mycollider_ = GetComponent<Collider>();
        playerController_ = FindObjectOfType<PlayerController>();
        captureSystem_ = FindObjectOfType<CaptureSystem>();
        cubeAnimations_ = GetComponent<CubeAnimations>();

        canMovement_ = true;
        canCollider_ = true;

        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;
        
        fakeCube_ = transform.GetChild(1);
        captureCube_ = transform.GetChild(0);
        captureCube_.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        rigibody_ = GetComponent<Rigidbody>();
        captureSystem_ = FindObjectOfType<CaptureSystem>();    

        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;

        captureCube_.localScale = Vector3.one;
        captureCube_.gameObject.SetActive(true);
        fakeCube_.gameObject.SetActive(false);

        canMovement_ = true;
        canCollider_ = true;
        break_ = false;
        
        shakeValue_ = 1;
    }

    private void OnDisable()
    {
        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;
    }

    private void Update()
    {
        Abduction(lastColliderDetected_);

        if (afterCapture_)
        {
            Quaternion tempRotation_ = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, tempRotation_, 5f * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target_) <= 1.5f && canMovement_ && !abductionProcess)
        {
            canMovement_ = false;
            rigibody_.useGravity = false;
        }

        MonsterGetOutCube();
        //StopDistanceControl();
    }

    private void FixedUpdate()
    {
        #region Movimentação do Cubo até o alvo
        if (rigibody_.useGravity && canMovement_ && !abductionProcess)
        {
            rigibody_.velocity = (target_ - transform.position).normalized * speed_ + new Vector3(0, impulseY_, 0);
            transform.Rotate(-900 * Time.deltaTime, 0, 0, Space.Self);
        }
        #endregion
    }
    #endregion

    private void Abduction(Collider other)
    {
        if (abductionProcess)
        {
            other.GetComponent<Rigidbody>().useGravity = false;
            
            other.transform.localScale = Vector3.Lerp(other.transform.localScale, Vector3.zero, 8f * Time.deltaTime);
            other.transform.position = Vector3.Lerp(other.transform.position, transform.position, 5f * Time.deltaTime);

            if (other.transform.localScale == Vector3.zero)
            {
                abductionProcess = false;
                other.transform.gameObject.SetActive(false);
                other.transform.SetParent(transform);
                rigibody_.useGravity = true;
                mycollider_.enabled = true;
                afterCapture_ = true;
            }
        }
    } // Função que controla a interação Cubo/Monstro
    
    private void MonsterGetOutCube()
    {
        if (monsterBreakFree_ && monsterColliderDetected_)
        {
            playerController_.CanMove_ = true;

            monsterColliderDetected_.transform.localScale = Vector3.Lerp(
                monsterColliderDetected_.transform.localScale, Vector3.one, 5 * Time.deltaTime);
            
            Vector3 monsterPosition = monsterColliderDetected_.transform.position;
            monsterColliderDetected_.transform.position = 
                new Vector3(monsterPosition.x, monsterCapturePosition_.y, monsterPosition.z);

            Quaternion tempRotation_ = Quaternion.Euler(0, monsterColliderDetected_.transform.rotation.eulerAngles.y, 0);
            monsterColliderDetected_.transform.rotation = tempRotation_;

            if (monsterColliderDetected_.transform.localScale == monsterCaptureScale_)
            {
                monsterBreakFree_ = false;
                monsterColliderDetected_ = null;
            }                 
        }
    }

    #region Interação com o Cubo após Colisões
    private IEnumerator StopCube(Collider col) //Função que controla a Execução do Cubo e puxa a Abudação do Monstro
    {
        mycollider_.enabled = false;
        yield return new WaitForSeconds(1f);
        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;
        rigibody_.freezeRotation = true;
        transform.LookAt(monsterColliderDetected_.transform);

        Debug.Log("Toca Animação");
        cubeAnimations_.Capturing();
        yield return new WaitUntil(() => fakeCube_.GetComponent<CaptureHelper>().canGo);
        moveCapture_ = false;
        abductionProcess = true;
        lastColliderDetected_ = col;
        yield break;
    }

    private void BreakCube()
    {
        rigibody_.velocity = Vector3.zero;
        rigibody_.useGravity = true;
        canMovement_ = false;
        GameObject t = Pooling.InstantiatePooling(fakeCube, fakeCube_.transform.position,
            fakeCube_.transform.rotation);
        transform.gameObject.SetActive(false);
        break_ = true;

        captureSystem_.captureProcess = false;
    } //Função que controla a interação Cubo/(Chão e Parede)

    private void FalseBreakCube()
    {
        rigibody_.velocity = Vector3.zero;
        rigibody_.useGravity = true;
        canMovement_ = false;
        GameObject t = Pooling.InstantiatePooling(fakeCube, fakeCube_.transform.position,
        fakeCube_.transform.rotation);
        mycollider_.enabled = false;
        captureSystem_.captureProcess = false;
        break_ = true;
    }
    #endregion

    #region Interação com o Cubo Pós Captura
    private IEnumerator ShakeItOff(Collider other)
    {
        afterCapture_ = false;
        abductionProcess = false;
        canCollider_ = false;

        yield return new WaitForSeconds(1);

        if (!setshake_)
        {
            feedbackBool_ = GetInOrOutMonsterChance(sucessPercentage_);
            randomHelper_ = Random.Range(1, 4);
            setshake_ = true;
        }

        cubeAnimations_.ShakeCube();
        yield return new WaitForSeconds(1);

        if (feedbackBool_ && shakeValue_ <= 2)
        {
            shakeValue_++;
            StartCoroutine(ShakeItOff(monsterColliderDetected_));
        }
        else if (feedbackBool_ && shakeValue_ > 2)
        {
            cubeAnimations_.DissolveCube();
            yield return new WaitForSeconds(1);

            camera_.SetCameraMode(CameraController.CameraMode.FollowPlayer);
            camera_.SetTarget(previewTarget_);

            #region Acesso ao CaptureSystem
            if (captureSystem_.cubeAmount > 0) captureSystem_.CaptureInstantiate();
            else captureSystem_.ExitCaptureMode();
            captureSystem_.captureProcess = false;
            #endregion

            setshake_ = false;
            yield break;
        }

        if (!feedbackBool_ && shakeValue_ == randomHelper_)
        {
            other.GetComponent<Rigidbody>().useGravity = true;
            other.transform.parent = null;
            other.gameObject.SetActive(true);

            monsterBreakFree_ = true;

            Debug.Log("Work" + other.name);
            FalseBreakCube();

            camera_.SetCameraMode(CameraController.CameraMode.FollowPlayer);
            camera_.SetTarget(previewTarget_);

            yield return new WaitUntil(() => !monsterBreakFree_);

            #region Acesso ao CaptureSystem
            if (captureSystem_.cubeAmount > 0)
                captureSystem_.CaptureInstantiate();
            else captureSystem_.ExitCaptureMode();
            captureSystem_.captureProcess = false;
            #endregion

            setshake_ = false;
            yield break;
        }
        else if (!feedbackBool_ && shakeValue_ != randomHelper_)
        {
            shakeValue_++;
            StartCoroutine(ShakeItOff(monsterColliderDetected_));
        }
    }

    private bool GetInOrOutMonsterChance(float chance_)
    {
        float randomValue_ = Random.Range(1, 101);

        if (randomValue_ > chance_) return false;
        else return true;
    }
    #endregion

    private void OnTriggerStay(Collider currentCollider)
    {
        lastColliderDetected_ = currentCollider;

        if (canCollider_ && !abductionProcess)
        {
            if ((currentCollider.gameObject.name == "Ground" || currentCollider.gameObject.name == "Wall") && !break_)
            {
                if (!afterCapture_)
                {
                    BreakCube();

                    if (captureSystem_.cubeAmount > 0)
                    {
                        captureSystem_.CaptureInstantiate();
                    }
                    else captureSystem_.ExitCaptureMode();

                    captureSystem_.captureProcess = false;
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

                    StartCoroutine(ShakeItOff(monsterColliderDetected_));
                }
            }
            else if (currentCollider.gameObject.tag == "Enemy")
            {
                monsterCaptureScale_ = currentCollider.transform.localScale;
                monsterCaptureRotation_ = currentCollider.transform.rotation;
                monsterCapturePosition_ = currentCollider.transform.position;
                monsterColliderDetected_ = currentCollider;

                canMovement_ = false;

                rigibody_.velocity = Vector3.zero;
                rigibody_.AddForce(-(currentCollider.transform.position - transform.position) * 7 + new Vector3(0, 35, 0), ForceMode.Impulse);
                rigibody_.AddTorque(Vector3.forward * -5, ForceMode.Impulse);
                StartCoroutine(StopCube(currentCollider));
            }
        }
    }
}
