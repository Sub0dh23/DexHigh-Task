using UnityEngine;

namespace DexHigh.Combat
{
    public interface IDamageable
    {
        void TakeDamage(DamageInfo damageInfo);
        bool IsAlive { get; }
        Transform Transform { get; }
    }
}
