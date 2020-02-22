using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CaptureSystem : MonoBehaviour
{
    [Header("Variaveis de Alocação")]
    public GameObject theaterCube;
    public GameObject captureCube;
    public Transform hand;
    private LayerMask layermask_;

    [Header("Variaveis de Captura")]
    public float impulseForce = 20;
    public float impulseY = 10;
    [Range(0, 100)]
    public float sucessPercentage;

    [Range(0, 10f)]
    public float distanceMultiplier = 1;

    [Header("Variaveis de Feedback")]
    public int cuboQuantidade = 1000;

    public bool throwbool;
    public bool capturing;
    public bool captured;
    public bool capturingProcess;

    //Variaveis Privadas
    private IInput input_;
    private PlayerController player_;
    private PlayerAnimation playerAnimation_;
    private GameObject captureCubeTemp_;
    private Vector3 holdTarget_;
    private Vector3 hitPointV3_;
    private float distance_;
    private float xv3_, yv3_, zv3_;
    private bool cubeOnWorld_;
    private bool sizing_;
    private GameObject kuberTemp_;
    private bool turnToCall_;
    private Vector3 helperPoint_;

    private void Construt(IInput newInputInterface)
    {
        input_ = newInputInterface;
    }

    private void Awake()
    {
        layermask_ = LayerMask.GetMask("Input");
        player_ = GetComponent<PlayerController>();
        Construt(Object.FindObjectOfType<InputSystem>());
    }

    private void Start()
    {
        playerAnimation_ = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        #region Inputs
        if (input_.CaptureKubberInput())
        {
            if (!capturing && !capturingProcess && cuboQuantidade > 0 && !cubeOnWorld_)
            {
                if (!player_.jump && player_.isEnabled)
                    EnterCaptureMode();
            }
            else if (input_.CaptureKubberInput() && capturing && !capturingProcess && !captured)
            {
                ExitCaptureMode();
            }
        }

        if (input_.ExecuteActionInput())
        {
            if (ThrowPointRange() && capturing && !capturingProcess && !cubeOnWorld_)
            {
                captureCubeTemp_.GetComponent<CaptureCube>().target = MiraCube();
                holdTarget_ = captureCubeTemp_.GetComponent<CaptureCube>().target;
                throwbool = true;
            }
        }

        if (input_.RescueKubberInput())
        {
            if (!capturing && !capturingProcess && !sizing_ && !cubeOnWorld_ && CallPointRange())
            {
                theaterCube.SetActive(true);
                cubeOnWorld_ = true;
                player_.canMove_ = false;
                playerAnimation_.CallMoster();
                turnToCall_ = true;
                helperPoint_ = MiraCube();
            }

            else if (!capturing && !capturingProcess && !sizing_ && kuberTemp_ && cubeOnWorld_)
            {
                theaterCube.SetActive(true);
                player_.canMove_ = false;
                playerAnimation_.CallMoster();
                turnToCall_ = true;
                helperPoint_ = MiraCube();
            }

        }
        #endregion

        if (throwbool) ThrowProcess();

        if (captureCubeTemp_ && captureCubeTemp_.transform.parent)
        {
            captureCubeTemp_.transform.position = hand.transform.position;
        }

        if (sizing_ && kuberTemp_)
        {
            GoSizeKubber(kuberTemp_);
            turnToCall_ = false;
        }

        if (turnToCall_) CallingProcess();
    }

    #region Funções usaveis/Publicas
    public void ThrowProcess()
    {
        capturingProcess = true;
        Vector3 tempV_ = new Vector3(holdTarget_.x, transform.position.y, holdTarget_.z) - transform.position;
        transform.forward = Vector3.Lerp(transform.forward, tempV_, 0.5f * Time.deltaTime);
        playerAnimation_.ThrowCube();
    }

    public void CallingProcess()
    {
        if (kuberTemp_)
        {
            if (!kuberTemp_.GetComponent<MonsterBase>().spawnByPlayer)
            {
                Vector3 tempV_ = new Vector3(helperPoint_.x, transform.position.y, helperPoint_.z) - transform.position;
                transform.forward = Vector3.Lerp(transform.forward, tempV_, 0.5f * Time.deltaTime);
            }

            else
            {
                Vector3 tempV_ = new Vector3(kuberTemp_.transform.position.x, transform.position.y, kuberTemp_.transform.position.z) - transform.position;
                transform.forward = Vector3.Lerp(transform.forward, tempV_, 0.5f * Time.deltaTime);
            }
        }

        else
        {
            Vector3 tempV_ = new Vector3(helperPoint_.x, transform.position.y, helperPoint_.z) - transform.position;
            transform.forward = Vector3.Lerp(transform.forward, tempV_, 0.5f * Time.deltaTime);
        }
    }

    public void ThrowCube()
    {
        if (cuboQuantidade > 0)
        {
            captureCubeTemp_.transform.SetParent(null);
            captureCubeTemp_.GetComponent<Collider>().enabled = true;
            captureCubeTemp_.transform.GetChild(0).GetComponent<Animator>().Play("DiminuirCuboPequeno", -1, 0);
            captureCubeTemp_.transform.GetChild(1).gameObject.SetActive(true);
            captureCubeTemp_.GetComponent<Rigidbody>().useGravity = true;

            captureCubeTemp_.GetComponent<CaptureCube>().speed = impulseForce + (distance_ * distanceMultiplier);

            captureCubeTemp_ = null;
            cuboQuantidade--;

        }
        else Debug.Log("Você não possui mais cubos para jogar!");
    }

    public void SpawnKubberOnWorld()
    {
        if (!kuberTemp_) kuberTemp_ = Pooling.InstantiatePooling(player_.monster[0], helperPoint_, player_.monster[0].transform.rotation);
        else if (kuberTemp_ && !kuberTemp_.activeInHierarchy) kuberTemp_.transform.position = helperPoint_;
        sizing_ = true;
 
    }

    public void EnterCaptureMode() //Função para entrar no modo de captura
    {
        capturing = true;
        CaptureInstantiate();

        //Ações Sobre o Player (Não pode pular, movimento lento)
    }

    public void ExitCaptureMode()
    {
        capturing = false;
        if (captureCubeTemp_)
        {
            captureCubeTemp_.SetActive(false);
            captureCubeTemp_.transform.SetParent(null);
            captureCubeTemp_ = null;
        }
    }

    public void CaptureInstantiate() //Função para instanciar o Cubo que estará selecionado (Não tem seleção ainda).
    {
        if (!captureCubeTemp_)
        {
            captureCubeTemp_ = Pooling.InstantiatePooling(captureCube, hand.position, Quaternion.identity);
            captureCubeTemp_.transform.SetParent(hand.transform);
            captureCubeTemp_.transform.position = hand.transform.position;
            captureCubeTemp_.GetComponent<Collider>().enabled = false;
            captureCubeTemp_.GetComponent<CaptureCube>().impulseY = impulseY;
            captureCubeTemp_.GetComponent<CaptureCube>().sucessPercentage = sucessPercentage;
            captureCube.GetComponent<Rigidbody>().useGravity = false;
            captureCube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void GoToWalkAnimator()
    {
        throwbool = false;
        playerAnimation_.ResetAll();
    }

    public void CallingToWalk()
    {
        theaterCube.SetActive(false);
        player_.canMove_ = true;
        playerAnimation_.ResetAll();
    }
    #endregion

    #region Funções Protegidas/Privadas
    private Vector3 MiraCube()
    {
        RaycastHit hit;
        Ray mousePosition_ = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mouseposV3_ = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (Physics.Raycast(mousePosition_, out hit, Mathf.Infinity, layermask_))
        {
            Debug.Log(hit.transform.name);
            hitPointV3_ = new Vector3(hit.point.x, mouseposV3_.y, hit.point.z);
            xv3_ = hitPointV3_.x;
            yv3_ = hitPointV3_.y;
            zv3_ = hitPointV3_.z;
        }
        else hitPointV3_ = new Vector3(xv3_, yv3_, zv3_);

        Vector3 FinalPos = hitPointV3_;
        distance_ = Vector3.Distance(transform.position, FinalPos);

        return FinalPos;
    }

    private bool ThrowPointRange()
    {
        return Vector3.Distance(transform.position, MiraCube()) >= 15.0f &&
               Vector3.Distance(transform.position, MiraCube()) <= 80.0f;
    }

    private bool CallPointRange()
    {

        return Vector3.Distance(transform.position, MiraCube()) >= 5f &&
               Vector3.Distance(transform.position, MiraCube()) <= 15f;
    }

    private Vector3 CubeDirection()
    {
        Vector3 v1_ = MiraCube();
        Vector3 v2_ = hand.transform.position;
        Vector3 dirFinal_ = ((v1_ - v2_).normalized);
        return dirFinal_;

    }

    private void GoSizeKubber(GameObject kubber)
    {
        if (!kubber.GetComponent<MonsterBase>().spawnByPlayer)
        {

            kubber.gameObject.SetActive(true);
            kubber.GetComponent<Collider>().enabled = true;
            kubber.GetComponent<NavMeshAgent>().enabled = true;
            kubber.transform.localScale = Vector3.Lerp(kubber.transform.localScale, Vector3.one, 8 * Time.deltaTime);


            if (kubber.transform.localScale == Vector3.one)
            {
                sizing_ = false;
                kubber.GetComponent<MonsterBase>().spawnByPlayer = true;
            }
        }

        else
        {
            kubber.GetComponent<Collider>().enabled = false;
            kubber.GetComponent<NavMeshAgent>().enabled = false;
            kubber.transform.localScale = Vector3.Lerp(kubber.transform.localScale, Vector3.zero, 8f * Time.deltaTime);
            kubber.transform.position = Vector3.Lerp(kubber.transform.position, hand.transform.position, 8f * Time.deltaTime);

            if (kubber.transform.localScale == Vector3.zero)
            {
                sizing_ = false;
                kubber.GetComponent<MonsterBase>().spawnByPlayer = false;
                kubber.gameObject.SetActive(false);
                cubeOnWorld_ = false;
            }

        }
    }
    #endregion
}
