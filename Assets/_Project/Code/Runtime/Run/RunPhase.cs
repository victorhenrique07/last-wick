using System;
using System.Collections.Generic;
using System.Text;

namespace Assets._Project.Code.Runtime.Run
{
    public enum RunPhase
    {
        Hordes,        // nascem inimigos, a barra anda
        ClearingArena, // marco atingido: o spawn para e os inimigos restantes vão até o player
        Boss,          // luta com boss de marco ou com o boss final
        Finished       // run encerrada
    }
}
