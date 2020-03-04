using CoreWCF;
using CoreWCF.Configuration;
using CoreWCFChatApplication.Shared.Contract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CoreWCFServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServiceModel(builder =>
            {
                builder.AddService<ChatService>();
                builder.AddServiceEndpoint<ChatService, IChatService>(new BasicHttpBinding(), "/basichttp");
                builder.AddServiceEndpoint<ChatService, IChatService>(new NetTcpBinding(), "/nettcp");
            });
        }
    }
}
