using System;
using TPL.PVZR.Classes.DataClasses.Item.Card;

namespace TPL.PVZR.Classes
{
    public enum ProjectileId
    {
        Pea,
        FrozenPea
    }

    public enum ZombieId
    {
        NormalZombie,
        ConeheadZombie
    }

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
    }

    public enum PlantId
    {
        NotSet,
        PeaShooter,
        Sunflower,
        Wallnut,
        Flowerpot,
        SnowPea,
        Marigold,
    
        // CherryBoom,
        // PotatoMine,
        // Blover,
        // CabbagePult,
        // Cactus,
        // Caltrop,
        // CornPult,
        // MelonPult,
        // RepeaterPea,
        // SplitPea,
        // Chomper
    }
    
    public enum PlantVariant
    {
        V0,
        V1,
        V2,
        V3,
        V4,
        V5,
        V6,
        V7,
        V8,
        V9
    }
}