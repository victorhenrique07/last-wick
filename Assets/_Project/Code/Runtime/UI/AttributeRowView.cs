using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.UI
{
    // Uma linha do menu do padre: ícone, nome, nível, os status que o atributo move ("atual → próximo")
    // e o botão de compra. O menu cria uma por atributo e redesenha quando o save muda.
    public class AttributeRowView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text levelLabel;
        [SerializeField] private Text effectsLabel;
        [SerializeField] private Button buyButton;

        private Action onBuy;

        void Awake() => buyButton.onClick.AddListener(() => onBuy?.Invoke());

        public void Show(Sprite sprite, string displayName, int level, string effects, bool canBuy, Action buy)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
            nameLabel.text = displayName;
            levelLabel.text = $"Nv {level}";
            effectsLabel.text = effects;
            buyButton.interactable = canBuy;
            onBuy = buy;
        }
    }
}
