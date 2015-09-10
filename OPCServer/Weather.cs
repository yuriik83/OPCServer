using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OPCServer
{
    static class Weather
    {
        private static string city;
        private static string conditions;
        private static string[] forecast;

        public static string[] Forecast
        {
            get { return forecast; }
            set { forecast = value; }
        }


        public static string Conditions
        {
            get { return conditions; }
            set { conditions = value; }
        }

        public static string City
        {
            get { return city; }
            set { city = value; }
        }

        public static void getCurrentConditions()
        {
            XmlDocument xmlConditions = new XmlDocument();
            xmlConditions.Load(string.Format("http://api.wunderground.com/api/3c28de0268c1471e/conditions/lang:PL/q/Poland/{0}.xml", city));
            XmlNode conditionsNode = xmlConditions.SelectSingleNode("/response/current_observation");
            conditions = String.Format("{0} temp. {1} °C. Wiatr {2} z pred. {3} km/h", conditionsNode.SelectSingleNode("weather").InnerText, 
                conditionsNode.SelectSingleNode("temp_c").InnerText, conditionsNode.SelectSingleNode("wind_dir").InnerText, 
                conditionsNode.SelectSingleNode("wind_mph").InnerText);
        }

        public static void conditionsPrintToFile()
        {
            StreamWriter file = new StreamWriter("logsConditions.txt", true);
            file.WriteLine("{0} - {1}",DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), Conditions);
            file.Close();
        }

        public static void forecastPrintToFile()
        {
            StreamWriter file = new StreamWriter("logsForecast.txt", true);
            file.WriteLine("Prognoza zauktalizowana {0}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            for (int i = 0; i < 8; i++)
            {
                file.WriteLine("\t{0}", forecast[i]);
            }
            file.Close();
        }

        public static void getForecast()
        {
            XmlDocument xmlForecast = new XmlDocument();
            xmlForecast.Load(string.Format("http://api.wunderground.com/api/3c28de0268c1471e/hourly/lang:PL/q/Poland/{0}.xml", city));
            XmlNodeList nodes = xmlForecast.SelectNodes("/response/hourly_forecast/forecast");
            int i = 0;
            forecast = new string[8];
            foreach(XmlNode item in nodes)
            {
                forecast[i] = String.Format("{0}:00 - {1} temp. {2} °C. Wiatr {3} z pred {4} km/h", DateTime.Now.AddHours(i + 1).ToString("dd-MM-yyyy HH"), 
                    item.SelectSingleNode("wx").InnerText, item.SelectSingleNode("temp/metric").InnerText, item.SelectSingleNode("wdir/dir").InnerText, 
                    item.SelectSingleNode("wspd/metric").InnerText);
                i++;
                if (i == 8)
                    break;
            }

        }
    }
}
