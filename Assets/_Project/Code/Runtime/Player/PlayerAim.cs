using UnityEngine;

namespace LastWick.Player
{
    // Mira com o mouse: calcula a direção e gira o AimPivot, onde a arma fica pendurada.
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] private Transform aimPivot;

        private PlayerInput input;

        public Vector2 Direction { get; private set; } = Vector2.right;

        private void Reset() => aimPivot = transform.Find("AimPivot");

        private void Awake() => input = GetComponent<PlayerInput>();

        private void Update()
        {
            Vector2 toMouse = input.AimWorldPosition - (Vector2)transform.position;
            if (toMouse.sqrMagnitude > 0.0001f)
                Direction = toMouse.normalized;

            if (aimPivot != null)
                aimPivot.right = Direction;
        }
    }
}
