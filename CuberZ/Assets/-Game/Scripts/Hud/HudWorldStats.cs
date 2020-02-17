using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudWorldStats : MonoBehaviour
{
    [Header("Variáveis para Setar")]
    [SerializeField] private Image lifeImage;
    [SerializeField] private Image staminaImage;

    #region Funções para todos os Kubberz
    public void HudUpdateLife(float life, float maxLife)
    {
        lifeImage.fillAmount = life / maxLife;
    }

    public void HudUpdateStamina(float stamina, float staminaMax)
    {
        staminaImage.fillAmount = stamina / staminaMax;
    }
    #endregion
}
