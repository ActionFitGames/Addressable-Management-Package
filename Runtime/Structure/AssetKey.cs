
using System;
using ActionFit.Framework.Addressable;

internal readonly struct AssetKey : IAssetKey, IEquatable<AssetKey>
{
    public string Primary { get; }
    public string Runtime { get; }
    public string InternalID { get; }
    public Type Type { get; }

    public bool IsValid => !string.IsNullOrEmpty(Primary) ||
                           !string.IsNullOrEmpty(Runtime) ||
                           !string.IsNullOrEmpty(InternalID);

    internal AssetKey(string primary, string runtime, string internalId, Type type)
    {
        Primary = primary;
        Runtime = runtime;
        InternalID = internalId;
        Type = type;
    }

    public bool Equals(IAssetKey other)
    {
        if (other == null)
        {
            return false;
        }
        
        if (!string.IsNullOrEmpty(Primary) && !string.IsNullOrEmpty(other.Primary))
        {
            return string.Equals(Primary, other.Primary);
        }
        
        if (!string.IsNullOrEmpty(Runtime) && !string.IsNullOrEmpty(other.Runtime))
        {
            return string.Equals(Runtime, other.Runtime);
        }
        
        return string.Equals(InternalID, other.InternalID);
    }

    public bool Equals(AssetKey other)
    {
        if (!string.IsNullOrEmpty(Primary) && !string.IsNullOrEmpty(other.Primary))
        {
            return string.Equals(Primary, other.Primary);
        }

        if (!string.IsNullOrEmpty(Runtime) && !string.IsNullOrEmpty(other.Runtime))
        {
            return string.Equals(Runtime, other.Runtime);
        }
        
        return string.Equals(InternalID, other.InternalID);
    }

    public override bool Equals(object obj) => obj switch
    {
        IAssetKey key => Equals(key),
        _ => false
    };

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            if (!string.IsNullOrEmpty(Primary))
            {
                return hash * 31 + Primary.GetHashCode();
            }
            if (!string.IsNullOrEmpty(Runtime))
            {
                return hash * 31 + Runtime.GetHashCode();
            }
            return hash * 31 + (InternalID?.GetHashCode() ?? 0);
        }
    }

    public static bool operator ==(AssetKey left, AssetKey right) => left.Equals(right);
    public static bool operator !=(AssetKey left, AssetKey right) => !(left == right);
}