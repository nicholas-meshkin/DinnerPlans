namespace DinnerPlans.Shared.DTOs
{
    public class LocationSearchResponseDto
    {
        public Datum[] data { get; set; }
        public Meta meta { get; set; }
        

        public class Meta
        {
            public Pagination pagination { get; set; }
        }

        public class Pagination
        {
            public int start { get; set; }
            public int limit { get; set; }
            public int total { get; set; }
        }

        public class Datum
        {
            public string locationId { get; set; }
            public string storeNumber { get; set; }
            public string divisionNumber { get; set; }
            public string chain { get; set; }
            public Address address { get; set; }
            public Geolocation geolocation { get; set; }
            public string name { get; set; }
            public Hours hours { get; set; }
            public string phone { get; set; }
            public Department[] departments { get; set; }
        }

        public class Address
        {
            public string addressLine1 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zipCode { get; set; }
            public string county { get; set; }
        }

        public class Geolocation
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
            public string latLng { get; set; }
        }

        public class Hours
        {
            public string timezone { get; set; }
            public string gmtOffset { get; set; }
            public bool open24 { get; set; }
            public Day monday { get; set; }
            public Day tuesday { get; set; }
            public Day wednesday { get; set; }
            public Day thursday { get; set; }
            public Day friday { get; set; }
            public Day saturday { get; set; }
            public Day sunday { get; set; }
        }

        public class Day
        {
            public string open { get; set; }
            public string close { get; set; }
            public bool open24 { get; set; }
        }

        public class Department
        {
            public string departmentId { get; set; }
            public string name { get; set; }
            public Hours hours { get; set; }
            public string phone { get; set; }
            public Address address { get; set; }
            public Geolocation geolocation { get; set; }
            public bool offsite { get; set; }
        }

    }
}
