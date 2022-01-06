using CurrencyRateChangesApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace CurrencyRateChangesApp.Services
{
    public class ExchangeRateService
    {
        private readonly string apiUrl = "http://www.lb.lt/webservices/ExchangeRates/ExchangeRates.asmx/getExchangeRatesByDate?Date=";

        public List<ExchangeRate> GetAllByDate(string date)
        {
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>();

            var xmlData = GetXml(apiUrl + date);

            foreach (var element in xmlData)
            {
                exchangeRates.Add(mapXmlToExchangeRate(element));
            }

            return exchangeRates;
        }

        private IEnumerable<XElement> GetXml(string url)
        {
            XElement loadedXml = XElement.Load(url);

            return loadedXml.Elements();
        }

        private ExchangeRate mapXmlToExchangeRate(XElement element)
        {
            ExchangeRate exchangeRate = new ExchangeRate();

            exchangeRate.Date = element.Element("date").Value;
            exchangeRate.Currency = element.Element("currency").Value;
            exchangeRate.Quantity = Convert.ToInt32(element.Element("quantity").Value);
            exchangeRate.Rate = ConvertToDouble(element.Element("rate").Value);
            exchangeRate.Unit = element.Element("unit").Value;

            return exchangeRate;
        }

        private double ConvertToDouble(string number)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";

            return Convert.ToDouble(number, provider);
        }

        public List<RateChange> GetRateChanges(string date)
        {
            DateTime prevDateObj = DateTime.Parse(date);
            prevDateObj = prevDateObj.AddDays(-1);

            string prevDate = prevDateObj.ToString("yyyyMMdd");

            string currDate = date.Replace("-", "");

            List<ExchangeRate> currentRates = GetAllByDate(currDate);
            List<ExchangeRate> prevRates = GetAllByDate(prevDate);

            List<RateChange> rateChanges = new List<RateChange>();

            for (int i = 0; i < currentRates.Count; i++)
            {
                RateChange rateChange = new RateChange();
                rateChange.Date = currentRates[i].Date;
                rateChange.Currency = currentRates[i].Currency;
                rateChange.Quantity = currentRates[i].Quantity;
                rateChange.Rate = currentRates[i].Rate;
                rateChange.Unit = currentRates[i].Unit;

                rateChange.Change = currentRates[i].Rate - prevRates[i].Rate;

                rateChanges.Add(rateChange);
            }

            return rateChanges.OrderByDescending(x => x.Change).ToList();
        }
    }
}
