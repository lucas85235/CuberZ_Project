using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSystem : MonoBehaviour
{
    [Header("Variaveis de Alocação")]
    public GameObject captureCube;
    public Transform hand;
    private LayerMask layermask;

    [Header("Variaveis de Captura")]
    public float impulseForce = 20;
    public float impulseY = 10;
    [Range(0,100)]
    public float sucessPercentage;

    [Range(0, 10f)]
    public float distanceMultiplier_ = 1;

    [Header("Variaveis de Feedback")]
    public int cuboQuantidade = 1000;


    //Variaveis Privadas
    private IInput input_;
    private GameObject captureCubeTemp_;
    public bool capturing_;
    public bool captured_;
    public  bool capturingProcess_;
    private Vector3 hitPointV3_;
    private float xv3_, yv3_, zv3_;
    private float distance_;
    public bool throwbool;
    private Vector3 holdTarget_;

    //Singleton
    private static CaptureSystem instance_;
    public static CaptureSystem instance { get { return instance_; } }

    protected virtual void Construt(IInput newInputInterface) 
    {
        input_ = newInputInterface;
    }

    private void Awake()
    {
        instance_ = this;
        layermask = LayerMask.GetMask("Input");    

        Construt(Object.FindObjectOfType<InputSystem>());    
    }

    private void Update()
    {
        if (input_.CaptureKubberInput() && !capturing_ && !capturingProcess_ && cuboQuantidade > 0 && 
            !GetComponent<PlayerController>().jump && GetComponent<PlayerController>().canMove_) 
            EnterCaptureMode();

        else if (input_.CaptureKubberInput() && capturing_ && !capturingProcess_ && !captured_) 
            ExitCaptureMode();

        if (input_.ExecuteActionInput() && capturing_ && !capturingProcess_ && Vector3.Distance(transform.position,MiraCube()) > 15
            && Vector3.Distance(transform.position, MiraCube()) <= 80)
        {

            captureCubeTemp_.GetComponent<CaptureCube>().target = MiraCube();
            holdTarget_ = captureCubeTemp_.GetComponent<CaptureCube>().target;
            throwbool = true;
        }

        if (throwbool) ThrowProcess();

        if(captureCubeTemp_ && captureCubeTemp_.transform.parent)
        {
            captureCubeTemp_.transform.position = hand.transform.position;
        }

    }

    #region Funções usaveis/Publicas

    public void ThrowProcess()
    {
        capturingProcess_ = true;
        Vector3 tempV_ = new Vector3(holdTarget_.x,transform.position.y,holdTarget_.z) - transform.position;
        transform.forward = Vector3.Lerp(transform.forward, tempV_, 0.5f * Time.deltaTime);
        PlayerAnimation.instance.SetAnimatorAndAnimation(1, "throwfar");
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
            
            captureCubeTemp_.GetComponent<CaptureCube>().speed = impulseForce + (distance_ * distanceMultiplier_);

            captureCubeTemp_ = null;
            cuboQuantidade--;
            
          //  if (cuboQuantidade > 0) CaptureInstantiate();
          //  else Debug.Log("Você não possui mais cubos para jogar!");
        }

        else Debug.Log("Você não possui mais cubos para jogar!");
    }

    public void EnterCaptureMode() //Função para entrar no modo de captura
    {
        capturing_ = true;
        CaptureInstantiate();

        //Ações Sobre o Player (Não pode pular, movimento lento)
    }

    public void ExitCaptureMode()
    {
        capturing_ = false;
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
    #endregion

    #region Funções Protegidas/Privadas


    private Vector3 MiraCube()
    {
        RaycastHit hit;
        Ray mousePosition_ = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mouseposV3_ = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (Physics.Raycast(mousePosition_, out hit, Mathf.Infinity, layermask))
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

    private Vector3 CubeDirection()
    {
        Vector3 v1_ = MiraCube();
        Vector3 v2_ = captureCubeTemp_.transform.position;
        Vector3 dirFinal_ = ((v2_ - v1_).normalized);
        return dirFinal_;

    }
    #endregion
}
