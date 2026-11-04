using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.UI
{
    // Um recurso na barra do hub: ícone e quantidade. O ícone é fixo no prefab; o script só troca o número.
    public class ResourceCounterView : MonoBehaviour
    {
        [SerializeField] private Text value;

        public void SetValue(int amount) => value.text = amount.ToString();
    }
}
