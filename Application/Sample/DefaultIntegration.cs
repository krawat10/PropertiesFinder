using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Interfaces;
using Models;
using Utilities.Extensions;

namespace Application.Sample
{
    public class DefaultIntegration : IWebSiteIntegration
    {
        private static Regex _urlRegex = new Regex(@"\'(?<url>https://[a-zA-Z0-9~_.,/-]+)\'");
        private static Regex _priceRegex = new Regex(@"Cena: (?<price>[0-9 ]+|inf\. u dewelopera)");
        private List<string> _offerTypeTexts = new List<string>();

        public DefaultIntegration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "https://www.oferty.net/mieszkania/szukaj",
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

            foreach (var city in Enum.GetNames(typeof(PolishCity)).Take(5))
            {
                var uri = new Uri(WebPage.Url)
                    .AddParameter("ps[type]", "1")
                    .AddParameter("ps[location][type]", "1")
                    .AddParameter("ps[location][text]", city)
                    .AddParameter("ps[transaction]", "1");

                var doc = web.Load(uri);

                var offerCollection = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'property')]");

                foreach (var offer in offerCollection)
                {
                    var offerLink = offer.GetAttributeValue("onclick", String.Empty);

                    if (_urlRegex.IsMatch(offerLink))
                    {
                        var url = _urlRegex.Match(offerLink).Groups["url"].Value;
                        var htmlNode = new HtmlWeb().Load(url).DocumentNode;

                        if (IsValidOffer(htmlNode))
                        {
                            entries.Add(new Entry
                            {
                                PropertyPrice = CreatePropertyPrice(htmlNode),
                                PropertyAddress = PropertyAddress(htmlNode, city),
                                OfferDetails = CreateOfferDetail(htmlNode, url),
                                PropertyDetails = CreatePropertyDetails(htmlNode),
                                PropertyFeatures = CreatePropertyFeatures(htmlNode),
                                RawDescription = CreateDescription(htmlNode)
                            });
                        }
                    }
                }
            }


            return new Dump {Entries = entries};
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
                IsPrimaryMarket = GetCustomOfferDetailValue(htmlNode, "Rynek pierwotny:").StringToNullableBool("Tak", "Nie")
            };

            // case "Materiał budowlany:": break;
            // case "Stan nieruchomości:": break;
            // case "Wysokość pomieszczeń:": break;
            // case "Typ kuchni:": break;
            // case "Liczba łazienek:": break;
            // case "Czy łazienka z WC:": break;
            // case "Liczba sypialni:": break;
            // case "Czy zamknięty obiekt:": break;
            // case "Powierzchnia mieszkalna:": break;
            // case "Forma własności:": break;
            // case "Dostępne od:":
            // break;
            // case "Na biuro:": break;
            // case "Ogłoszeniodawca posiada tą ofertę na wyłączność": break;
            // case "Liczba miejsc parkingowych:": break;
            // case "Ogrzewanie:": break;
            // case "Miejsca parkingowe:": break;
            // case "Domofon:": break;
            // case "Powierzchnia balkonu:": break;
            // case "Telefon:": break;
            // case "Liczba lini telefonicznych:": break;
            // case "Balkon:": break;
            // case "Gaz:": break;
            // case "Rynek pierwotny:": break;
            // case "Woda:": break;
            // case "Taras:": break;
            // case "Powierzchnia tarasu:": break;
            // case "Stan budynku:": break;
            // case "Meble:": break;
            // case "Liczba wind w budynku:": break;
            // case "Czy kuchnia jest umeblowana:": break;
            // case "Klimatyzacja:": break;
            // case "Deweloper:": break;
            // case "Inwestycja:": break;
            // case "Lokalizacja:": break;
            // case "Powierzchnia loggia:": break;
            // case "Powierzchnia ogródka:": break;
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
            var priceNode = htmlNode.SelectNodes("//div[@class='header']/h3");

            return priceNode.Any();
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

            if (floorNumberText.Contains("parter")) propertyDetails.FloorNumber = 0;
            else propertyDetails.FloorNumber = floorNumberText.FindInteger();

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
            var priceText = _priceRegex.Match(priceNode).Groups["price"].Value;

            if (priceText.Contains("inf. u dewelopera"))
            {
                propertyPrice.NegotiablePrice = true;
            }
            else
            {
                propertyPrice.TotalGrossPrice = priceText.RemoveWhitespaces().FindDecimal();
            }

            propertyPrice.PricePerMeter = GetCustomOfferDetailValue(htmlNode, "Cena za m&sup2;:").FindDecimal();
            // propertyPrice.ResidentalRent = GetCustomOfferDetailValue(htmlNode,); // todo

            return propertyPrice;
        }

        private static string CreateDescription(HtmlNode htmlNode)
        {
            return htmlNode
                .SelectSingleNode("//div[@id='description']").InnerText;
        }

        private OfferDetails CreateOfferDetail(HtmlNode htmlNode, string url)
        {
            var sellerContactName = htmlNode
                .SelectSingleNode(@"//span[@class='propertyName' or @class='propertyCompanyName']")
                .InnerText;

            var sellerContact = htmlNode
                .SelectSingleNode("//span[@class='visible-contact']")
                .InnerText
                .RemoveWhitespaces();

            var offerTypeText = htmlNode
                .SelectSingleNode("//div[@class='header']/span/a")
                .InnerText;

            _offerTypeTexts.Add(offerTypeText);

            var creationDateTime = htmlNode
                .SelectNodes("//div[@class='baseParam']/div")
                .FirstOrDefault(node => node.Element("b")?.InnerText == "Data dodania:")
                .InnerText
                .FindDate();

            var lastUpdateDateTime = htmlNode
                .SelectNodes("//div[@class='baseParam']/div")
                .FirstOrDefault(node => node.Element("b")?.InnerText == "Data aktualizacji:")
                .InnerText
                .FindDate();


            return new OfferDetails
            {
                CreationDateTime = creationDateTime,
                LastUpdateDateTime = lastUpdateDateTime,
                IsStillValid = true,
                Url = url,
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