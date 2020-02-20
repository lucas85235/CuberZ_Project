using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDKubberTeam : MonoBehaviour
{
    #region Variáveis Públicas

    [Header("Variáveis Alocáveis")]
    //Holder dos slots
    public Transform teamParent;
    //Slots em si
    public RectTransform[] slots;
    //Ícones dentro dos slots
    public Image[] icons;

    [Header("Tempo para os ícones se movimentarem")]
    public float iconsMovingTime = 0.5f;

    #endregion

    #region Variáveis Privadas

    //Identifica se algum ícone está sendo arrastado
    private bool dragging_ = false;

    //Controle primário do movimentamento dos ícones
    private bool iconMove_ = false;
    //Posições do movimento primário, 0 = atual, 1 = destino
    private Transform[] primaryMovement_ = new Transform[2];

    //Controle para movimentação secundária dos ícones
    private bool secondaryIconMove_ = false;
    //Posições do movimento secundário, 0 = atual, 1 = destino
    private Transform[] secondaryMovement_ = new Transform[2];

    //Valor do lerp de movimentação
    private float movementLerp_ = 0;

    #endregion

    #region Variáveis Holder

    //Imagem do ícone que está sendo arrastado
    private Image draggedIcon_;
    //Antigo index do ícone que está sendo arrastado
    private int draggedIconIndex_ = -1;

    #endregion

    #region Funções MonoBehaviour

    //Fluxo Lógico:
    //Se os ícones estão se movendo ele trava todas as outras funções, até que o movimento acabe
    //se não estão mais se movendo as outras funções são chamadas pelos comandos.
    //O drag é iniciado quando aperta o botão esquerdo do mouse em cima de um slot.
    //Durante o drag o ícone segue o posicionamento do mouse.
    //O drag é finalizado quando o botão esquerdo do mouse é soltado,
    //nesse caso, se o comando foi realizado em cima de um outro slot os ícones se movimentam e trocam de lugar,
    //se foi em cima do antigo slot ou em qualquer outro ponto ta tela, o ícone que estava sendo arrastado simplesmente retorna ao slot inicial.
    private void Update()
    {
        if (iconMove_)
        {
            MoveIcons();
        }
        else
        {
            if (dragging_)
            {
                PointerDrag();

                if (Input.GetMouseButtonUp(0))
                {
                    PointerUp();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                PointerDown();
            }
        }
    }

    #endregion

    #region Funcão de Movimento

    //Movimenta suavemente os ícones com base no tempo predeterminado.
    //Faz um Lerp entre as posições atuais e as de destino de um item (ou de dois caso a ação secundária esteja ativada).
    //Ao finalizar a movimentação, atribui false às variáveis de controle de movimento e limpa os transforms de movimentação
    private void MoveIcons()
    {
        movementLerp_ += Time.deltaTime / iconsMovingTime;

        if (movementLerp_ < 1)
        {
            primaryMovement_[0].position = Vector3.Lerp(primaryMovement_[0].position, primaryMovement_[1].position, movementLerp_);

            if (secondaryIconMove_)
            {
                secondaryMovement_[0].position = Vector3.Lerp(secondaryMovement_[0].position, secondaryMovement_[1].position, movementLerp_);
            }
        }
        else
        {
            movementLerp_ = 0;

            primaryMovement_[0].position = primaryMovement_[1].position;
            primaryMovement_[0].SetParent(primaryMovement_[1]);

            if (secondaryIconMove_)
            {
                secondaryMovement_[0].position = secondaryMovement_[1].position;
                secondaryMovement_[0].SetParent(secondaryMovement_[1]);
            }

            iconMove_ = false;
            secondaryIconMove_ = false;
            primaryMovement_ = new Transform[2];
            secondaryMovement_ = new Transform[2];
        }
    }

    #endregion

    #region Função de retorno dos slots

    //Checa se o ponteiro está sobre algum slot.
    //Caso sim, retorna o index do slot,
    //caso não, retorna um valor negativo.
    private int ArePointerOnAnySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slots[i], Input.mousePosition)) return i;
        }

        return -1;
    }

    #endregion

    #region Funções do Mouse

    //Configura tudo necessário para iniciar o Drag
    //Atribui os valores no Holder e configura o ícone atual para ser movimentado
    private void StartDragging(int index)
    {
        draggedIconIndex_ = index;
        dragging_ = true;

        draggedIcon_ = icons[index];
        icons[index] = null;

        MoveThis(draggedIcon_.transform);
    }

    //Quando um ícone for se mover
    //altera o pai dele para o teamParent (holder dos slots) e aloca como último filho
    //isso serve para que o ícone não se mova por debaixo de nenhum slot holder
    private void MoveThis(Transform transform_)
    {
        transform_.SetParent(teamParent);
        transform_.SetAsLastSibling();
    }

    //Limpa o Holder
    //Esvazia todas as informações "seguradas"
    private void ClearHolder()
    {
        draggedIcon_ = null;
        draggedIconIndex_ = -1;
        dragging_ = false;
    }

    //Arrasta o ícone com o ponteiro
    //Posição do ícone é atribuída como a mesma do mouse
    private void PointerDrag()
    {
        draggedIcon_.transform.position = Input.mousePosition;
    }

    //Ao botão esquerdo do mouse ser levantado enquanto o drag está sendo executado
    //Chama a função para checar se o ponteiro está sobre algum slot e determina o transform do movimento primário como o do ícone que está sendo arrastado
    //Caso não esteja ou esteja sobre o slot inicial do ícone que está sendo arrastado:
    //Configura o transform do destino do movimento primário como o slot inicial do ícone arrastado e retorna o ícone para sua posição anterior da array
    //Caso esteja sobre um slot novo:
    //Configura o transform do destino do movimento primário como o slot atual
    //Configura o movimento secundário com o transform do ícone que ocupa aquele slot e seu destino sendo o antigo slot do ícone arrastado
    //Configura o ícone do movimento secundário para ser movido
    //Troca as informações da array
    //E configura o movimento secundário como verdadeiro
    //Ao final, o movimento primário é configurado como verdadeiro
    //E o holder é esvaziado.
    private void PointerUp()
    {
        int currentSlot = ArePointerOnAnySlot();

        primaryMovement_[0] = draggedIcon_.transform;

        if (currentSlot < 0 || currentSlot == draggedIconIndex_)
        {
            primaryMovement_[1] = slots[draggedIconIndex_] as Transform;

            icons[draggedIconIndex_] = draggedIcon_;
        }
        else
        {
            primaryMovement_[1] = slots[currentSlot];

            secondaryMovement_[0] = icons[currentSlot].transform;
            secondaryMovement_[1] = slots[draggedIconIndex_] as Transform;

            MoveThis(icons[currentSlot].transform);

            icons[draggedIconIndex_] = icons[currentSlot];
            icons[currentSlot] = draggedIcon_;

            secondaryIconMove_ = true;
        }

        iconMove_ = true;

        ClearHolder();
    }

    //Ao botão esquerdo do mouse ser pressionado enquanto o drag não está sendo executado
    //Chama a função para determinar se o cursor está em cima de algum slot
    //Se a função retornar um valor válido, chama a função para configurar o drag
    private void PointerDown()
    {
        int currentSlot = ArePointerOnAnySlot();
        if (currentSlot >= 0) StartDragging(currentSlot);
    }

    #endregion
}
