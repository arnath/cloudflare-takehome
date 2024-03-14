using Cfth.Data;
using Cfth.Handlers;
using Microsoft.Extensions.Logging;
using Sydney.Core;

namespace Cfth;

public class Program
{
    public static async Task Main()
    {
        SydneyServiceConfig config = new SydneyServiceConfig(
            8080,
            returnExceptionMessagesInResponse: true);
        
        ILoggerFactory loggerFactory =
                LoggerFactory.Create(
                    (builder) => builder.AddConsole());

        IUrlRepository urlRepository = new InMemoryUrlRepository();
        ICodec codec = new Base62Codec();
        
        using (SydneyService service = new SydneyService(config, loggerFactory))
        {
            service.AddResourceHandler(
                "/urls",
                new UrlResourceHandler(loggerFactory, urlRepository));
            service.AddRestHandler(
                "/{encodedId}",
                new RedirectRestHandler(loggerFactory, urlRepository, codec));

            await service.StartAsync();
        }
    }
}
