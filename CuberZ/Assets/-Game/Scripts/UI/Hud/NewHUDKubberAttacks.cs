using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHUDKubberAttacks : MonoBehaviour
{
    [Header("Variável de Alocação")]
    public HUDKubberAttackInfo[] hudInfo;

    #region Variáveis de Controle

    private CharacterAbstraction currentCharacter_;
    private MonsterBase currentKubber_;
    private AttackManager currentAttacks_;
    private bool hudControl_;

    #endregion

    #region Funções MonoBehaviour

    void Start()
    {
        ClearHUD();
        FindPlayerKubber();
    }

    void Update()
    {
        if (hudControl_)
            UpdateHUD();
        else
            ClearHUD();

        if (currentCharacter_ == null || !currentCharacter_.isEnabled)
            FindPlayerKubber();
    }

    #endregion

    #region Função para encontrar o CharacterAbstraction ativo

    private void FindPlayerKubber()
    {
        foreach (CharacterAbstraction character in FindObjectsOfType<CharacterAbstraction>())
        {
            if (character.isEnabled)
            {
                currentCharacter_ = character;

                if (character.GetComponent<MonsterBase>())
                {
                    currentKubber_ = character.GetComponent<MonsterBase>();
                    currentAttacks_ = character.GetComponent<AttackManager>();
                    hudControl_ = true;
                }
                else
                {
                    hudControl_ = false;
                }
            }
        }
    }

    #endregion

    #region Funções de controle da HUD

    //Limpa a HUD inteira
    private void ClearHUD()
    {
        for (int i = 0; i < hudInfo.Length; i++)
        {
            ClearHUDIndex(i);
        }
    }

    //Limpa apenas um determinado campo da HUD
    private void ClearHUDIndex(int index)
    {
        hudInfo[index].skillName.text = "";
        hudInfo[index].staminaCost.text = "";
        hudInfo[index].skillBaseDamage.text = "";
        hudInfo[index].attackCoolDown.text = "";
        hudInfo[index].selector.SetActive(false);
        hudInfo[index].gameObject.SetActive(false);
    }

    //Atualiza as informações da HUD
    private void UpdateHUD()
    {
        for (int i = 0; i < hudInfo.Length; i++)
        {
            if (currentAttacks_.attackTier[i] < 0)
                ClearHUDIndex(i);
            else
                SetHUDIndex(i);
        }
    }

    //Aloca as informações de determinado campo da HUD
    private void SetHUDIndex(int index)
    {
        int tier = currentAttacks_.attackTier[index];

        hudInfo[index].gameObject.SetActive(true);

        hudInfo[index].skillName.text = " Attack Name: " + currentAttacks_.attackStats[tier].attackName;
        hudInfo[index].staminaCost.text = " Stamina Cost: " + currentAttacks_.attackStats[tier].staminaCost;
        hudInfo[index].skillBaseDamage.text = " Base Damage: " + currentAttacks_.attackStats[tier].baseDamage;
        hudInfo[index].attackCoolDown.text = " Attack Cooldown: " + currentAttacks_.attackStats[tier].attackCoolDown;

        if (index == currentKubber_.currentAttackIndex)
            hudInfo[index].selector.SetActive(true);
        else
            hudInfo[index].selector.SetActive(false);
    }

    #endregion
}
