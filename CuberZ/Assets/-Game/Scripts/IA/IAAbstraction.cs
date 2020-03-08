using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAAbstraction : MonoBehaviour
{
    [Header("Stamina Stats")]
    [SerializeField] protected float staminaRegen = 0.03f;
    [SerializeField] protected float regenStartTime = 1.0f;
    [SerializeField] protected float monsterStamina;

    protected float maxStamina = 100f;
    
    private float countRegenStartTime = 0;
    private bool startRegenProcess = false;

    [Header("Stamina Stats")]
    [SerializeField] protected float monsterLife;
    protected float maxLife = 100f;
    protected bool isDead;

    protected abstract void ControlWalkState();
    protected abstract void ControlBatlleState();

    public abstract void IncrementLife(float increment);
    public abstract void DecrementLife(float decrement);

    public abstract void IncrementStamina(float increment);
    public abstract void DecrementStamina(float decrement);

    protected virtual void RegenStamina() 
    {
        if (monsterStamina < maxStamina)
        {
            if (!startRegenProcess)
            {
                startRegenProcess = true;
                countRegenStartTime = 0;
            }
            else countRegenStartTime += Time.deltaTime;

            if (countRegenStartTime >= regenStartTime)
                IncrementStamina(staminaRegen);
        }
    }

    public enum State
    {
        Walk,
        Batlle,
    }
}
