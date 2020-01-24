using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCube : MonoBehaviour
{
    [Header("Variaveis de Alocação")]
    public GameObject fakeCube;
    public Transform[] allcubes;
    Rigidbody rigibody_;

    [HideInInspector]
    public Vector3 target { get { return target_; } set { target_ = value; } }
    private Vector3 target_;

    [HideInInspector]
    public float speed { get { return speed_; } set { speed_ = value; } }
    private float speed_;
    private bool moviment_;

    [HideInInspector]
    public float impulseY { get { return impulseY_; } set { impulseY_ = value; } }
    private float impulseY_;
    private bool break_;
    private bool capture_;
    private bool moveCapture_;
    private Vector3 point_;
    private Collider col2_;
    private Collider mycollider_;
    private Transform bigcube_, smallcube_;
    private Vector3 monsterPositionCap_;
    private Quaternion monsterRotationCap_;
    private Vector3 monsterScaleCap_;
    private bool afterCapture_;
    #region Funções MonoBehaviour de Execução

    private void Awake()
    {
        InitializingAwake();
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

    private void Update()
    {
        Abduction(col2_);
        AfterCaptureMonster();
        StopDistanceControl();
    }

    private void FixedUpdate()
    {
        PhysicsControl();
    }

    #endregion



    #region Funções de Inicialização 

    private void InitializingAwake()
    {
        moviment_ = true;
        rigibody_ = GetComponent<Rigidbody>();
        mycollider_ = GetComponent<Collider>();
        rigibody_.useGravity = false;
        rigibody_.velocity = Vector3.zero;
        bigcube_ = transform.GetChild(1);
        smallcube_ = transform.GetChild(0);
        smallcube_.localScale = Vector3.one;

    }

    private void InitializaingOnEnable()
    {
        rigibody_ = GetComponent<Rigidbody>();
        smallcube_.localScale = Vector3.one;
        smallcube_.gameObject.SetActive(true);
        bigcube_.gameObject.SetActive(false);
        rigibody_.useGravity = false;
        break_ = false;
        rigibody_.velocity = Vector3.zero;
        moviment_ = true;
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
        transform.forward = point_ - transform.position;
        Debug.Log("Toca Animação");
        bigcube_.GetComponent<Animator>().Play("Capturing", -1, 0);
        yield return new WaitUntil(() => bigcube_.GetComponent<CaptureHelper>().canGo);
        moveCapture_ = false;
        capture_ = true;
        col2_ = col;
        yield break;
    }

    void AfterCaptureMonster()
    {
        if (afterCapture_)
        {

            Quaternion tempRotation_ = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, tempRotation_, 5f * Time.deltaTime);

        }

    }


    private void Abduction(Collider coll)
    {
        if (capture_)
        {
            monsterPositionCap_ = coll.transform.position;
            monsterRotationCap_ = coll.transform.rotation;
            monsterScaleCap_ = coll.transform.localScale;

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
                mycollider_.isTrigger = false;
                afterCapture_ = true;
                //   

                //   bigcube_.GetComponent<Animator>().Play("DissolveCubo", -1, 0);
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
    } //Função que controla a interação Cubo/(Chão e Parede)

    private void OnTriggerStay(Collider col)
    {
        col2_ = col;

        if ((col.gameObject.name == "Ground" || col.gameObject.name == "Wall") && !break_ && !capture_)
        {
            if(!afterCapture_)  BreakCube();
            //else Programação de Mexer o cubo
        }

        else if (col.gameObject.tag == "Monster" && !capture_)
        {
            moviment_ = false;
            rigibody_.velocity = Vector3.zero;
            rigibody_.AddForce(-(col.transform.position - transform.position) * 7 + new Vector3(0, 35, 0), ForceMode.Impulse);
            rigibody_.AddTorque(Vector3.forward * -5, ForceMode.Impulse);
            StartCoroutine(StopCube(col));

        }
    }  //Verifica Onde o Cubo bateu

    #endregion
}
