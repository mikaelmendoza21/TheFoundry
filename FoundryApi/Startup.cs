using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FoundryApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Db Services - Dependency Injection
            CollectionDbSettings setServiceDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["SetCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            services.AddSingleton<IMtgSetAccessor>(s => new MtgSetAccessor(setServiceDbSettings));

            CollectionDbSettings metaCardDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["MetaCardCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            services.AddSingleton<IMetaCardAccessor>(s => new MetaCardAccessor(metaCardDbSettings));

            CollectionDbSettings cardDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["CardCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            services.AddSingleton<IMtgCardAccessor>(s => new MtgCardAccessor(cardDbSettings));

            CollectionDbSettings cardConstructDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["CardConstructCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            services.AddSingleton<ICardConstructAccessor>(s => new CardConstructAccessor(cardConstructDbSettings));

            CollectionDbSettings deckDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["DeckCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            services.AddSingleton<IDeckAccessor>(s => new DeckAccessor(cardDbSettings));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
