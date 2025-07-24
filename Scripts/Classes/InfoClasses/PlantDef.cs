using System;

namespace TPL.PVZR.Classes.InfoClasses
{
    [Serializable]
    public struct PlantDef : IEquatable<PlantDef>
    {
        public PlantId Id;
        public PlantVariant Variant;

        public PlantDef(PlantId id, PlantVariant variant)
        {
            Id = id;
            Variant = variant;
        }

        public bool Equals(PlantDef other)
        {
            return Id == other.Id && Variant == other.Variant;
        }

        public override bool Equals(object obj)
        {
            return obj is PlantDef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Variant);
        }

        public static bool operator ==(PlantDef left, PlantDef right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlantDef left, PlantDef right)
        {
            return !left.Equals(right);
        }

        public static implicit operator PlantId(PlantDef def)
        {
            return def.Id;
        }
    }
}