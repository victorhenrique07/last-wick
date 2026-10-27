using LastWick.Combat;
using UnityEngine;

namespace LastWick.UI
{
    // Barra de vida do player: o preenchimento (Fill) encolhe pela âncora direita.
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private RectTransform fill;

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
        private void Start() => Refresh();

        private void OnDamaged(DamageInfo info) => Refresh();
        private void OnHealed(float amount) => Refresh();

        private void Refresh()
        {
            if (health == null) return;
            fill.anchorMax = new Vector2(Mathf.Clamp01(health.Current / health.Max), 1f);
        }
    }
}
