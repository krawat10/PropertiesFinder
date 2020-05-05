using System;
using System.Collections.Generic;

namespace Models.Comparers
{
    public class SellerContactEqualityComparer : IEqualityComparer<SellerContact>
    {
        public bool Equals(SellerContact x, SellerContact y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Telephone == y.Telephone &&
                   x.Name == y.Name;
        }

        public int GetHashCode(SellerContact obj)
        {
            return HashCode.Combine(obj.Telephone, obj.Name);
        }
    }
}