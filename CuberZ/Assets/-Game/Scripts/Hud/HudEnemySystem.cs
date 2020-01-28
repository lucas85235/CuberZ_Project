using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudEnemySystem : MonoBehaviour
{
    [Header("Variáveis para Setar")]
    public Image vidaImage;
    public Image vidaStamina;


    #region Singleton
    private static HudEnemySystem instance_;
    public static HudEnemySystem instance { get { return instance_; } }
    private void Awake()
    {
        instance_ = this;
    }
    #endregion



    #region Funções para todos os Kubberz

    public void HudUpdateVida(Transform target_, float life_, float maxLife_)
    {
        target_.GetComponent<HudEnemySystem>().vidaImage.fillAmount = life_ / maxLife_;
    }

    public void HudUpdateStamina(Transform target_, float stamina_, float staminaMax_)
    {
        target_.GetComponent<HudEnemySystem>().vidaStamina.fillAmount = stamina_ / staminaMax_;
    }

    #endregion



   
}
