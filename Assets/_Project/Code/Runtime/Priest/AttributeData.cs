using Assets._Project.Code.Runtime.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Priest
{
    [CreateAssetMenu(fileName = "AttributeData", menuName = "Last Wick/AttributeData", order = 1)]
    public class AttributeData : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public Sprite Icon;
        [SerializeField] private List<AttributeEffect> effects;

        public IReadOnlyList<AttributeEffect> Effects => effects;
    }
}
