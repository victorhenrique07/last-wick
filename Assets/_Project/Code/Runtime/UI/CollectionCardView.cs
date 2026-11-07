using System;
using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Code.Runtime.Cards;
using Assets._Project.Code.Runtime.Save;

namespace Assets._Project.Code.Runtime.UI
{
    // Uma carta da aba Coleção: ícone, nome e a descrição do asset (TAROT-08). Bloqueada, mostra embaixo
    // o botão "Desbloquear" e o custo em osso, ferro e cera; o material que falta fica na cor de aviso
    // e o botão desliga (TAROT-02, TAROT-05, TAROT-17).
    public class CollectionCardView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Sprite defaultIcon; // carta sem desenho
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text descriptionLabel;

        [Header("Desbloqueio (só aparece com a carta bloqueada)")]
        [SerializeField] private GameObject unlockGroup;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Text bonesLabel; // só o número; o ícone do material fica ao lado
        [SerializeField] private Text ironLabel;
        [SerializeField] private Text waxLabel;
        [SerializeField] private Color costColor = Color.white;
        [SerializeField] private Color missingColor = new Color32(201, 109, 69, 255); // cor de aviso da paleta

        private Action onUnlock;

        void Awake() => unlockButton.onClick.AddListener(() => onUnlock?.Invoke());

        // owned = materiais do jogador, para pintar o que falta
        public void Show(CardData card, bool locked, ResourceBag owned, Action unlock)
        {
            icon.sprite = card.Icon != null ? card.Icon : defaultIcon;
            nameLabel.text = card.Name;
            descriptionLabel.text = card.Description;

            unlockGroup.SetActive(locked);
            onUnlock = locked ? unlock : null;
            if (!locked) return;

            ResourceBag cost = card.UnlockCost;
            ShowCost(bonesLabel, cost.Bones, owned.Bones);
            ShowCost(ironLabel, cost.Iron, owned.Iron);
            ShowCost(waxLabel, cost.Wax, owned.Wax);
            unlockButton.interactable = owned.Covers(cost);
        }

        private void ShowCost(Text label, int cost, int owned)
        {
            label.text = cost.ToString();
            label.color = owned >= cost ? costColor : missingColor;
        }
    }
}
