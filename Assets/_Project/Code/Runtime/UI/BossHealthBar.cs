using LastWick.Combat;
using UnityEngine;

namespace LastWick.UI
{
    // Barra de vida do boss: aparece quando ele nasce e some quando ele morre.
    // Este objeto fica sempre ativo para ouvir o evento. Quem liga e desliga é o panel.
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private RectTransform fill;

        private Health boss;

        private void OnEnable() => GameEvents.BossSpawned += OnBossSpawned;

        private void OnDisable()
        {
            GameEvents.BossSpawned -= OnBossSpawned;
            Unbind();
        }

        private void OnBossSpawned(Health spawned)
        {
            Unbind();

            boss = spawned;
            boss.Damaged += OnDamaged;
            boss.Healed += OnHealed;
            boss.Died += OnDied;

            panel.SetActive(true);
            Refresh();
        }

        private void OnDamaged(DamageInfo info) => Refresh();
        private void OnHealed(float amount) => Refresh();

        private void OnDied()
        {
            Unbind();
            panel.SetActive(false);
        }

        private void Unbind()
        {
            if (boss == null) return;
            boss.Damaged -= OnDamaged;
            boss.Healed -= OnHealed;
            boss.Died -= OnDied;
            boss = null;
        }

        private void Refresh()
        {
            fill.anchorMax = new Vector2(Mathf.Clamp01(boss.Current / boss.Max), 1f);
        }
    }
}
