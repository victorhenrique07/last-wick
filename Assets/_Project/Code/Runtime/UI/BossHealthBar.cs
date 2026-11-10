using LastWick.Combat;
using UnityEngine;
using Assets._Project.Code.Runtime.Core;

namespace LastWick.UI
{
    // Barra de vida do boss: aparece quando ele nasce e some quando ele morre.
    // Este objeto fica sempre ativo para ouvir o evento. Quem liga e desliga é o panel.
    // A animação (drain suave + rastro branco de dano) fica no HealthBarDrain.
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private RectTransform fill;
        [SerializeField] private HealthBarDrain drain = new HealthBarDrain();

        private Health boss;
        private bool hiding;

        private void Awake() => drain.Setup(fill);

        private void OnEnable() => GameEvents.BossSpawned += OnBossSpawned;

        private void OnDisable()
        {
            GameEvents.BossSpawned -= OnBossSpawned;
            Unbind();
        }

        private void Update()
        {
            if (!panel.activeSelf) return;

            drain.Tick(Time.unscaledDeltaTime);

            // Depois da morte, espera a barra e o rastro esvaziarem antes de esconder.
            if (hiding && drain.IsSettled)
            {
                hiding = false;
                panel.SetActive(false);
            }
        }

        private void OnBossSpawned(Health spawned)
        {
            Unbind();

            boss = spawned;
            boss.Damaged += OnDamaged;
            boss.Healed += OnHealed;
            boss.Died += OnDied;

            hiding = false;
            panel.SetActive(true);
            drain.Snap(Percent);
        }

        private void OnDamaged(DamageInfo info) => drain.SetTarget(Percent);
        private void OnHealed(float amount) => drain.SetTarget(Percent);

        private void OnDied()
        {
            Unbind();
            drain.SetTarget(0f);
            hiding = true;
        }

        private void Unbind()
        {
            if (boss == null) return;
            boss.Damaged -= OnDamaged;
            boss.Healed -= OnHealed;
            boss.Died -= OnDied;
            boss = null;
        }

        private float Percent => boss.Current / boss.Max;
    }
}
