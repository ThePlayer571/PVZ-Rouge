using System;
using TPL.PVZR.Classes.DataClasses.Item.Card;

namespace TPL.PVZR.Classes
{
    [Serializable]
    public enum ProjectileId
    {
        NotSet = 0,
        Pea = 1,
        FrozenPea = 2,
        MungBean = 3,
        Spike = 4,
    }

    [Serializable]
    public enum ZombieId
    {
        NotSet = 0,
        NormalZombie = 1,
        ConeheadZombie = 2,
        BucketHeadZombie = 3,
        ScreenDoorZombie = 4,
        NewspaperZombie = 5,
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

    [Serializable]
    public enum PlantId
    {
        NotSet = 0,
        PeaShooter = 1,
        Sunflower = 2,
        Wallnut = 3,
        Flowerpot = 4,
        SnowPea = 5,
        Marigold = 6,
        Repeater = 7,
        PotatoMine = 8,
        CherryBomb = 9,
        SplitPea = 10,
        Cactus = 11,

        // Blover,
        // CabbagePult,
        // Caltrop,
        // CornPult,
        // MelonPult,
        // Chomper
    }

    [Serializable]
    public enum PlantVariant
    {
        V0 = 0,
        V1 = 1,
        V2 = 2,
        V3 = 3,
        V4 = 4,
        V5 = 5,
        V6 = 6,
        V7 = 7,
        V8 = 8,
        V9 = 9,
    }
}