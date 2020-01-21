using System.Collections;
using System.Collections.Generic;

public interface IInput
{
    bool MoveCameraInput(); //Padrão- Botão Direito
    bool GoToBattleInput(); //Padrão- Botão Esquerdo
    bool KubberAtk1Input(); //Padrão- Número 1
    bool KubberAtk2Input(); //Padrão- Número 2
    bool KubberAtk3Input(); //Padrão- Número 3
    bool KubberAtk4Input(); //Padrão- Número 4
    float GetAxisHorizontal(); //Padrão- (-1A | 1 D)
    float GetAxisVertical();   //Padrão- (-1S | 1 W)
    bool FixCameraOnMyKubberInput(); //Padrão- Tab
    bool RescueKubberInput(); //Padrão- Control
    bool RunInput(); //Padrão- Shift
    bool ExitInput(); //Padrão- Esc
    bool CaptureKubberInput(); //Padrão- Backspace
}

