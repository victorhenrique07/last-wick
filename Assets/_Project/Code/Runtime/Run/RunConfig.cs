using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Run
{
    [CreateAssetMenu(fileName = "RunConfig", menuName = "Last Wick/Run Config")]
    public class RunConfig : ScriptableObject
    {
        [Tooltip("Duração máxima da run, em segundos. Ao zerar, a escuridão consome o player.")]
        [SerializeField, Min(1f)] private float timeLimit = 600f;

        [Tooltip("Pontos acumulados em que cada boss aparece. O último valor é o boss final.")]
        [SerializeField] private int[] milestones = { 3000, 7500, 14000 };

        public float TimeLimit => timeLimit;
        public int[] Milestones => milestones;
        public int FinalMilestone => milestones[^1];

        public int BaseExp = 100;
        public int ExpGrowth = 200;
    }
}
