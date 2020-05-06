using System;
using System.Collections.Generic;

namespace Models.Comparers
{
    class EntryEqualityComparer : IEqualityComparer<Entry>
    {
        private static readonly OfferDetailsEqualityComparer OfferDetailsEqualityComparer = new OfferDetailsEqualityComparer();
        private static readonly PropertyPriceEqualityComparer PropertyPriceEqualityComparer = new PropertyPriceEqualityComparer();
        private static readonly PropertyDetailsEqualityComparer PropertyDetailsEqualityComparer = new PropertyDetailsEqualityComparer();
        private static readonly PropertyAddressEqualityComparer PropertyAddressEqualityComparer = new PropertyAddressEqualityComparer();
        private static readonly PropertyFeaturesEqualityComparer PropertyFeaturesEqualityComparer = new PropertyFeaturesEqualityComparer();

        public bool Equals(Entry x, Entry y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return OfferDetailsEqualityComparer.Equals(x.OfferDetails, y.OfferDetails) &&
                   PropertyPriceEqualityComparer.Equals(x.PropertyPrice, y.PropertyPrice) &&
                   PropertyDetailsEqualityComparer.Equals(x.PropertyDetails, y.PropertyDetails) &&
                   PropertyAddressEqualityComparer.Equals(x.PropertyAddress, y.PropertyAddress) &&
                   PropertyFeaturesEqualityComparer.Equals(x.PropertyFeatures, y.PropertyFeatures) &&
                   x.RawDescription == y.RawDescription;
        }

        public int GetHashCode(Entry obj)
        {
            return HashCode.Combine(
                OfferDetailsEqualityComparer.GetHashCode(obj.OfferDetails), 
                PropertyPriceEqualityComparer.GetHashCode(obj.PropertyPrice), 
                PropertyDetailsEqualityComparer.GetHashCode(obj.PropertyDetails), 
                PropertyAddressEqualityComparer.GetHashCode(obj.PropertyAddress),
                PropertyFeaturesEqualityComparer.GetHashCode(obj.PropertyFeatures), 
                obj.RawDescription);
        }
    }
}