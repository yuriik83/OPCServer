using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace OPCServer
{
    class OpcNodeManager : BaseNodeManager, OPCServer.IWeatherStationMethods
    {

        private ushort TypenamespaceIndex { get; set; }

        private OPCServer.WeatherStationModel WeatherStation { get; set; }

        public override void Startup()
        {
            try
            {
                Console.WriteLine("Starting OpcNodeManager.");

                DefaultNamespaceIndex = AddNamespaceUri("OPCServer");
                TypenamespaceIndex = AddNamespaceUri(OPCServer.Namespaces.OPCServer);


                Console.WriteLine("Loading the OPCServer model.");
                ImportUaNodeset(System.Reflection.Assembly.GetEntryAssembly(), "opcserver.xml");

                Console.WriteLine("Create WeatherStation Node.");

                NodeId rootId = new NodeId("WeatherStation", DefaultNamespaceIndex);
                QualifiedName rootName = new QualifiedName("WeatherStation", DefaultNamespaceIndex);

                CreateObjectSettings settings = new CreateObjectSettings()
                {
                    ParentNodeId = UnifiedAutomation.UaBase.ObjectIds.ObjectsFolder,
                    ReferenceTypeId = UnifiedAutomation.UaBase.ReferenceTypeIds.Organizes,
                    RequestedNodeId = rootId,
                    BrowseName = rootName,
                    TypeDefinitionId = new NodeId(OPCServer.ObjectTypes.WeatherStationType, TypenamespaceIndex)
                };

                CreateObject(Server.DefaultRequestContext, settings);

                WeatherStation = new OPCServer.WeatherStationModel();

                Weather.City = "Krakow";
                Weather.getCurrentConditions();
                Weather.getForecast();
                Weather.conditionsPrintToFile();
                Weather.forecastPrintToFile();
                WeatherStation.Conditions = Weather.Conditions;
                WeatherStation.Forecast = Weather.Forecast;

                LinkModelToNode(rootId, WeatherStation, null, null, 500);

                Console.WriteLine("Make the OpcNodeManager the implementator of Methods defined on the Object.");

                WeatherStation.WeatherStationMethods = this;

                Timer t = new Timer(TimeSpan.FromMinutes(15).TotalMilliseconds); // set the time

                t.AutoReset = true;

                t.Elapsed += new System.Timers.ElapsedEventHandler(update);

                t.Start();
   
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to start OpcNodeManager " + e.Message);
            }
        }


        private void update(object sender, ElapsedEventArgs e)
        {
            Weather.getCurrentConditions();
            Weather.getForecast();
            Weather.conditionsPrintToFile();
            Weather.forecastPrintToFile();
            WeatherStation.Conditions = Weather.Conditions;
            WeatherStation.Forecast = Weather.Forecast;


        }

        public StatusCode getConditions(RequestContext context, OPCServer.WeatherStationModel model)
        {
            Console.WriteLine("Calling getConditions");
            lock (model)
            {
                Weather.getCurrentConditions();
                model.Conditions = Weather.Conditions;
                Weather.conditionsPrintToFile();
            }

            return StatusCodes.Good;
        }

        public StatusCode getForecast(RequestContext context, OPCServer.WeatherStationModel model)
        {
            Console.WriteLine("Calling getForecast");
            lock (model)
            {
                Weather.getForecast();
                model.Forecast = Weather.Forecast;
                Weather.forecastPrintToFile();
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Called when the node manager is stopped.
        /// </summary>
        public override void Shutdown()
        {
            try
            {
                Console.WriteLine("Stopping OpcNodeManager.");

                // TBD 

                base.Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to stop OpcNodeManager " + e.Message);
            }
        }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public OpcNodeManager(ServerManager server) : base(server)
        {
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TBD
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Fields
        #endregion
    }

}