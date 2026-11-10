using LastWick.Combat;
using UnityEngine;

namespace LastWick.UI
{
    // Barra de vida do player: o preenchimento (Fill) encolhe pela âncora direita.
    // A animação (drain suave + rastro branco de dano) fica no HealthBarDrain.
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private RectTransform fill;
        [SerializeField] private HealthBarDrain drain = new HealthBarDrain();

        private void Awake() => drain.Setup(fill);

        private void OnEnable()
        {
            if (health == null) return;
            health.Damaged += OnDamaged;
            health.Healed += OnHealed;
        }

        private void OnDisable()
        {
            if (health == null) return;
            health.Damaged -= OnDamaged;
            health.Healed -= OnHealed;
        }

        // O Health só enche a vida no próprio OnEnable, e a ordem entre objetos não é garantida.
        // O Start roda depois de todos os OnEnable, então o valor inicial já está certo.
        private void Start()
        {
            if (health != null) drain.Snap(Percent);
        }

        // Unscaled: a barra termina de animar mesmo com o jogo pausado (level up, fim de run).
        private void Update() => drain.Tick(Time.unscaledDeltaTime);

        private void OnDamaged(DamageInfo info) => drain.SetTarget(Percent);
        private void OnHealed(float amount) => drain.SetTarget(Percent);

        private float Percent => health.Current / health.Max;
    }
}
