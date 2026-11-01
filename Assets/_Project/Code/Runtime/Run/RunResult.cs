using System;
using System.Collections.Generic;
using System.Text;
using LastWick.Player;

namespace Assets._Project.Code.Runtime.Run
{
    public enum RunEndReason
    {
        Victory,   // boss final derrotado
        Death,     // vida do player chegou a 0
        Darkness,  // relógio de 10:00 zerou com a barra incompleta
        Quit       // saiu no meio da run
    }

    public class RunResult
    {
        public RunEndReason Reason;
        public float ElapsedTime;        // segundos desde o início da run
        public float RemainingTime;      // segundos que sobraram no relógio
        public int Points;               // pontos acumulados na barra
        public int Kills;
        public int MilestonesReached;    // marcos atingidos (0 a 3)
        public int Level;                // nível final da run
        public IReadOnlyDictionary<CardData, int> Cards; // cartas escolhidas e quantidade de cada uma
        public PlayerRunStats Stats;     // status finais do player

        // Recompensas coletadas, antes da regra de perda de 50% em morte, saída ou escuridão
        public int Gold;
        public int Oil;
        public int Bones;                // nomes provisórios dos materiais gerais
        public int Iron;
        public int Wax;
    }
}
