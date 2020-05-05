﻿namespace Models
{
    public class PropertyFeatures
    {

        /// <summary>
        /// Wielkość ogródka w metrach. "0" w przypadku gdy oferta nie zawiera ogródka
        /// "null" w przypadku braku danych
        /// </summary>
        public decimal? GardenArea { get; set; }

        /// <summary>
        /// Ilość balkonów. "0" w przypadku braku balkonu, "null" w przypadku braku danych
        /// </summary>
        public bool HasBalcony { get; set; }

        public int? BalconyArea { get; set; }
        /// Wielkość piwnicy w metrach. "0" w przypadku gdy oferta nie zawiera ogródka
        /// "null" w przypadku braku danych.
        /// Komórkę lokatorską można potraktować jak piwnicę.
        public bool HasBasementArea { get; set; }

        /// <summary>
        /// Ilość miejsc prakingowych naziemnych. "0" w przypadku braku miejsc, "null" w przypadku braku danych
        /// </summary>
        public int? OutdoorParkingPlaces { get; set; }
        /// <summary>
        /// Ilość miejsc prakingowych podziemnych/garażowych. "0" w przypadku braku miejsc, "null" w przypadku braku danych
        /// </summary>
        public int? IndoorParkingPlaces { get; set; }

        public bool HasElevator { get; set; }
        public int? ParkingPlaces { get; set; }
        public bool? IsPrimaryMarket { get; set; }
    }
}