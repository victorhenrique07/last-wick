using UnityEngine;
using UnityEngine.InputSystem;

namespace LastWick.Player
{
    // Lê as ações do Input System do projeto (Assets/Settings/InputSystem_Actions)
    // e expõe o estado para os outros scripts do player.
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private string moveAction = "Player/Move";
        [SerializeField] private string attackAction = "Player/Attack";
        [Tooltip("Por enquanto a esquiva usa a ação Jump (Espaço).")]
        [SerializeField] private string dodgeAction = "Player/Jump";

        private InputAction move;
        private InputAction attack;
        private InputAction dodge;
        private Camera mainCamera;

        public Vector2 Move => move.ReadValue<Vector2>();
        public bool AttackHeld => attack.IsPressed();
        public bool DodgePressed => dodge.WasPressedThisFrame();

        // Posição do mouse no mundo, usada pela mira.
        public Vector2 AimWorldPosition
        {
            get
            {
                if (mainCamera == null || Mouse.current == null)
                    return transform.position;
                return mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
        }

        private void Awake()
        {
            InputActionAsset actions = InputSystem.actions;
            move = actions.FindAction(moveAction, true);
            attack = actions.FindAction(attackAction, true);
            dodge = actions.FindAction(dodgeAction, true);
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            move.Enable();
            attack.Enable();
            dodge.Enable();
        }
    }
}
