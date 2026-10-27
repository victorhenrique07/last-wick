using LastWick.Combat;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Player
{
    [RequireComponent(typeof(Health))]
    public class PlayerDeath : MonoBehaviour
    {
        private Health health;

        void Awake() => health = GetComponent<Health>();

        void OnEnable() => health.Died += OnDied;

        void OnDisable() => health.Died -= OnDied;

        void OnDied() => GameEvents.RaisePlayerDied();
    }
}
