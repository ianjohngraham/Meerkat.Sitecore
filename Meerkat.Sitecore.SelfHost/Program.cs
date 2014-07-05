using System;
using System.ServiceModel;
using System.Data.Services;
using System.ServiceModel.Description;
using Meerkat.Sitecore.Service;

namespace Meerkat.Sitecore.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Please enter the URL (e.g. http://localhost):");
            var url = Console.ReadLine();
            Console.WriteLine("Please enter the Port (e.g. 8081):");
            var port = Console.ReadLine();

            Uri baseAddress = new Uri(url + ":" + port + "/ItemComparerService.svc");
            Uri[] baseAddresses = new Uri[] { baseAddress };
            // Step 2 of the hosting procedure: Create ServiceHost
            DataServiceHost selfHost = new DataServiceHost(typeof(ItemComparerService), baseAddresses);

            WebHttpBinding binding = new WebHttpBinding();
            selfHost.AddServiceEndpoint(typeof(IRequestHandler),
                                    binding, "WebServiceHost");

            try
            {
                DataServiceHost host = new DataServiceHost(typeof(ItemComparerService), baseAddresses);
                host.Open();
                Console.WriteLine("Service at " + baseAddress);
                Console.WriteLine("Press any key to stop the Service ");
                Console.Read();
                host.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}
