﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDKubberAttacks : MonoBehaviour
{
    public Text[] skillName, staminaCost, skillBaseDamage, attackCoolDown;
    public GameObject[] selector_;
    private IInput input_;
    private AttackManager.AttackStats[] skillSetHolder_;
    private AttackManager.AttackStats selectedAttack_;

    private int[] attackTierHolder_;
    private bool getContent_;

    #region Singleton
    private static HUDKubberAttacks instance_;
    public static HUDKubberAttacks instance { get { return instance_; } }

    protected virtual void Construt(IInput newInputInterface)
    {
        input_ = newInputInterface;
    }

    private void Awake()
    {
        instance_ = this;

        Construt(FindObjectOfType<DesktopInputImpl>());
    }
    #endregion

    private void Update()
    {
        WhichSkillIsSelected();

        if (Input.GetKeyDown(KeyCode.E))
        {
            HudUpdateAll(true);
        }

        if (input_.ExecuteAction())
        {
            CastSkill();
        }
    }

    #region Funções para o Kubber do Player selecionado 
    public void HudUpdateSkillAttackStats(bool kubberPlayer_, AttackManager.AttackStats[] attackstats_)
    {
        skillSetHolder_ = attackstats_;
    }

    public void HudUpdateSkillAttackTier(bool kubberPlayer_, int[] attackTier_)
    {
        attackTierHolder_ = attackTier_;
    }

    public void HudUpdateAll(bool kubberPlayer_) // Atualiza todos os dados para o HUD (SÓ DEVE SER CHAMADO PELO MONSTRO DO JOGADOR)
    {
        if (kubberPlayer_)
        {
            for (int i = 0; i < attackTierHolder_.Length; i++)
            {
                //O nome da Skill deve estar preenchido para ser lido por esse script

                if (i < skillSetHolder_.Length)
                {
                    skillName[i].text = "Nome: " + skillSetHolder_[i].attackName;
                    staminaCost[i].text = "Custo: " + skillSetHolder_[i].staminaCost.ToString();
                    skillBaseDamage[i].text = "Dano: " + skillSetHolder_[i].baseDamage.ToString();
                    attackCoolDown[i].text = "CoolDown: " + ((int)skillSetHolder_[i].attackCoolDown).ToString();
                }
                else
                {
                    skillName[i].text = "Nome: " + "Valor não alocado";
                    staminaCost[i].text = "Custo: " + "Valor não alocado";
                    skillBaseDamage[i].text = "Dano: " + "Valor não alocado";
                    attackCoolDown[i].text = "CoolDown: " + "Valor não alocado";
                }
            }
        }
    }

    public void WhichSkillIsSelected()
    {
        if (input_.KubberAttack1())
        {
            if (skillSetHolder_.Length > 0)
            {
                selectedAttack_ = skillSetHolder_[0];
                ActiveOne(0);
                getContent_ = true;
            }
        }
        else if (input_.KubberAttack2())
        {
            if (skillSetHolder_.Length > 1)
            {
                selectedAttack_ = skillSetHolder_[1];
                ActiveOne(1);
                getContent_ = true;
            }
        }  
        else if (input_.KubberAttack3())
        {
            if (skillSetHolder_.Length > 2)
            {
                selectedAttack_ = skillSetHolder_[2];
                ActiveOne(2);
                getContent_ = true;
            }
        }
        else if (input_.KubberAttack4())
        {
            if (skillSetHolder_.Length > 3)
            {
                selectedAttack_ = skillSetHolder_[3];
                ActiveOne(3);
                getContent_ = true;
            }
        }
    }

    public void CastSkill()
    {
        if(getContent_) Debug.Log(selectedAttack_.attackName + " foi castada.");
        getContent_ = false;
    }

    public void ActiveOne(int index)
    {
        for (int i = 0; i < selector_.Length; i++)
        {
            if (i != index) 
                selector_[i].SetActive(false);
            else 
                selector_[i].SetActive(true);
        }
    }
    #endregion
}
