using CurrencyRateChangesApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace CurrencyRateChangesApp.Services
{
    public class ExchangeRateService
    {
        public List<ExchangeRate> GetAllByDate(string date = "20141231")
        {
            string url = "http://www.lb.lt/webservices/ExchangeRates/ExchangeRates.asmx/getExchangeRatesByDate?Date=" + date;

            XElement loadedXml = XElement.Load(url);
            IEnumerable<XElement> xmlElements = loadedXml.Elements();
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>();

            foreach (var element in xmlElements)
            {
                exchangeRates.Add(mapXmlToExchangeRate(element));
            }

            return exchangeRates;
        }

        private ExchangeRate mapXmlToExchangeRate(XElement element)
        {
            ExchangeRate exchangeRate = new ExchangeRate();

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";

            exchangeRate.Date = element.Element("date").Value;
            exchangeRate.Currency = element.Element("currency").Value;
            exchangeRate.Quantity = Convert.ToInt32(element.Element("quantity").Value);
            exchangeRate.Rate = Convert.ToDouble(element.Element("rate").Value, provider);
            exchangeRate.Unit = element.Element("unit").Value;

            return exchangeRate;
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
