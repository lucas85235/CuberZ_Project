using System.Collections;
using System.Collections.Generic;

public interface IInput
{
    bool MoveCamera(); //Padrão- Botão Direito
    bool ExecuteAction(); //Padrão- Botão Esquerdo
    bool KubberAttack1(); //Padrão- Número 1
    bool KubberAttack2(); //Padrão- Número 2
    bool KubberAttack3(); //Padrão- Número 3
    bool KubberAttack4(); //Padrão- Número 4
    float GetAxisHorizontal(); //Padrão- (-1A | 1 D)
    float GetAxisVertical();   //Padrão- (-1S | 1 W)
    float GetAxisMouseX(); // Padrão- (-1A | 1 D)
    float GetAxisMouseY();   // Padrão- (-1S | 1 W)
    bool FixCameraOnMyKubberInput(); //Padrão- Tab
    bool RescueKubberInput(); //Padrão- Control
    bool RunInput(); //Padrão- LeftShift
    bool RunInputUp(); //Padrão - LeftShift
    bool RunInputOnce(); //Padrão - LeftShift    
    bool Exit(); //Padrão- Esc
    bool EnterInCaptureMode(); //Padrão- Backspace
    bool Jump(); //Padrão- SpaceBar
    bool JumpOnce(); //Padrão- SpaceBar
}

