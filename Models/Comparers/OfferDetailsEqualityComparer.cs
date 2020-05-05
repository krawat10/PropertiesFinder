using System;
using System.Collections.Generic;

namespace Models.Comparers
{
    public class OfferDetailsEqualityComparer : IEqualityComparer<OfferDetails>
    {
        private static readonly SellerContactEqualityComparer SellerContactEqualityComparer = new SellerContactEqualityComparer();

        public bool Equals(OfferDetails x, OfferDetails y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Url == y.Url &&
                   x.CreationDateTime.Equals(y.CreationDateTime) &&
                   x.OfferKind == y.OfferKind &&
                   SellerContactEqualityComparer.Equals(x.SellerContact, y.SellerContact);
        }

        public int GetHashCode(OfferDetails obj)
        {
            return HashCode.Combine(obj.Url, obj.CreationDateTime, (int) obj.OfferKind, obj.SellerContact);
        }
    }
}