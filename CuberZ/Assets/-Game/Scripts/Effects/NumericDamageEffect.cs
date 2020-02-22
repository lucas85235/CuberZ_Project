using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumericDamageEffect : MonoBehaviour
{
    #region Variáveis "Públicas"

    [Header("Configurações do Efeito")]
    [Tooltip("Duração total do efeito.")]
    [SerializeField] private float effectDuration = 1f;

    [Range(0f, 1f)]
    [Tooltip("Em que momento do efeito o fade inicia?")]
    [SerializeField] private float fadeStartAt = 0.75f;

    [Min(1f)]
    [Tooltip("Quanto será o escalonamento do efeito?")]
    [SerializeField] private float scalingFactor = 1.5f;

    [Range(0f, 1f)]
    [Tooltip("Proporção de tempo de quando aumenta a escala e quando diminui.")]
    [SerializeField] private float scalingTimeProportion = 0.5f;
    [Tooltip("Velocidade em que o efeito se move para cima.")]
    [SerializeField] private float upSpeed = 3f;

    #endregion

    #region Variáveis Privadas

    //Texto cujo número aparecerá, e também onde todas as configurações do efeito serão aplicadas
    private Text damageEffect_;

    //Cor inicial do efeito
    private Color myColor_;
    //Cor do fade, igual à cor inicial, mas com o alfa transparente
    private Color fadeColor_;
    //Escala inicial do efeito
    private Vector3 myScale_;
    //Escala com o scalingFactor aplicado
    private Vector3 newScale_;

    //Timer da duração total do efeito
    private float timer_ = 0f;
    //Lerp usado no fade
    private float fadeLerp_ = 0f;
    //Lerp usado para aumentar a escala
    private float scaleUpLerp_ = 0f;
    //Lerp usado para diminuir a escala
    private float scaleDownLerp_ = 0f;

    //Transform da câmera, para não ficar usando Camera.main a cada frame
    private Transform mainCamera_;

    #endregion

    #region Função Pública para a Configuração do Efeito

    //Função a ser chamada para ativar o efeito:
    //Por motivos de evitar bugs, ela reseta todas as variáveis que precisam ser resetadas,
    //atribui o valor do dano ao texto do efeito
    //e, por fim, ativa o texto do efeito em si.
    public void SetUpEffect(int damageAmount)
    {
        timer_ = 0f;
        fadeLerp_ = 0f;
        scaleUpLerp_ = 0f;
        scaleDownLerp_ = 0f;

        newScale_ = myScale_ * scalingFactor;

        damageEffect_.transform.localPosition = Vector3.zero;
        damageEffect_.transform.localScale = myScale_;
        damageEffect_.color = myColor_;

        damageEffect_.text = damageAmount.ToString();

        damageEffect_.gameObject.SetActive(true);
    }

    #endregion

    #region Funções MonoBehaviour

    //Atribuição padrão de algumas variáveis
    private void Awake()
    {
        damageEffect_ = GetComponentInChildren<Text>();

        myScale_ = damageEffect_.transform.localScale;

        myColor_ = damageEffect_.color;
        fadeColor_ = new Color(myColor_.r, myColor_.g, myColor_.b, 0);

        damageEffect_.gameObject.SetActive(false);
        mainCamera_ = Camera.main.transform;
    }

    //Update só funcionará se damageEffect estiver ativado, ou seja, se a função SetUpEffect tiver sido chamada
    private void Update()
    {
        if (!damageEffect_.gameObject.activeInHierarchy) return;

        timer_ += Time.deltaTime;

        Scale();
        Fade();
        End();

        damageEffect_.transform.position += Vector3.up * upSpeed * Time.deltaTime;
        transform.rotation = mainCamera_.rotation;
    }

    #endregion

    #region Funções do Efeito

    //Atinge a nova escala e depois retorna para a anterior tendo como base a proporção de tempo configurada
    private void Scale()
    {
        if (timer_ < effectDuration * scalingTimeProportion)
        {
            scaleUpLerp_ += Time.deltaTime / (effectDuration * scalingTimeProportion);
            if (scaleUpLerp_ > 1) scaleUpLerp_ = 1;

            damageEffect_.transform.localScale = Vector3.Lerp(myScale_, newScale_, scaleUpLerp_);
        }
        else
        {
            scaleDownLerp_ += Time.deltaTime / (effectDuration - effectDuration * scalingTimeProportion);
            if (scaleDownLerp_ > 1) scaleDownLerp_ = 1;

            damageEffect_.transform.localScale = Vector3.Lerp(newScale_, myScale_, scaleDownLerp_);
        }
    }

    //Começa a desaparecer a partir do momento configurado na proporção do tempo para iniciar o fade
    private void Fade()
    {
        if (timer_ >= effectDuration * fadeStartAt)
        {
            fadeLerp_ += Time.deltaTime / (effectDuration - effectDuration * fadeStartAt);
            if (fadeLerp_ > 1) fadeLerp_ = 1;

            damageEffect_.color = Color.Lerp(myColor_, fadeColor_, fadeLerp_);
        }
    }

    //Após atingir o tempo de duração total, simplesmente desativa o damageEffect, isso faz com que o Update pare de ser executado
    private void End()
    {
        if (timer_ >= effectDuration)
        {
            timer_ = 0;
            damageEffect_.gameObject.SetActive(false);
        }
    }

    #endregion
}
