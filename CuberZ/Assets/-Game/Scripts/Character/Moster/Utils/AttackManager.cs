using System.Collections;
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
        public float damagePerSecond;
        public float totalDamageTime;
        public float startDamageTime; //
        public float endDamageTime; // 
        public float attackAnimationTime; //

        public bool canMove;
        public bool canStartInStay;
        public bool isDamagePerSencond;
        public bool isProjectileAttack;

        public Lineage attackEffect;
        public Lineage[] attackTypes;
    }
    
    public string GetAttackName(int index) { return attackStats[index].attackName; }

    public int GetBaseDamage(int index) { return attackStats[index].baseDamage; }
    public int GetStaminaCost(int index) { return attackStats[index].staminaCost; }

    public float GetAttackCoolDown(int index) { return attackStats[index].attackCoolDown; }
    public float GetDamagePerSecond(int index) { return attackStats[index].damagePerSecond; }
    public float GetTotalDamageTime(int index) { return attackStats[index].totalDamageTime; }
    public float GetStartDamageTime(int index) { return attackStats[index].startDamageTime; }
    public float GetEndDamageTime(int index) { return attackStats[index].endDamageTime; }
    public float GetAttackAnimationTime(int index) { return attackStats[index].attackAnimationTime; }

    public bool GetCanMove(int index) { return attackStats[index].canMove; }
    public bool GetCanStartInStay(int index) { return attackStats[index].canStartInStay; }
    public bool GetIsDamagePerSecond(int index) { return attackStats[index].isDamagePerSencond; }
    public bool GetIsTotalDamageTime(int index) { return attackStats[index].isProjectileAttack; }

    public Lineage GetAttackEffect(int index) { return attackStats[index].attackEffect; }
    public Lineage[] GetAttackTypes(int index) { return attackStats[index].attackTypes; }

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
            
            attackStats[i].attackEffect = Lineage.Lava;
        }
        // Armazena o attackStats em uma variavel do HudSystem.
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
