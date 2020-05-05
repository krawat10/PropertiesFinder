using System;
using System.Collections.Generic;

namespace Models.Comparers
{
    public class PropertyPriceEqualityComparer : IEqualityComparer<PropertyPrice>
    {
        public bool Equals(PropertyPrice x, PropertyPrice y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.TotalGrossPrice == y.TotalGrossPrice &&
                   x.PricePerMeter == y.PricePerMeter &&
                   x.ResidentalRent == y.ResidentalRent &&
                   x.NegotiablePrice == y.NegotiablePrice;
        }

        public int GetHashCode(PropertyPrice obj)
        {
            return HashCode.Combine(obj.TotalGrossPrice, obj.PricePerMeter, obj.ResidentalRent, obj.NegotiablePrice);
        }
    }
}