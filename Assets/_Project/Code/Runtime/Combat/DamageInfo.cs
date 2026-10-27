using UnityEngine;

namespace LastWick.Combat
{
    // Tudo o que um acerto carrega: quanto, de que direção e de quem veio.
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly Vector2 Direction;
        public readonly GameObject Source;

        public DamageInfo(float amount, Vector2 direction, GameObject source)
        {
            Amount = amount;
            Direction = direction;
            Source = source;
        }
    }
}
