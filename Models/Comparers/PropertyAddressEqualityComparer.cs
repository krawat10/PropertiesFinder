using System;
using System.Collections.Generic;

namespace Models.Comparers
{
    public class PropertyAddressEqualityComparer : IEqualityComparer<PropertyAddress>
    {
        public bool Equals(PropertyAddress x, PropertyAddress y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.City == y.City &&
                   x.District == y.District &&
                   x.StreetName == y.StreetName &&
                   x.DetailedAddress == y.DetailedAddress;
        }

        public int GetHashCode(PropertyAddress obj)
        {
            return HashCode.Combine((int) obj.City, obj.District, obj.StreetName, obj.DetailedAddress);
        }
    }
}