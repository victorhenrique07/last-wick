using System;
using System.Collections.Generic;
using Assets._Project.Code.Runtime.Save;
using UnityEngine;

namespace LastWick.Combat
{
    // Um nível de melhoria da arma: os bônus que ele soma e o custo para comprá-lo (SMITH-12).
    [Serializable]
    public class WeaponLevel
    {
        [Tooltip("Dano somado ao dano da arma. Não é uma porcentagem.")]
        [Min(0f)] public float DamageBonus;
        [Tooltip("Fração somada à cadência. 0,1 = +10%.")]
        [Min(0f)] public float FireRateBonus;
        [Tooltip("Fração somada à velocidade do projétil. 0,1 = +10%.")]
        [Min(0f)] public float ProjectileSpeedBonus;
        [Tooltip("Fração somada à vida do projétil, o que aumenta o alcance. 0,1 = +10%.")]
        [Min(0f)] public float LifetimeBonus;
        [Tooltip("Inimigos a mais atravessados. É uma quantidade.")]
        [Min(0)] public int PierceBonus;
        [Tooltip("Custo do nível. Só osso, ferro e cera.")]
        public ResourceBag Cost;
    }

    // Uma arma como dado: projétil, status base e níveis de melhoria (SMITH-11).
    [CreateAssetMenu(fileName = "Weapon", menuName = "Last Wick/Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Tooltip("Chave da arma no save. Não mude depois que houver saves.")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Projectile projectilePrefab;

        [Header("Status base")]
        [SerializeField, Min(0f)] private float baseDamage = 10f;
        [Tooltip("Disparos por segundo.")]
        [SerializeField, Min(0.01f)] private float baseFireRate = 4f;

        [Header("Melhorias")]
        [Tooltip("Níveis do ferreiro, em ordem. O nível N soma os bônus dos níveis 1 até N.")]
        [SerializeField] private List<WeaponLevel> levels = new();

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public Projectile ProjectilePrefab => projectilePrefab;
        public float BaseDamage => baseDamage;
        public float BaseFireRate => baseFireRate;
        public IReadOnlyList<WeaponLevel> Levels => levels;
        public int MaxLevel => levels != null ? levels.Count : 0;

        // O ferreiro só cobra osso, ferro e cera (SMITH-12): ouro e óleo num nível são zerados com aviso
        private void OnValidate()
        {
            if (levels == null) return;

            for (int i = 0; i < levels.Count; i++)
            {
                WeaponLevel level = levels[i];
                if (level == null || (level.Cost.Gold == 0 && level.Cost.Oil == 0)) continue;

                Debug.LogWarning($"WeaponData: o nível {i + 1} de {name} custava ouro ou óleo; o ferreiro só usa osso, ferro e cera, e esses valores foram zerados.", this);
                level.Cost.Gold = 0;
                level.Cost.Oil = 0;
            }
        }
    }
}
