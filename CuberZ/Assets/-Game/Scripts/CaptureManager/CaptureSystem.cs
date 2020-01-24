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
    private GameObject captureCubeTemp_;
    private bool capturing_;
    private Vector3 raycastPosition_;
    private Vector3 hitPointV3_;
    private float xv3_, yv3_, zv3_;
    private float distance_;

    //Singleton
    public static CaptureSystem instance;
    private CaptureSystem Instance_ { get { return instance; } }



    private void Awake()
    {
        instance = this;
        layermask = LayerMask.GetMask("Input");
        
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Backspace) && !capturing_) EnterCaptureMode();

        else if (Input.GetKeyDown(KeyCode.Backspace) && capturing_) ExitCaptureMode();


        if (capturing_ && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowCube();

        }
    }



    #region Funções usaveis/Publicas

    public void ThrowCube()
    {
        if (cuboQuantidade > 0)
        {
            captureCubeTemp_.transform.SetParent(null);
            captureCubeTemp_.transform.GetChild(0).GetComponent<Animator>().Play("DiminuirCuboPequeno", -1, 0);
            captureCubeTemp_.transform.GetChild(1).gameObject.SetActive(true);
            captureCubeTemp_.GetComponent<Rigidbody>().useGravity = true;
            captureCubeTemp_.GetComponent<CaptureCube>().target = MiraCube();
            captureCubeTemp_.GetComponent<CaptureCube>().speed = impulseForce + (distance_ * distanceMultiplier_);

            captureCubeTemp_ = null;
            cuboQuantidade--;

            if (cuboQuantidade > 0) CaptureInstantiate();
            else Debug.Log("Você não possui mais cubos para jogar!");
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

    #endregion



    #region Funções Protegidas/Privadas

    private void CaptureInstantiate() //Função para instanciar o Cubo que estará selecionado (Não tem seleção ainda).
    {
        if (!captureCubeTemp_)
        {
            captureCubeTemp_ = Pooling.InstantiatePooling(captureCube, hand.position, Quaternion.identity);
            captureCubeTemp_.transform.SetParent(hand.transform);
            captureCubeTemp_.GetComponent<CaptureCube>().impulseY = impulseY;
            captureCubeTemp_.GetComponent<CaptureCube>().sucessPercentage = sucessPercentage;
            captureCube.GetComponent<Rigidbody>().useGravity = false;
            captureCube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

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
