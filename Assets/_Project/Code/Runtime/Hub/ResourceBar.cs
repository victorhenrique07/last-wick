using Assets._Project.Code.Runtime.Save;
using Assets._Project.Code.Runtime.UI;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Hub
{
    // Barra no topo do hub com os 5 recursos do save; redesenha sempre que o save avisa Changed (HUB-08, HUB-09).
    public class ResourceBar : MonoBehaviour
    {
        [SerializeField] private ResourceCounterView gold;
        [SerializeField] private ResourceCounterView oil;
        [SerializeField] private ResourceCounterView bones;
        [SerializeField] private ResourceCounterView iron;
        [SerializeField] private ResourceCounterView wax;

        void OnEnable()
        {
            SaveSystem.Changed += Refresh;
            Refresh(); // ao entrar no hub, a volta da run já gravou as recompensas (HUB-10)
        }

        void OnDisable() => SaveSystem.Changed -= Refresh;

        private void Refresh()
        {
            var resources = SaveSystem.Data.Resources;
            gold.SetValue(resources.Gold);
            oil.SetValue(resources.Oil);
            bones.SetValue(resources.Bones);
            iron.SetValue(resources.Iron);
            wax.SetValue(resources.Wax);
        }
    }
}
