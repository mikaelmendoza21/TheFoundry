using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiefOfTheFoundry.DataAccess;
using ChiefOfTheFoundry.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace foundry
{
    public class Startup
    {
        const string ApiCors = "FoundryApiCors";

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
            MtgSetAccessor mtgSetAccessor = new MtgSetAccessor(setServiceDbSettings);
            services.AddSingleton<IMtgSetAccessor>(s => mtgSetAccessor);

            CollectionDbSettings metaCardDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["MetaCardCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            MetaCardAccessor metaCardAccessor = new MetaCardAccessor(metaCardDbSettings);
            services.AddSingleton<IMetaCardAccessor>(s => metaCardAccessor);

            CollectionDbSettings cardDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["CardCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            MtgCardAccessor mtgCardAccessor = new MtgCardAccessor(cardDbSettings);
            services.AddSingleton<IMtgCardAccessor>(s => mtgCardAccessor);

            CollectionDbSettings cardConstructDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["CardConstructCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            CardConstructAccessor cardConstructAccessor = new CardConstructAccessor(cardConstructDbSettings);
            services.AddSingleton<ICardConstructAccessor>(s => cardConstructAccessor);

            CollectionDbSettings deckDbSettings = new CollectionDbSettings()
            {
                CollectionName = Configuration.GetSection("DbSettings")["DeckCollectionName"],
                ConnectionString = Configuration.GetSection("DbSettings")["ConnectionString"],
                DatabaseName = Configuration.GetSection("DbSettings")["DatabaseName"]
            };
            DeckAccessor deckAccessor = new DeckAccessor(cardDbSettings);
            services.AddSingleton<IDeckAccessor>(s => deckAccessor);

            SetManagerService setManagerService = new SetManagerService(metaCardAccessor, mtgSetAccessor);
            services.AddSingleton<ISetManagerService>(s => setManagerService);

            CardManagerService cardManagerService = new CardManagerService(metaCardAccessor, mtgSetAccessor, mtgCardAccessor, cardConstructAccessor);
            services.AddSingleton<ICardManagerService>(c => cardManagerService);

            CollectionManagerService collectionManagerService = new CollectionManagerService(metaCardAccessor, mtgSetAccessor, mtgCardAccessor, cardConstructAccessor, deckAccessor);
            services.AddSingleton<ICollectionManagerService>(c => collectionManagerService);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
