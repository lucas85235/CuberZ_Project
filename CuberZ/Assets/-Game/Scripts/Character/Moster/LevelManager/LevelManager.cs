using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Variáveis de Nível e Experiência
    private const int maxLevel_ = 50;

    [Header("Configurações de Nível e Experiência.")]
    [Tooltip("Nível atual do Kubber")]
    [Range(1, 50)]
    public int currentLevel = 1;

    [Tooltip("Experiência base necessária para o Kubber passar de nível.")]
    public float baseExp;
    private float currentExp_ = 0;
    private float toNextLevelExp_;

    private int upgradeSkillPoints_ = 0;
    #endregion

    #region Variáveis de Stats
    //Cada Stat precisa de 3 Variáveis, uma com o valor base do Stat, uma para o limite máximo do Stat e uma para o stat atual do kubber

    [Header("HP")]
    [Header("Configurações De Stats")]
    public int baseHP;
    public int maxHP;
    private int currentHP_;

    [Header("Speed")]
    public int baseSpeed;
    public int maxSpeed;
    private int currentSpeed_;

    [Header("Stamina")]
    public int baseStamina;
    public int maxStamina;
    private int currentStamina_;

    [Header("Melee Attack")]
    public int baseMeleeAttack;
    public int maxMeleeAttack;
    private int currentMeleeAttack_;

    [Header("Ranged Attack")]
    public int baseRangedAttack;
    public int maxRangedAttack;
    private int currentRangedAttack_;

    [Header("Melee Defense")]
    public int baseMeleeDefense;
    public int maxMeleeDefense;
    private int currentMeleeDefense_;

    [Header("Ranged Defense")]
    public int baseRangedDefense;
    public int maxRangedDefense;
    private int currentRangedDefense_;
    #endregion

    #region Funções da Unity
    private void Start()
    {
        //
        /* Modificar isso quando houver um sistema de save para salvar os Stats e/ou qunado tiver um sistema de cálculo aleatório de Stats baseado no nível do Kubber, para encontros selvagens */
        //

        #region Level and Exp Setters
        //Quando tiver sistema de save, recuperar o nível atual, a Exp atual e obter a Experiência necessária para o próximo nível
        //Caso seja um Kubber selvagem, um cálculo para determinar seu nível também entrará aqui
        GetToNextLevelExp_();
        #endregion

        #region Stats Setter
        //Se tiver um valor salvo para HP, pega o valor salvo, senão:
        currentHP_ = baseHP;
        //Se tiver um valor salvo para Speed, pega o valor salvo, senão:
        currentSpeed_ = baseSpeed;
        //Se tiver um valor salvo para Stamina, pega o valor salvo, senão:
        currentStamina_ = baseStamina;
        //Se tiver um valor salvo para Melee Attack, pega o valor salvo, senão:
        currentMeleeAttack_ = baseMeleeAttack;
        //Se tiver um valor salvo para Ranged Attack, pega o valor salvo, senão:
        currentRangedAttack_ = baseRangedAttack;
        //Se tiver um valor salvo para Melee Defense, pega o valor salvo, senão:
        currentMeleeDefense_ = baseMeleeDefense;
        //Se tiver um valor salvo para Ranged Defense, pega o valor salvo, senão:
        currentRangedDefense_ = baseMeleeDefense;
        #endregion

    }
    #endregion

    #region Stats Getters
    //Métodos públicos para pegar o valor de cada stat do Kubber
    public int GetHP() { return currentHP_; }
    public int GetSpeed() { return currentSpeed_; }
    public int GetStamina() { return currentStamina_; }
    public int GetMeleeAttack() { return currentMeleeAttack_; }
    public int GetRangedAttack() { return currentRangedAttack_; }
    public int GetMeleeDefense() { return currentMeleeDefense_; }
    public int GetRangedDefense() { return currentRangedDefense_; }
    #endregion

    #region Upgrade Stats
    //Função usada para facilitar o Upgrade de skills
    //Após receber o stat a ser melhorado e o valor máximo do stat, verifica se a quantidade de Upgrade Skill Points é maior que zero, assim como se o stat atual é menor que o máximo
    //Caso tudo esteja certo ela decrementa o Upgrade Skill Points e retorna o valor do stat + 1, caso não, retorna apenas o valor do stat
    private int UpgradeSkill(int currentSkillValue, int maxSkillValue)
    {
        if (upgradeSkillPoints_ > 0 && currentSkillValue < maxSkillValue)
        {
            upgradeSkillPoints_--;
            return ++currentSkillValue;
        }
        else
        {
            return currentSkillValue;
        }
    }

    //Funções para melhoramento de Stats ao subir de nível
    public void UpgradeHP() { currentHP_ = UpgradeSkill(currentHP_, maxHP); }
    public void UpgradeSpeed() { currentSpeed_ = UpgradeSkill(currentSpeed_, maxSpeed); }
    public void UpgradeStamina() { currentStamina_ = UpgradeSkill(currentStamina_, maxStamina); }
    public void UpgradeMeleeAttack() { currentMeleeAttack_ = UpgradeSkill(currentMeleeAttack_, maxMeleeAttack); }
    public void UpgradeRangedAttack() { currentRangedAttack_ = UpgradeSkill(currentRangedAttack_, maxRangedAttack); }
    public void UpgradeMeleeDefense() { currentMeleeDefense_ = UpgradeSkill(currentMeleeDefense_, maxMeleeDefense); }
    public void UpgradeRangedDefense() { currentRangedDefense_ = UpgradeSkill(currentRangedDefense_, maxRangedDefense); }
    #endregion

    #region Funções de Controle de Experiência e de Nível
    /* A experiência necessária para o próximo nível é progressiva e pode ser calculada pela fórmula de juros compostos: M = C * (1 + i) ^ n
    Aqui, no caso: ExperiênciaNecessária = ExperiênciaBase * (1 + PorcentagemDeAumentoProgressivo) ^ (NívelAtal - 1) */
    private float GetToNextLevelExp_() { return toNextLevelExp_ = baseExp * Mathf.Pow(1 + 0.3f, currentLevel - 1); }

    //Experiência adquirida ao derrotar este Kubber, 25% da experiência necessária para o Kubber passar para o próximo nível
    public float GetKubberDefeatedExp() { return toNextLevelExp_ * 0.25f; }

    /* Experiência Ganha por este kubber
    Um kubber só pode receber experiência se seu nível for menor que o nível máximo.
    Ao receber experiência ela é atribuída à experiência atual do Kubber, caso a experiência atual seja maior ou igual à necessária para o kubber passar de nível,
    a função de passar de nível é chamada e é calculado se haverá experiência extra. Ao final, caso haja experiência extra a função é chamada novamente, passando a experiência extra ao parâmetro */
    public void SetKubberExp(float exp)
    {
        if (currentLevel < maxLevel_)
        {
            float expExtra = 0;
            currentExp_ += exp;

            if (currentExp_ >= toNextLevelExp_)
            {
                expExtra = currentExp_ - toNextLevelExp_;
                KubberToNextLevel();
            }

            if (expExtra > 0) SetKubberExp(expExtra);
        }
    }

    /* Função chamada para passar o Kubber para o próximo nível
    Um Kubber só passa de nível caso não tenha atingido nível máximo.
    Ao passar de nível sua experiência atual é zerada, o nível atual incrementado em um e o Kubber ganha dois pontos de Upgrade de Skill
    Também é atualizada a quantidade de experiência necessária para passar de nível. */
    public void KubberToNextLevel()
    {
        if (currentLevel < maxLevel_)
        {
            currentExp_ = 0;
            currentLevel++;
            upgradeSkillPoints_ += 2;

            GetToNextLevelExp_();
        }
    }
    #endregion
}
