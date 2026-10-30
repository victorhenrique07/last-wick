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

        void OnEnable()
        {
            health.Died += OnDied;
            GameEvents.RunTimeExpired += OnRunTimeExpired;
        }

        void OnDisable()
        {
            health.Died -= OnDied;
            GameEvents.RunTimeExpired -= OnRunTimeExpired;
        }

        void OnDied() => GameEvents.RaisePlayerDied();

        // A escuridão consome o player aos 10:00, e isso conta como morte.
        void OnRunTimeExpired() => health.Kill();
    }
}
