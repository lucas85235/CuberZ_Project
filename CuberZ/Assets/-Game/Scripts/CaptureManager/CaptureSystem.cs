using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSystem : MonoBehaviour
{
    [Header("Variaveis de Alocação")]
    public GameObject captureCube_;
    public Transform hand_;
    public LayerMask layermask_;

    [Header("Variaveis de Captura")]
    public float impulseForce_;
    public float impulseY_;
    public float gravityIntensity_;

    [Header("Variaveis de Feedback")]
    public int cuboQuantidade_ = 10;
    [Range(-50, 50)]
    public float MouseZ = 10;

    //Variaveis Privadas
    private GameObject captureCubeTemp_;
    private bool capturing_;
    private Vector3 raycastPosition_;
    Vector3 hitPointV3_;
    float xv3_, yv3_, zv3_;
    float distance_;

    //Singleton
    public static CaptureSystem instance_;
    private CaptureSystem Instance_ { get { return instance_; } }



    private void Awake()
    {
        instance_ = this;
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
        if (cuboQuantidade_ > 0)
        {
            captureCubeTemp_.transform.SetParent(null);
            captureCubeTemp_.GetComponent<Rigidbody>().useGravity = true;
            captureCubeTemp_.GetComponent<Rigidbody>().AddForce((-new Vector3(CubeDirection().x, -CubeDirection().y, CubeDirection().z)
                * (distance_ / 0.1334f)) + new Vector3(0, impulseY_, 0), ForceMode.Impulse);
            captureCubeTemp_ = null;
            cuboQuantidade_--;

            if (cuboQuantidade_ > 0) CaptureInstantiate();
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

    protected void CaptureInstantiate() //Função para instanciar o Cubo que estará selecionado (Não tem seleção ainda).
    {
        if (!captureCubeTemp_)
        {
            captureCubeTemp_ = Pooling.InstantiatePooling(captureCube_, hand_.position, Quaternion.identity);
            captureCubeTemp_.transform.SetParent(hand_.transform);
            captureCubeTemp_.GetComponent<CaptureCube>().gravityImpact_ = gravityIntensity_;
            captureCube_.GetComponent<Rigidbody>().useGravity = false;
            captureCube_.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    protected Vector3 MiraCube()
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
        distance_ = Vector3.Distance(FinalPos, hand_.transform.position);
        GameObject t = Pooling.InstantiatePooling(captureCube_, FinalPos, Quaternion.identity);
        return FinalPos;
    }


    protected Vector3 CubeDirection()
    {
        Vector3 v1_ = MiraCube();
        Vector3 v2_ = captureCubeTemp_.transform.position;
        Vector3 dirFinal_ = ((v2_ - v1_).normalized);
        return dirFinal_;

    }


    #endregion
}
