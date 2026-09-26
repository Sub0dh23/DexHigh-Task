using UnityEngine;

namespace DexHigh.Combat
{
    public enum DamageType
    {
        Physical,
        Fire,
        Impact,
        Burn
    }

    public struct DamageInfo
    {
        public float Amount;
        public AbilityType AbilitySource;
        public DamageType Type;
        public GameObject Attacker;
        public Vector3 HitPoint;
        public Vector3 HitDirection;
        public float KnockbackForce;
        public bool IsCritical;
        public bool IsFocalHit;

        public DamageInfo(
            float amount, 
            AbilityType abilitySource, 
            GameObject attacker, 
            Vector3 hitPoint, 
            Vector3 hitDirection, 
            float knockbackForce = 0f,
            DamageType damageType = DamageType.Physical,
            bool isCritical = false,
            bool isFocalHit = false)
        {
            Amount = amount;
            AbilitySource = abilitySource;
            Attacker = attacker;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
            KnockbackForce = knockbackForce;
            Type = damageType;
            IsCritical = isCritical;
            IsFocalHit = isFocalHit;
        }
    }
}
