using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Interfaces;
using Models;
using Models.Entries;
using Utilities.Extensions;

namespace Application.Sample
{
    public class DefaultIntegration : IWebSiteIntegration
    {
        private static readonly Regex UrlRegex = new Regex(@"(?<url>https://[a-zA-Z0-9~_.,/-]+)");
        private static readonly Regex PriceRegex = new Regex(@"Cena: (?<price>[0-9 ]+|inf\. u dewelopera)");
        private static readonly List<string> OfferTypes = new List<string> {"pokoje", "mieszkania", "domy"};

        public DefaultIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://www.oferty.net",
                Name = "oferty.net",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = true,
                    HomeRental = false,
                    HouseSale = false,
                    HouseRental = false
                }
            };
        }

        public Dump GenerateDump()
        {
            var web = new HtmlWeb();
            var entries = new List<Entry>();


            foreach (var offerType in OfferTypes)
            {
                foreach (var city in Enum.GetNames(typeof(PolishCity)))
                {
                    var uri = new Uri($"{WebPage.Url}/{offerType}/szukaj")
                        .AddParameter("page", "1") // For the sake of testing, only one page from each city
                        .AddParameter("ps[type]", "1")
                        .AddParameter("ps[foreign_search]", "0")
                        .AddParameter("ps[location][type]", "1")
                        .AddParameter("ps[location][text]", city)
                        .AddParameter("ps[transaction]", "1");

                    var doc = web.Load(uri);

                    var offerCollection = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'property')]");

                    if (offerCollection != null)
                    {
                        foreach (var offer in offerCollection)
                        {
                            var offerLink = offer.GetAttributeValue("onclick", String.Empty);

                            if (UrlRegex.IsMatch(offerLink))
                            {
                                var url = UrlRegex.Match(offerLink).Groups["url"].Value;
                                var htmlNode = new HtmlWeb().Load(url).DocumentNode;

                                if (IsValidOffer(htmlNode))
                                {
                                    entries.Add(item: new Entry
                                    {
                                        PropertyPrice = CreatePropertyPrice(htmlNode),
                                        PropertyAddress = PropertyAddress(htmlNode, city),
                                        OfferDetails = CreateOfferDetail(htmlNode),
                                        PropertyDetails = CreatePropertyDetails(htmlNode),
                                        PropertyFeatures = CreatePropertyFeatures(htmlNode),
                                        RawDescription = CreateDescription(htmlNode)
                                    });
                                    Console.WriteLine($"[Processing]: {url}");
                                }
                            }
                        }
                    }
                }
            }


            return new Dump {Entries = entries, WebPage = WebPage, DateTime = DateTime.Now};
        }

        private PropertyFeatures CreatePropertyFeatures(HtmlNode htmlNode)
        {
            var propertyFeatures = new PropertyFeatures
            {
                HasElevator = GetCustomOfferDetailValue(htmlNode, "Winda:").StringToBool("tak", "nie"),
                HasBasementArea = GetCustomOfferDetailValue(htmlNode, "Piwnica:").StringToBool("tak", "nie"),
                HasBalcony = GetCustomOfferDetailValue(htmlNode, "Balkon:").StringToBool("Tak", "Nie"),
                BalconyArea = GetCustomOfferDetailValue(htmlNode, "Powierzchnia tarasu:").FindIntegerNullable(),
                GardenArea = GetCustomOfferDetailValue(htmlNode, "Powierzchnia ogródka").FindIntegerNullable(),
                ParkingPlaces = GetCustomOfferDetailValue(htmlNode, "Miejsca parkingowe:").FindIntegerNullable(),
                IsPrimaryMarket = GetCustomOfferDetailValue(htmlNode, "Rynek pierwotny:")
                    .StringToNullableBool("Tak", "Nie")
            };
            return propertyFeatures;
        }

        public bool? IsPrimaryMarket { get; set; }

        private PropertyAddress PropertyAddress(HtmlNode htmlNode, string city)
        {
            var title = htmlNode
                .SelectSingleNode("//div[@class='header']/h1")
                .InnerText;

            var propertyAddress = new PropertyAddress
            {
                City = Enum
                    .Parse<PolishCity>(city),
                StreetName = title
                    .Split(',')
                    .Last()
                    .RemoveWhitespaces(),
                District = GetCustomOfferDetailValue(htmlNode, "Województwo :\n")
                    .RemoveWhitespaces()
            };

            return propertyAddress;
        }

        private bool IsValidOffer(HtmlNode htmlNode)
        {
            var hasPrice = htmlNode.SelectSingleNode("//div[@class='header']/h3") != null;

            var isForeignOffer = htmlNode
                                     .SelectSingleNode(@"//span[@class='propertyName' or @class='propertyCompanyName']")
                                     .InnerText?.Equals("Współpraca zagraniczna") ??
                                 false;

            var hasPhoneNumber = htmlNode.SelectSingleNode("//span[@class='visible-contact']") != null;

            return hasPrice && !isForeignOffer && hasPhoneNumber;
        }

        private PropertyDetails CreatePropertyDetails(HtmlNode htmlNode)
        {
            var propertyDetails = new PropertyDetails
            {
                Area = GetCustomOfferDetailValue(htmlNode, "Powierzchnia użytkowa:").FindDecimal(),
                NumberOfRooms = GetCustomOfferDetailValue(htmlNode, "Liczba pokoi:").FindInteger(),
                BuldingType = GetCustomOfferDetailValue(htmlNode, "Typ budynku:").RemoveWhitespaces(),
                NumberOfFloors = GetCustomOfferDetailValue(htmlNode, "Liczba pięter:").FindInteger(1),
                YearOfConstruction = GetCustomOfferDetailValue(htmlNode, "Rok budowy:").FindIntegerNullable()
            };

            var floorNumberText = GetCustomOfferDetailValue(htmlNode, "Piętro:");

            if (!string.IsNullOrWhiteSpace(floorNumberText))
            {
                propertyDetails.FloorNumber = floorNumberText.Contains("parter") ? 0 : floorNumberText.FindInteger();
            }

            return propertyDetails;
        }

        private string GetCustomOfferDetailValue(HtmlNode htmlNode, string name)
        {
            var nameNodes = htmlNode.SelectNodes(@"//*[@id=""propertyLeft""]/div[3]/dl/dt");
            var nameNode = nameNodes.FirstOrDefault(node => node.InnerText == name);

            if (nameNode == null) return null;

            var valueNode = htmlNode
                .SelectNodes(@"//*[@id=""propertyLeft""]/div[3]/dl/dd")
                .ElementAt(nameNodes.IndexOf(nameNode));

            return valueNode.InnerText;
        }

        private PropertyPrice CreatePropertyPrice(HtmlNode htmlNode)
        {
            var propertyPrice = new PropertyPrice();

            var priceNode = htmlNode.SelectSingleNode("//div[@class='header']/h3").InnerText;
            var priceText = PriceRegex.Match(priceNode).Groups["price"].Value;

            if (priceText.Contains("inf. u dewelopera"))
            {
                propertyPrice.NegotiablePrice = true;
            }
            else
            {
                propertyPrice.TotalGrossPrice = priceText.RemoveWhitespaces().FindDecimal();
            }

            propertyPrice.PricePerMeter = GetCustomOfferDetailValue(htmlNode, "Cena za m&sup2;:").FindDecimalOrNull();
            // propertyPrice.ResidentalRent = GetCustomOfferDetailValue(htmlNode,); // todo

            return propertyPrice;
        }

        private static string CreateDescription(HtmlNode htmlNode)
        {
            return htmlNode
                .SelectSingleNode("//div[@id='description']").InnerText;
        }

        private OfferDetails CreateOfferDetail(HtmlNode htmlNode)
        {
            var sellerContactName = htmlNode
                .SelectSingleNode(@"//span[@class='propertyName' or @class='propertyCompanyName']")
                .InnerText;

            var offerType = htmlNode
                .SelectSingleNode(@"//div[@class='header']/span")
                .InnerText;

            OfferKind offerKind;

            if (offerType.Contains("wynajęcia")) offerKind = OfferKind.RENTAL;
            else offerKind = OfferKind.SALE;


            var sellerContact = htmlNode
                .SelectSingleNode("//span[@class='visible-contact']")
                .InnerText
                .RemoveWhitespaces();

            var lastUpdateDateTime = htmlNode
                .SelectNodes("//div[@class='baseParam']/div")
                .FirstOrDefault(node => node.Element("b")?.InnerText == "Data aktualizacji:")
                .InnerText
                .FindDate();

            var creationDateTime = htmlNode
                                       .SelectNodes("//div[@class='baseParam']/div")
                                       .FirstOrDefault(node => node.Element("b")?.InnerText == "Data dodania:")
                                       ?.InnerText.FindDate() ?? lastUpdateDateTime;


            var urlShort = htmlNode
                .SelectNodes("//div[@class='baseParam']/div")
                .FirstOrDefault(node => node.Element("b")?.InnerText == "Link do oferty:")
                ?.InnerText;

            return new OfferDetails
            {
                CreationDateTime = creationDateTime,
                LastUpdateDateTime = lastUpdateDateTime,
                IsStillValid = true,
                Url = UrlRegex.Match(urlShort).Groups["url"].Value,
                OfferKind = offerKind,
                SellerContact = new SellerContact
                {
                    Name = sellerContactName,
                    Telephone = sellerContact
                }
            };
        }

        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }
        public IEqualityComparer<Entry> EntriesComparer { get; }
    }
}