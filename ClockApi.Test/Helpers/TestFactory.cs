using ClockApi.Common.Model;
using ClockApi.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ClockApi.Test.Helpers
{
    public class TestFactory
    {
        public static RecordEntity GetRecordEntity()
        {
            return new RecordEntity
            {
                DateTimeRecord = DateTime.UtcNow,
                ETag = "*",
                IsConsolidated = false,
                Id = 1,
                Type = 0,
                PartitionKey = "REC",
                RowKey = Guid.NewGuid().ToString(),


            };
        }
        public static DefaultHttpRequest CreateHttpRequest(Guid recordId, Record recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamForString(request),
                Path = $"/{recordId}"
            };
        }
        public static DefaultHttpRequest CreateHttpRequest(Guid recordId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{recordId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Record recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamForString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Record GetRecordRequest()
        {
            return new Record
            {
                DateTimeRecord = DateTime.UtcNow,
                IsConsolidated = false,
                Id = 1,
                Type = 0

            };
        }

        public static Stream GenerateStreamForString(string stringToconvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToconvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        ///Modificar dependiendo del proyecto 
        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null logger");
            }
            return logger;
        }
    }
}