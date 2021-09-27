using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            var rfcSettings = new Dictionary<string, string>();
            config.GetSection("saprfc").Bind(rfcSettings);
           
            var documentId = ParseDocumentIdSettings(config);


            using var context = new RfcContext(new ConnectionBuilder(rfcSettings)
                .WithStartProgramCallback(StartProgram)
                .Build());

            await 
                (from documentData in context.DocumentGetDetail(documentId)
                from sapftpDest in context.StartRegServer("sapftp")
                from saphttpDest in context.StartRegServer("saphttp")
                select(documentData,sapftpDest, saphttpDest))
                .Match(r =>
                    {
                        var d = r.documentData;
                        Console.WriteLine($"Document : {d.Id.Type}/{d.Id.Number}/{d.Id.Part}/{d.Id.Version}, Status: {d.Status}, Description: {d.Description}");
                        d.Files.Iter(async f =>
                        {
                            Console.WriteLine($"  File : {f.FileName}/{f.OriginalType}/{f.ApplicationId}");
                            await CheckoutFile(context, f, r.sapftpDest, r.saphttpDest);
                        });
                    },
                    l =>
                    {
                        Console.Error.WriteLine(l.Message);
                    });

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

        private static async Task CheckoutFile(IRfcContext context, DocumentFileInfo fileInfo, string sapftpDest, string saphttpDest)
        {
            var checkoutDir = AppDomain.CurrentDomain.BaseDirectory;
            await context.DocumentCheckoutFile(fileInfo, checkoutDir, sapftpDest, saphttpDest)
                .Match(
                    r => Console.WriteLine(
                        $"    checked out to '{Path.Combine(checkoutDir, fileInfo.FileName)}'"),
                    l => Console.WriteLine($"    checkout failed:'{l.Message}'"));

        }

        private static RfcErrorInfo StartProgram(string command)
        {
            var programParts = command.Split(' ');
            var arguments = command.Replace(programParts[0], "");

            try
            {
                var pStart = new ProcessStartInfo(
                    AppDomain.CurrentDomain.BaseDirectory + @"\" + programParts[0] + ".exe",
                    arguments.TrimStart()) {UseShellExecute = true};
                
                var p = Process.Start(pStart);

                return RfcErrorInfo.Ok();
            }
            catch (Exception ex)
            {
                return new RfcErrorInfo(RfcRc.RFC_EXTERNAL_FAILURE, RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE,
                    "", ex.Message, "", "", "", "", "", "", "");
            }
        }
    }
}
