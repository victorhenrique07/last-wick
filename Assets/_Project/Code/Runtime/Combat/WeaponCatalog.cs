using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LastWick.Combat
{
    // Todas as armas do jogo e a arma que um save novo equipa (SMITH-13).
    [CreateAssetMenu(fileName = "WeaponCatalog", menuName = "Last Wick/Weapons/Weapon Catalog")]
    public class WeaponCatalog : ScriptableObject
    {
        [SerializeField] private List<WeaponData> weapons = new();
        [Tooltip("Arma equipada num save novo ou quando a arma salva não existe mais.")]
        [SerializeField] private WeaponData startingWeapon;

        public IReadOnlyList<WeaponData> Weapons => weapons;
        public WeaponData StartingWeapon => startingWeapon;

        // Arma sem Id nunca é encontrada, para não ser equipada por engano
        public WeaponData Find(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            WeaponData found = weapons?.FirstOrDefault(w => w != null && !string.IsNullOrWhiteSpace(w.Id) && w.Id == id);
            if (found != null) return found;

            // A arma inicial vale mesmo que alguém esqueça de pô-la na lista
            return startingWeapon != null && startingWeapon.Id == id ? startingWeapon : null;
        }

        private void OnValidate()
        {
            if (weapons == null) return;

            var ids = new HashSet<string>();

            foreach (WeaponData weapon in weapons)
            {
                if (weapon == null) continue;

                if (string.IsNullOrWhiteSpace(weapon.Id))
                    Debug.LogWarning($"WeaponCatalog: a arma {weapon.name} está sem Id.", this);
                else if (!ids.Add(weapon.Id))
                    Debug.LogWarning($"WeaponCatalog: o Id \"{weapon.Id}\" se repete em {weapon.name}.", this);
            }

            if (startingWeapon != null && !weapons.Contains(startingWeapon))
                Debug.LogWarning($"WeaponCatalog: a arma inicial {startingWeapon.name} não está na lista de armas; ponha-a na lista.", this);
        }
    }
}
