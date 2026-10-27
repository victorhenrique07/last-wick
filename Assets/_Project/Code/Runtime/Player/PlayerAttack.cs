using LastWick.Combat;
using UnityEngine;

namespace LastWick.Player
{
    // Ataque manual: enquanto o botão estiver segurado, dispara a arma na direção da mira.
    [RequireComponent(typeof(PlayerInput), typeof(PlayerAim))]
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;

        private PlayerInput input;
        private PlayerAim aim;

        private void Reset() => weapon = GetComponentInChildren<Weapon>();

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            aim = GetComponent<PlayerAim>();
        }

        private void Update()
        {
            if (weapon != null && input.AttackHeld)
                weapon.TryFire(aim.Direction);
        }
    }
}
