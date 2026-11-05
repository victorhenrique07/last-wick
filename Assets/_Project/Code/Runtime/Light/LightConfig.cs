using UnityEngine;

namespace Assets._Project.Code.Runtime.Light
{
    // Números da fogueira: o raio de luz de cada nível e o custo em óleo para subir de nível.
    [CreateAssetMenu(fileName = "LightConfig", menuName = "Last Wick/Config/Light Config")]
    public class LightConfig : ScriptableObject
    {
        [Header("Luz")]
        [Tooltip("Raio da luz do player no nível 0 da fogueira, em unidades.")]
        [SerializeField, Min(0f)] private float baseRadius = 3f;

        [Tooltip("Quanto cada nível da fogueira soma ao raio.")]
        [SerializeField, Min(0f)] private float radiusPerLevel = 0.5f;

        [Tooltip("Nível máximo da fogueira.")]
        [SerializeField, Min(1)] private int maxLevel = 10;

        [Header("Custo")]
        [Tooltip("Custo do nível 1, em óleo.")]
        [SerializeField, Min(0)] private int baseCost = 40;

        [Tooltip("Quanto cada nível seguinte custa a mais, em óleo: o nível n custa base + passo × (n − 1).")]
        [SerializeField, Min(0)] private int costStep = 20;

        public float BaseRadius => baseRadius;
        public float RadiusPerLevel => radiusPerLevel;
        public int MaxLevel => maxLevel;
        public int BaseCost => baseCost;
        public int CostStep => costStep;
    }
}
