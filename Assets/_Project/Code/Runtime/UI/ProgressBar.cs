using Assets._Project.Code.Runtime.Run;
using UnityEngine;
using UnityEngine.UI;

namespace LastWick.UI
{
    // Barra de pontos da run dividida em um segmento por marco.
    // Cada segmento tem largura proporcional ao seu trecho e enche sozinho.
    // Este objeto precisa de um HorizontalLayoutGroup, que distribui os segmentos.
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RunConfig config;
        [SerializeField] private RunProgress progress;
        [SerializeField] private Color segmentColor = new Color(0.12f, 0.1f, 0.13f, 0.9f);
        [SerializeField] private Color fillColor = new Color(0.95f, 0.7f, 0.25f);

        private RectTransform[] fills;

        private void Awake() => Build();

        private void OnEnable()
        {
            if (progress != null)
                progress.PointsChanged += Refresh;
        }

        private void OnDisable()
        {
            if (progress != null)
                progress.PointsChanged -= Refresh;
        }

        private void Start()
        {
            if (progress != null)
                Refresh(progress.Points);
        }

        // Cria um segmento por marco. O flexibleWidth do trecho faz o layout dividir a largura na proporção certa.
        private void Build()
        {
            int[] milestones = config.Milestones;
            fills = new RectTransform[milestones.Length];

            for (int i = 0; i < milestones.Length; i++)
            {
                int start = i == 0 ? 0 : milestones[i - 1];
                int span = milestones[i] - start;

                var segment = new GameObject($"Segment_{i}", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
                segment.transform.SetParent(transform, false);
                segment.GetComponent<Image>().color = segmentColor;
                segment.GetComponent<LayoutElement>().flexibleWidth = span;

                var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                fillObject.transform.SetParent(segment.transform, false);
                fillObject.GetComponent<Image>().color = fillColor;

                var rect = fillObject.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = new Vector2(0f, 1f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                fills[i] = rect;
            }
        }

        private void Refresh(int points)
        {
            int[] milestones = config.Milestones;

            for (int i = 0; i < fills.Length; i++)
            {
                int start = i == 0 ? 0 : milestones[i - 1];
                float ratio = Mathf.Clamp01((points - start) / (float)(milestones[i] - start));
                fills[i].anchorMax = new Vector2(ratio, 1f);
            }
        }
    }
}
