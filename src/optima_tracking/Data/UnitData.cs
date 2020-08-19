using System;

namespace optima_tracking.Data
{
    public class UnitData
    {
        public DateTime DateRecorded { get; set; }
        public int ApartmentNumber { get; set; }
        public int Rooms { get; set; }
        public int Baths { get; set; }
        public int SquareFootage { get; set; }
        public int Rent { get; set; }
        public DateTime? DateAvailable { get; set; }
    }
}
