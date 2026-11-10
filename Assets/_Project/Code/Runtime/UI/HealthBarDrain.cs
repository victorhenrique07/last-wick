using System;
using UnityEngine;
using UnityEngine.UI;

namespace LastWick.UI
{
    // Animação compartilhada pelas barras de vida.
    // O Fill desce suave até a vida real. Atrás dele, um rastro branco espera um pouco
    // e depois desce mais devagar, mostrando quanto de dano acabou de entrar.
    [Serializable]
    public class HealthBarDrain
    {
        [Tooltip("Opcional. Se vazio, o rastro é criado em runtime como cópia do Fill.")]
        [SerializeField] private RectTransform damageFill;
        [SerializeField] private Color trailColor = Color.white;
        [Tooltip("Fração da barra por segundo que o Fill anda até a vida real.")]
        [SerializeField] private float fillSpeed = 2f;
        [Tooltip("Segundos que o rastro espera depois do último dano.")]
        [SerializeField] private float trailDelay = 0.4f;
        [Tooltip("Fração da barra por segundo que o rastro desce.")]
        [SerializeField] private float trailSpeed = 0.6f;

        private RectTransform fill;
        private float target;
        private float shown;
        private float trail;
        private float delayTimer;

        // True quando o Fill e o rastro já chegaram no valor real.
        public bool IsSettled => shown == target && trail == shown;

        public void Setup(RectTransform fillRect)
        {
            fill = fillRect;
            if (damageFill == null) damageFill = CreateTrail();
        }

        // Pula direto para o valor, sem animar (vida inicial, boss novo).
        public void Snap(float percent)
        {
            target = shown = trail = Mathf.Clamp01(percent);
            delayTimer = 0f;
            Apply();
        }

        public void SetTarget(float percent)
        {
            percent = Mathf.Clamp01(percent);
            // Só dano reinicia a espera. Hits seguidos acumulam um bloco branco maior.
            if (percent < target) delayTimer = trailDelay;
            target = percent;
        }

        public void Tick(float deltaTime)
        {
            shown = Mathf.MoveTowards(shown, target, fillSpeed * deltaTime);

            if (delayTimer > 0f) delayTimer -= deltaTime;
            else trail = Mathf.MoveTowards(trail, shown, trailSpeed * deltaTime);

            // Na cura o rastro nunca fica abaixo do Fill, senão ele some atrás dele e "atrasa" o próximo dano.
            trail = Mathf.Max(trail, shown);

            Apply();
        }

        private void Apply()
        {
            fill.anchorMax = new Vector2(shown, fill.anchorMax.y);
            damageFill.anchorMax = new Vector2(trail, damageFill.anchorMax.y);
        }

        // Duplica o Fill (mesmas âncoras e offsets) e coloca a cópia logo antes dele na hierarquia,
        // assim ela é desenhada atrás.
        private RectTransform CreateTrail()
        {
            var copy = UnityEngine.Object.Instantiate(fill.gameObject, fill.parent);
            copy.name = "DamageFill";
            copy.transform.SetSiblingIndex(fill.GetSiblingIndex());

            if (copy.TryGetComponent(out Image image)) image.color = trailColor;
            return (RectTransform)copy.transform;
        }
    }
}
