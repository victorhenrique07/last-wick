using LastWick.Combat;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Core
{
    // Tremor de tela proporcional ao dano que o player toma.
    // Guarda um "trauma" de 0 a 1: cada dano soma, e ele decai sozinho com o tempo.
    // A intensidade é trauma², então hits pequenos quase não tremem e hits grandes tremem bem mais.
    // Não move a câmera: só calcula o Offset, que o CameraFollow soma antes de arredondar para o pixel.
    public class CameraShake : MonoBehaviour
    {
        [Tooltip("Trauma que todo hit soma, mesmo o menor. Sem isso, hits pequenos ficam abaixo de 1 pixel e somem no arredondamento.")]
        [SerializeField, Range(0f, 1f)] private float minTraumaPerHit = 0.5f;
        [Tooltip("Trauma extra somado por um golpe que tiraria 100% da vida, em cima do mínimo.")]
        [SerializeField, Min(0f)] private float traumaPerFullHealth = 2f;
        [Tooltip("Quanto trauma some por segundo.")]
        [SerializeField, Min(0f)] private float traumaDecay = 1.5f;
        [Tooltip("Deslocamento máximo em unidades de mundo (com PPU 16, 0.0625 = 1 pixel).")]
        [SerializeField, Min(0f)] private float maxOffset = 0.4f;
        [Tooltip("Velocidade do ruído. Mais alto = tremor mais nervoso.")]
        [SerializeField, Min(0f)] private float frequency = 25f;
        [Tooltip("Multiplicador global, para uma futura opção de acessibilidade. 0 desliga.")]
        [SerializeField, Range(0f, 1f)] private float intensity = 1f;

        private Health health;
        private float trauma;
        private float time;
        private float seedX;
        private float seedY;

        public Vector3 Offset { get; private set; }
        public bool IsShaking => trauma > 0f;

        public float Intensity
        {
            get => intensity;
            set => intensity = Mathf.Clamp01(value);
        }

        private void Awake()
        {
            seedX = Random.value * 100f;
            seedY = seedX + 50f;
        }

        // No Start, igual ao CameraFollow: o player já existe na cena.
        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            health = player.GetComponentInChildren<Health>();
            if (health != null) health.Damaged += OnDamaged;
        }

        private void OnDestroy()
        {
            if (health != null) health.Damaged -= OnDamaged;
        }

        public void AddTrauma(float amount) => trauma = Mathf.Clamp01(trauma + amount);

        // Proporcional à vida máxima: com mais vida, o mesmo golpe "dói" menos e treme menos.
        // Um tremor por vez: hits que chegam enquanto a tela ainda treme não disparam outro,
        // senão uma horda encostando deixa a tela tremendo sem parar.
        private void OnDamaged(DamageInfo info)
        {
            if (IsShaking) return;
            AddTrauma(minTraumaPerHit + info.Amount / health.Max * traumaPerFullHealth);
        }

        // Unscaled: se o jogo pausar logo após um hit (level up, fim de run), a câmera não congela deslocada.
        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            trauma = Mathf.MoveTowards(trauma, 0f, traumaDecay * deltaTime);
            time += deltaTime * frequency;

            float shake = trauma * trauma * intensity;
            if (shake <= 0f)
            {
                Offset = Vector3.zero;
                return;
            }

            // Perlin varia de forma contínua: o tremor balança em vez de virar chiado.
            float x = Mathf.PerlinNoise(seedX, time) * 2f - 1f;
            float y = Mathf.PerlinNoise(seedY, time) * 2f - 1f;
            Offset = new Vector3(x, y, 0f) * (maxOffset * shake);
        }
    }
}
