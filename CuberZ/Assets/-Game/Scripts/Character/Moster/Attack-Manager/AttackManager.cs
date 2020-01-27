using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    // Esconder do Inspector quando não estiver usando para teste
    public int[] attackTier = { -1, -1, -1, -1 };

    // setar manualmente no inspector
    public AttackStats[] attackStats;

    private void Start() 
    {
        //SetAttackNamesInStats();
        //SetRandomStats();      
    }

    public enum Lineage
    {
        Lava,
        Original
    }

    public void SetRandomAttackTier(int range)
    {
        bool exitLoop = false;
        int i = 0;

        while (!exitLoop)
        {
            if (i >= attackTier.Length)
                return;

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
        public Lineage[] attackType;
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

    public Lineage[] GetAttackType(int index)
    {
        Lineage[] types = attackStats[index].attackType;
        return types;
    }

    public void SetAttackNamesInStats(System.Enum getName, int statsIndex)
    {
        attackStats[statsIndex].attackName = getName.ToString();
    }

    public void SetRandomStats()
    {
        for (int i = 0; i <= (int)DefaultLavaAttacks.VolcanicAttack; i++)
        {
            attackStats[i].baseDamage = Random.Range(10, 25);
            attackStats[i].staminaCost = Random.Range(5, 15);
            attackStats[i].attackCoolDown = Random.Range(0f, 3f);

            int random = Random.Range(0, 2);
            attackStats[i].canMove = random == 1 ? true : false;
            
            attackStats[i].attackType = new Lineage[1];
            attackStats[i].attackType[0] = Lineage.Lava;
        }
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
