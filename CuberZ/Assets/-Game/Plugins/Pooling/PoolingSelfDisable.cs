using UnityEngine;

public class PoolingSelfDisable : MonoBehaviour
{
    public void PoolingSelfDisableSetTimer(float Timer)
    {
        if (Timer > 0) Invoke("SelfDisable", Timer);
    }

    private void SelfDisable()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
        GetComponent<PoolingSelfDisable>().enabled = false;
    }
}
