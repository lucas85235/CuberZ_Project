using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KubberzInventory : MonoBehaviour
{
    #region Variáveis e Setups

    [SerializeField] private GameObject[] inventorySlots;
    [SerializeField] private int inventoryCapacity = 25;

    private void Awake()
    {
        inventorySlots = new GameObject[inventoryCapacity];
        //Load Kubberz do inventário
    }

    //Retorna verdadeiro caso o index seja inválido
    private bool InvalidIndex(int index) => index < 0 || index >= inventoryCapacity;

    #endregion

    #region Funções do inventário

    //Adiciona um kubber no primeiro slot vazio que tiver
    public void AddKubberInNextEmptySlot(GameObject kubber)
    {
        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = kubber;
                return;
            }
        }

        //Armanezamento cheio
    }

    //Tenta adicionar um Kubber em um slot específico
    public void AddKubberInIndex(GameObject kubber, int index)
    {
        if (InvalidIndex(index)) return;

        if (inventorySlots[index] == null)
        {
            inventorySlots[index] = kubber;
            return;
        }
        else
        {
            //Slot Ocupado: retornar ou trocar o kubber do slot atual com o handler
        }
    }

    //Pega um Kubber de um slot específico
    public GameObject TakeKubberInIndex(int index)
    {
        if (InvalidIndex(index)) return null;

        if (inventorySlots[index] == null)
        {
            //Slot vazio, não retorna nada
            return null;
        }
        else
        {
            GameObject kubber = inventorySlots[index];
            inventorySlots[index] = null;

            return kubber;
        }
    }

    //Remove pernamentemente um Kubber de um slot
    public void PermanentlyRemoveKubberInIndex(int index)
    {
        if (InvalidIndex(index)) return;

        inventorySlots[index] = null;
    }

    #endregion
}
