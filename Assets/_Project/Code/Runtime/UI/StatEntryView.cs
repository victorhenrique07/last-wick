using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.UI
{
    // Uma linha de informação com ícone, nome e valor (resumo e status da tela de vitória).
    // O ícone e o nome são fixos no prefab; o script só troca o valor.
    public class StatEntryView : MonoBehaviour
    {
        [SerializeField] private Text value;

        public void SetValue(string text) => value.text = text;
    }
}
