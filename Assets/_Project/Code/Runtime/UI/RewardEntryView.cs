using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Code.Runtime.UI
{
    // Um recurso no bloco de recompensas da tela de resultado: ícone, nome e quantidade.
    // O ícone e o nome são fixos no prefab; o script só troca a quantidade.
    public class RewardEntryView : MonoBehaviour
    {
        [SerializeField] private Text value;

        // Vitória: só o que o jogador leva (RWD-13)
        public void Show(int delivered) => value.text = delivered.ToString();

        // Morte e escuridão: o coletado ao lado do que sobrou depois da perda (RWD-14)
        public void Show(int collected, int delivered) => value.text = $"{collected} → {delivered}";
    }
}
