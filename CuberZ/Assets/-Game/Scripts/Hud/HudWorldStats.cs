using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudWorldStats : MonoBehaviour
{
    [Header("Variáveis para Setar")]
    public Image lifeImage;
    public Image staminaImage;

    #region Singleton
    private static HudWorldStats instance_;
    public static HudWorldStats instance { get { return instance_; } }
    
    private void Awake()
    {
        instance_ = this;
    }
    #endregion

    #region Funções para todos os Kubberz
    public void HudUpdateLife(Transform target, float life, float maxLife)
    {
        target.GetComponent<HudWorldStats>().lifeImage.fillAmount = life / maxLife;
    }

    public void HudUpdateStamina(Transform target, float stamina, float staminaMax)
    {
        target.GetComponent<HudWorldStats>().staminaImage.fillAmount = stamina / staminaMax;
    }
    #endregion
}
