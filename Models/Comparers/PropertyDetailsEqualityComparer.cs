using System;
using System.Collections.Generic;

namespace Models.Comparers
{
    public class PropertyDetailsEqualityComparer : IEqualityComparer<PropertyDetails>
    {
        public bool Equals(PropertyDetails x, PropertyDetails y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Area == y.Area &&
                   x.NumberOfRooms == y.NumberOfRooms &&
                   x.FloorNumber == y.FloorNumber &&
                   x.YearOfConstruction == y.YearOfConstruction &&
                   x.NumberOfFloors == y.NumberOfFloors &&
                   x.BuldingType == y.BuldingType;
        }

        public int GetHashCode(PropertyDetails obj)
        {
            return HashCode.Combine(obj.Area, obj.NumberOfRooms, obj.FloorNumber, obj.YearOfConstruction,
                obj.NumberOfFloors, obj.BuldingType);
        }
    }
}