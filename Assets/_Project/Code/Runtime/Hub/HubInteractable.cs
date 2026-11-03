using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets._Project.Code.Runtime.Hub
{
    // Objeto clicável do hub (igreja, bigorna, mesa de tarot): mostra o contorno com o mouse em cima
    // e dispara onClick no clique. Precisa de um Collider2D, de um Physics2DRaycaster na câmera
    // e de um EventSystem na cena.
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class HubInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private UnityEvent onClick;

        private SpriteRenderer outline;

        void Awake()
        {
            var sprite = GetComponent<SpriteRenderer>();

            // Cópia do sprite, desenhada por cima com o material de contorno
            var outlineObject = new GameObject("Contorno");
            outlineObject.transform.SetParent(transform, false);

            outline = outlineObject.AddComponent<SpriteRenderer>();
            outline.sprite = sprite.sprite;
            outline.material = outlineMaterial;
            outline.sortingLayerID = sprite.sortingLayerID;
            outline.sortingOrder = sprite.sortingOrder + 1;
            outline.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData) => outline.enabled = true;

        public void OnPointerExit(PointerEventData eventData) => outline.enabled = false;

        public void OnPointerClick(PointerEventData eventData) => onClick.Invoke();
    }
}
