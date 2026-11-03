using Assets._Project.Code.Runtime.Bosses;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Run
{
    [CreateAssetMenu(fileName = "RewardConfig", menuName = "Last Wick/Config/Reward Config")]
    public class RewardConfig : ScriptableObject
    {
        [Tooltip("Chance de drop do material")]
        [SerializeField, Range(0f, 1f)] private float materialChance;

        [SerializeField] private BossReward[] bosses;

        public float MaterialChance => materialChance;
        public BossReward[] Bosses => bosses;
    }
}
