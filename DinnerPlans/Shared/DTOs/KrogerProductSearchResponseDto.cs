namespace DinnerPlans.Shared.DTOs
{
    public class KrogerProductSearchResponseDto
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
            public string productId { get; set; }
            public string upc { get; set; }
            public object[] aisleLocations { get; set; }
            public string brand { get; set; }
            public string[] categories { get; set; }
            public string countryOrigin { get; set; }
            public string description { get; set; }
            public Image[] images { get; set; }
            public Item[] items { get; set; }
            public Iteminformation itemInformation { get; set; }
            public Temperature temperature { get; set; }
        }

        public class Iteminformation
        {
            //TODO figure out what can go here when on prod api
        }

        public class Temperature
        {
            public string indicator { get; set; }
            public bool heatSensitive { get; set; }
        }

        public class Image
        {
            public string perspective { get; set; }
            public Size[] sizes { get; set; }
            public bool featured { get; set; }
        }

        public class Size
        {
            public string size { get; set; }
            public string url { get; set; }
        }

        public class Item
        {
            public string itemId { get; set; }
            public bool favorite { get; set; }
            public Fulfillment fulfillment { get; set; }
            public Price? price { get; set; }
            public Price? nationalPrice { get; set; }
            public string size { get; set; }
            public string soldBy { get; set; }
            public Inventory inventory { get; set; }
        }

        public class Fulfillment
        {
            public bool curbside { get; set; }
            public bool delivery { get; set; }
            public bool inStore { get; set; }
            public bool shipToHome { get; set; }
        }

        public class Price
        {
            public float regular { get; set; }
            public float promo { get; set; }
            public float regularPerUnitEstimate { get; set; }
            public float promoPerUnitEstimate { get; set; }
        }

        public class Inventory
        {
            public string stockLevel { get; set; }
        }

    }
}
