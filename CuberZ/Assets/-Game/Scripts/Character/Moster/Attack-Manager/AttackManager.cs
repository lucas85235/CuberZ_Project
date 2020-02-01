﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public bool kubberPlayer;
    // Esconder do Inspector quando não estiver usando para teste
    public int[] attackTier = { -1, -1, -1, -1};

    // setar manualmente no inspector
    public AttackStats[] attackStats;

    private void Start() 
    {
        //SetAttackNamesInStats();
        //SetRandomStats();
        //SetRandomAttackTier(attackTier.Length);

        if (HUDKubberAttacks.instance != null)
            HUDKubberAttacks.instance.HudUpdateSkillAttackTier(kubberPlayer, attackTier);  
    }

    public enum Lineage
    {
        Lava,
        Original
    }

    public enum AttackType 
    {
        Physical,
        LongDistance,
        InArea,
        ChangeStatus,
        Relative
    }

    public void SetRandomAttackTier(int range)
    {
        bool exitLoop = false;
        int i = 0;

        while (!exitLoop)
        {
            if (i >= attackTier.Length)
            {
                //Armazena o attackTier em uma variavel do HudSystem.
                if (HUDKubberAttacks.instance != null)
                    HUDKubberAttacks.instance.HudUpdateSkillAttackTier(kubberPlayer, attackTier);  
                return;
            }

            int random = Random.Range(0, range);
            bool diferentNumber = true;

            for (int j = 0; j < attackTier.Length; j++)
            {
                if (random == attackTier[j])
                    diferentNumber = false;
            }

            if (diferentNumber)
            {
                attackTier[i] = random;  
                i++;  
            }
        }
    }

    #region Estrutura de dados dos attacks
    [System.Serializable]
    public struct AttackStats
    {
        public string attackName;
        public int baseDamage;
        public int staminaCost;
        public float attackCoolDown;
        public bool canMove;
        public Lineage[] attackEffect;
        //public AttackType attackType;
    }
    
    public string GetAttackName(int index)
    {
        string name = attackStats[index].attackName;
        return name;
    }

    public int GetBaseDamage(int index)
    {
        int baseDamage = attackStats[index].baseDamage;
        return baseDamage;
    }

    public int GetStaminaCost(int index)
    {
        int staminaCost = attackStats[index].staminaCost;
        return staminaCost;
    }

    public float GetAttackCoolDown(int index)
    {
        float coolDown = attackStats[index].attackCoolDown;
        return coolDown;
    }

    public bool GetCanMove(int index)
    {
        bool can = attackStats[index].canMove;
        return can;
    }

    public Lineage[] GetAttackEffects(int index)
    {
        Lineage[] types = attackStats[index].attackEffect;
        return types;
    }

    /*public AttackType GetAttackType(int index)
    {
        AttackType type = attackStats[index].attackType;
        return type;
    }*/

    public void SetAttackNamesInStats(System.Enum getName, int statsIndex)
    {
        attackStats[statsIndex].attackName = getName.ToString();
    }

    public void SetRandomStats()
    {
        for (int i = 0; i <= (int)DefaultLavaAttacks.VolcanicAttack; i++) // <=  ->   <
        {
            Debug.Log(i);
            attackStats[i].baseDamage = Random.Range(10, 25);
            attackStats[i].staminaCost = Random.Range(5, 15);
            attackStats[i].attackCoolDown = Random.Range(0f, 3f);

            int random = Random.Range(0, 2);
            attackStats[i].canMove = random == 1 ? true : false;
            
            attackStats[i].attackEffect = new Lineage[1];
            attackStats[i].attackEffect[0] = Lineage.Lava;
        }
        //Armazena o attackStats em uma variavel do HudSystem.
    }   
    #endregion

    #region Ataques padrão por tipo
    public enum DefaultLavaAttacks
    {
        FlameOfFire,
        FireBall,
        FireVortex,
        SpitsFire,
        FireBearing,
        FirePunch,
        OnslaughtOfFire,
        LavaRain,
        LivingFire,
        FireRay,
        VolcanicAttack
    }

    public enum DefaultOriginalAttacks
    {
        Scratch, // arranhão
        ToHeadButt, // cabeçada
        Oxtail, // rabada
        Bite, // mordida
        Onslaught, // investida(só funciona na terra)
        Kick, // chute(só funciona na terra)
        Punch, // Soco
        RotaryAttack, // Ataque Giratório
        Meditation, // meditação
        Horned // chifrada(relativo)
    }
    #endregion
}
