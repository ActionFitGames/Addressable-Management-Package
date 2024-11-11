
using System;
using System.Collections.Generic;

namespace ActionFit.Framework.Addressable
{
    internal sealed class AssetKey : IEquatable<AssetKey>, IEquatable<AssetKeySt>
    {
        #region Read-Only

        internal string Primary { get; }
        internal string Runtime { get; }
        internal string InternalID { get; }
        internal Type Type { get; }

        #endregion
        
        internal AssetKey() { }
        internal AssetKey(string primary, string runtime, string internalId, Type type)
        {
            Primary = primary;
            Runtime = runtime;
            InternalID = internalId;
            Type = type;
        }

        internal bool IsValid => !string.IsNullOrEmpty(Primary) && 
                                !string.IsNullOrEmpty(Runtime) && 
                                !string.IsNullOrEmpty(InternalID);

        public bool Equals(AssetKey other)
        {
            if (other == null) return false;
            return string.Equals(Primary, other.Primary) || 
                   string.Equals(Runtime, other.Runtime) ||
                   string.Equals(InternalID, other.InternalID);
        }

        public bool Equals(AssetKeySt other)
        {
            return string.Equals(Primary, other.Primary) || 
                   string.Equals(Runtime, other.Runtime) ||
                   string.Equals(InternalID, other.InternalID);
        }

        public override bool Equals(object obj) => obj switch
        {
            AssetKey key => Equals(key),
            AssetKeySt st => Equals(st),
            _ => false
        };

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                if (!string.IsNullOrEmpty(Primary)) return hash * 31 + Primary.GetHashCode();
                if (!string.IsNullOrEmpty(Runtime)) return hash * 31 + Runtime.GetHashCode();
                if (!string.IsNullOrEmpty(InternalID)) return hash * 31 + InternalID.GetHashCode();
                return hash;
            }
        }
    }

    internal struct AssetKeySt : IEquatable<AssetKeySt>, IEquatable<AssetKey>
    {
        internal string Primary { get; set; }
        internal string Runtime { get; set; }
        internal string InternalID { get; set; }
        internal Type Type { get; set; }

        internal bool IsValid => !string.IsNullOrEmpty(Primary) && !string.IsNullOrEmpty(Runtime);

        public bool Equals(AssetKeySt other) =>
            string.Equals(Primary, other.Primary) || 
            string.Equals(Runtime, other.Runtime) ||
            string.Equals(InternalID, other.InternalID);

        public bool Equals(AssetKey other)
        {
            if (other == null) return false;
            return string.Equals(Primary, other.Primary) || 
                   string.Equals(Runtime, other.Runtime) ||
                   string.Equals(InternalID, other.InternalID);
        }

        public override bool Equals(object obj) => obj switch
        {
            AssetKey key => Equals(key),
            AssetKeySt st => Equals(st),
            _ => false
        };

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                if (!string.IsNullOrEmpty(Primary)) return hash * 31 + Primary.GetHashCode();
                if (!string.IsNullOrEmpty(Runtime)) return hash * 31 + Runtime.GetHashCode();
                if (!string.IsNullOrEmpty(InternalID)) return hash * 31 + InternalID.GetHashCode();
                return hash;
            }
        }
    }

    internal class AssetKeyComparer : IEqualityComparer<AssetKey>, IEqualityComparer<AssetKeySt>
    {
        public bool Equals(AssetKey x, AssetKey y)
        {
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public bool Equals(AssetKeySt x, AssetKeySt y) => x.Equals(y);

        public bool Equals(AssetKey x, AssetKeySt y)
        {
            if (x == null) return false;
            return x.Equals(y);
        }

        public int GetHashCode(AssetKey obj) => obj.GetHashCode();

        public int GetHashCode(AssetKeySt obj) => obj.GetHashCode();

        public static AssetKeyComparer Instance { get; } = new AssetKeyComparer();
    }
}