using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.SAPDms.Functions;
using Dbosoft.YaNco;
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace Dbosoft.SAPDms.Apps
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurationBuilder =
                new ConfigurationBuilder();

            configurationBuilder.AddUserSecrets<Program>();
            configurationBuilder.AddCommandLine(args);
            var config = configurationBuilder.Build();

            try
            {
                var rfcSettings = new Dictionary<string, string>();
                config.GetSection("saprfc").Bind(rfcSettings);
               
                var documentId = ParseDocumentIdSettings(config);

                var runtime = new RfcRuntime();
                var connectionFunc = fun( () =>Connection.Create(rfcSettings, runtime));

                using var context = new RfcContext(connectionFunc);

                var documentResult = await context.DocumentGetDetail(documentId);

                documentResult
                    .Match(r =>
                        {
                            Console.WriteLine($"Document : {r.Id.Type}/{r.Id.Number}/{r.Id.Part}/{r.Id.Version}, Status: {r.Status}, Description: {r.Description}");
                        },
                        l =>
                        {
                            Console.Error.WriteLine(l.Message);
                        });

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static DocumentId ParseDocumentIdSettings(IConfiguration config)
        {
            var documentSettings = config.GetSection("doc");
            var documentType = documentSettings["type"] ?? throw new ArgumentException("Missing argument /doc:type");
            var documentNumber = documentSettings["number"] ?? throw new ArgumentException("Missing argument /doc:number");
            var documentVersion = documentSettings["version"] ?? "00";
            var documentPart = documentSettings["part"] ?? "000";
            
            return new DocumentId(documentType, documentNumber, documentPart, documentVersion);
        }
    }
}
