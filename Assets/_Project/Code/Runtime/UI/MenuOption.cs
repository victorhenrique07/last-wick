using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.UI
{
    // Opção de menu em texto: com o mouse em cima (ou selecionada pelo teclado), acende o texto
    // e mostra os colchetes ornamentados dos dois lados. Desabilitada, fica apagada e não acende.
    [RequireComponent(typeof(Button))]
    public class MenuOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Text label;
        [SerializeField] private Image[] brackets; // Image.enabled, e não SetActive, para o texto não pular de lugar
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private Color disabledColor = Color.gray;

        private Button button;
        private bool hovered;

        private Button Button => button != null ? button : button = GetComponent<Button>();

        void OnEnable()
        {
            hovered = false;
            Refresh();
        }

        public void SetInteractable(bool value)
        {
            Button.interactable = value;
            Refresh();
        }

        public void OnPointerEnter(PointerEventData eventData) => SetHovered(true);
        public void OnPointerExit(PointerEventData eventData) => SetHovered(false);
        public void OnSelect(BaseEventData eventData) => SetHovered(true);
        public void OnDeselect(BaseEventData eventData) => SetHovered(false);

        private void SetHovered(bool value)
        {
            hovered = value;
            Refresh();
        }

        private void Refresh()
        {
            bool lit = hovered && Button.interactable;
            label.color = !Button.interactable ? disabledColor : lit ? highlightColor : normalColor;
            foreach (var bracket in brackets)
                bracket.enabled = lit;
        }
    }
}
