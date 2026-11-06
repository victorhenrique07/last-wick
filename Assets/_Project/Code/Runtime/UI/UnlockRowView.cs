using System;
using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Save;

namespace Assets._Project.Code.Runtime.UI
{
    // Uma linha da aba Desbloquear: ícone, nome, custo em osso, ferro e cera e o botão "Desbloquear".
    // O material que falta fica na cor de aviso e o botão desliga (TAROT-02, TAROT-17).
    public class UnlockRowView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Sprite defaultIcon; // carta sem desenho
        [SerializeField] private Text nameLabel;

        [Header("Custo (só o número; o ícone do material fica ao lado)")]
        [SerializeField] private Text bonesLabel;
        [SerializeField] private Text ironLabel;
        [SerializeField] private Text waxLabel;
        [SerializeField] private Color costColor = Color.white;
        [SerializeField] private Color missingColor = new Color32(201, 109, 69, 255); // cor de aviso da paleta

        [SerializeField] private Button unlockButton;

        private Action onUnlock;

        void Awake() => unlockButton.onClick.AddListener(() => onUnlock?.Invoke());

        // owned = materiais do jogador, para pintar o que falta
        public void Show(CardData card, ResourceBag owned, Action unlock)
        {
            ResourceBag cost = card.UnlockCost;

            icon.sprite = card.Icon != null ? card.Icon : defaultIcon;
            nameLabel.text = card.Name;

            ShowCost(bonesLabel, cost.Bones, owned.Bones);
            ShowCost(ironLabel, cost.Iron, owned.Iron);
            ShowCost(waxLabel, cost.Wax, owned.Wax);

            unlockButton.interactable = owned.Covers(cost);
            onUnlock = unlock;
        }

        private void ShowCost(Text label, int cost, int owned)
        {
            label.text = cost.ToString();
            label.color = owned >= cost ? costColor : missingColor;
        }
    }
}
