using System;
using System.Collections.Generic;
using Models.Entries;

namespace Models.Comparers
{
    public class PropertyFeaturesEqualityComparer : IEqualityComparer<PropertyFeatures>
    {
        public bool Equals(PropertyFeatures x, PropertyFeatures y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.GardenArea == y.GardenArea &&
                   x.HasBalcony == y.HasBalcony &&
                   x.BalconyArea == y.BalconyArea &&
                   x.HasBasementArea == y.HasBasementArea &&
                   x.OutdoorParkingPlaces == y.OutdoorParkingPlaces &&
                   x.IndoorParkingPlaces == y.IndoorParkingPlaces &&
                   x.HasElevator == y.HasElevator &&
                   x.ParkingPlaces == y.ParkingPlaces &&
                   x.IsPrimaryMarket == y.IsPrimaryMarket;
        }

        public int GetHashCode(PropertyFeatures obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.GardenArea);
            hashCode.Add(obj.HasBalcony);
            hashCode.Add(obj.BalconyArea);
            hashCode.Add(obj.HasBasementArea);
            hashCode.Add(obj.OutdoorParkingPlaces);
            hashCode.Add(obj.IndoorParkingPlaces);
            hashCode.Add(obj.HasElevator);
            hashCode.Add(obj.ParkingPlaces);
            hashCode.Add(obj.IsPrimaryMarket);
            return hashCode.ToHashCode();
        }
    }
}