using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SubMenuAbstraction : MonoBehaviour
{
    public Button returnButton;

    public virtual Button GetReturnButton()
    {
        return returnButton;
    }
}
