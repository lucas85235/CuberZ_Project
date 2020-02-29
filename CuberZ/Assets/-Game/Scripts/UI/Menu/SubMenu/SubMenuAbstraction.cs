using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SubMenuAbstraction : MonoBehaviour
{
    protected DataPresetOptions data;

    [Header("Abstract Components")]
    [SerializeField] protected Button applyButton;
    [SerializeField] protected Button returnButton;

    public virtual Button GetReturnButton()
    {
        return returnButton;
    }
}
