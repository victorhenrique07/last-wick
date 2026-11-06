using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.UI
{
    // Uma linha do menu do ferreiro: o nome do status e o valor "atual → próximo" (SMITH-03).
    // O menu cria uma por status que muda no próximo nível.
    public class WeaponStatRowView : MonoBehaviour
    {
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text valueLabel;

        public void Show(string statName, string current, string next)
        {
            nameLabel.text = statName;
            valueLabel.text = $"{current} → {next}";
        }
    }
}
