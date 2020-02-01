using System.Collections;
using System.Collections.Generic;

public interface IInput
{
    bool MoveCameraInput(); //Padrão- Botão Direito
    bool ExecuteActionInput(); //Padrão- Botão Esquerdo
    bool KubberAttack1Input(); //Padrão- Número 1
    bool KubberAttack2Input(); //Padrão- Número 2
    bool KubberAttack3Input(); //Padrão- Número 3
    bool KubberAttack4Input(); //Padrão- Número 4
    float GetAxisHorizontal(); //Padrão- (-1A | 1 D)
    float GetAxisVertical();   //Padrão- (-1S | 1 W)
    float GetAxisMouseX(); // Padrão- (-1A | 1 D)
    float GetAxisMouseY();   // Padrão- (-1S | 1 W)
    bool FixCameraOnMyKubberInput(); //Padrão- Tab
    bool RescueKubberInput(); //Padrão- Control
    bool RunInput(); //Padrão- Shift
    bool ExitInput(); //Padrão- Esc
    bool CaptureKubberInput(); //Padrão- Backspace
    bool JumpInput(); //Padrão- SpaceBar

}

