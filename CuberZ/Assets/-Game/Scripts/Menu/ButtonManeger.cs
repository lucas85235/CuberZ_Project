using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private float initialSpaceButtons;
    private float spaceBetweenButtons;

    private GameObject currentButton_;
    private Vector3 buttonPosition_;
    private Transform parentPosition_;

    private string buttonText_;

    public void CreateButton(GameObject button)
    {
        GameObject buttonObject = button;
        buttonObject.transform.GetChild(0).GetComponent<Text>().
            text = buttonText_;

        currentButton_ = Instantiate(buttonObject, parentPosition_.position + buttonPosition_,
            Quaternion.identity, parentPosition_);
    }

    public void SetSpaceBetweenButtons(float space, float initialSpace)
    {
        spaceBetweenButtons = space;
        initialSpaceButtons = initialSpace;
    }

    public GameObject GetCurrentButton()
    {
        return currentButton_;
    }

    public void SetButton(string buttonText, int countButton)
    {
        buttonText_ = buttonText;

        switch (countButton)
        {
            case 1: buttonPosition_ = new Vector3(0, initialSpaceButtons, 0); break;
            case 2: buttonPosition_ = new Vector3(0, initialSpaceButtons * -1, 0); break;

            case 3: buttonPosition_ = new Vector3(0, initialSpaceButtons + spaceBetweenButtons, 0); break;
            case 4: buttonPosition_ = new Vector3(0, (initialSpaceButtons + spaceBetweenButtons) * -1, 0); break;

            case 5: buttonPosition_ = new Vector3(0, initialSpaceButtons + spaceBetweenButtons * 2, 0); break;
            case 6: buttonPosition_ = new Vector3(0, (initialSpaceButtons + spaceBetweenButtons * 2) * -1, 0); break;

            case 7: buttonPosition_ = new Vector3(0, initialSpaceButtons + spaceBetweenButtons * 3, 0); break;
            case 8: buttonPosition_ = new Vector3(0, (initialSpaceButtons + spaceBetweenButtons * 3) * -1, 0); break;
        }
    }

    public void SetParentPosition(Transform parent)
    {
        parentPosition_ = parent;
    }
}
