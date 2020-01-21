using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSystem : MonoBehaviour
{
    [Header("Variaveis de Alocação")]
    public GameObject captureCube_;
    public Transform hand_;
    public GameObject captureCanvas_;

    public float ImpulseForce;

    //Variaveis Privadas
    private GameObject captureCubeTemp_;
    private bool capturing_;

    //Singleton
    public static CaptureSystem Instance_;
    private CaptureSystem instance_ { get { return Instance_; } }

    public void CaptureInstantiate() //Função para instanciar o Cubo que estará selecionado (Não tem seleção ainda).
    {
        if (!captureCubeTemp_)
        {
            captureCubeTemp_ = Pooling.InstantiatePooling(captureCube_, hand_.position, Quaternion.identity);
            captureCubeTemp_.transform.SetParent(hand_);
        }
    }

    public void ThrowCube()
    {
     


    }

    #region Entrada e Saída do Modo de Captura

    public void EnterCaptureMode() //Função para entrar no modo de captura
    {
        capturing_ = true;
        CaptureInstantiate();
        captureCanvas_.SetActive(true);

        //Ações Sobre o Player (Não pode pular, movimento lento)
    }

    public void ExitCaptureMode()
    {
        capturing_ = false;
        captureCubeTemp_.transform.SetParent(null);
        captureCubeTemp_.SetActive(false);
        captureCubeTemp_ = null;
        captureCanvas_.SetActive(false);

    }

    #endregion


}
