using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.Cards
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button button;

        public CardData Data { get; private set; }

        public void Show(CardData data, Action<CardView> onClick)
        {
            Data = data;
            nameText.text = data.Name;
            descriptionText.text = data.Description;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick(this));
        }
    }
}
