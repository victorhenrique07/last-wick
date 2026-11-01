using Assets._Project.Code.Runtime.Core;
using LastWick.Combat;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Enemies
{
    [RequireComponent(typeof(Health))]
    public class EnemyDeath : MonoBehaviour
    {
        [SerializeField, Min(0)] private int pointsValue = 10;
        [SerializeField, Min(0)] private int expOnDeath;
        [SerializeField] private XpOrb XpOrb;

        private Health health;

        void Awake() => health = GetComponent<Health>();

        private void OnEnable() => health.Died += OnDied;
        private void OnDisable() => health.Died -= OnDied;

        private void OnDied()
        {
            GameEvents.RaiseEnemyKilled(pointsValue);

            XpOrb orb = Instantiate(XpOrb, transform.position, Quaternion.identity);
            orb.Init(expOnDeath);
        }
    }
}
