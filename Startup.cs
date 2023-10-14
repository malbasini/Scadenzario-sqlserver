using System;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scadenze.Customizations.Identity;
using Scadenze.Customizations.ModelBinders;
using Scadenze.Models.Options;
using Scadenze.Models.Services.Application;
using Scadenze.Models.Services.Application.Beneficiari;
using Scadenze.Models.Services.Application.Scadenze;
using Scadenze.Models.Services.Infrastructure;


namespace Scadenze
{
    public class Startup
    {
       
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReCaptcha(options =>
            {
                options.SecretKey = Configuration.GetValue<String>("ReCaptcha:SecretKey");
                options.SiteKey = Configuration.GetValue<String>("ReCaptcha:SiteKey");
                options.Version = ReCaptchaVersion.V2;
            });
            String connectionString = Configuration.GetValue<String>("ConnectionStrings:Default");
            services.AddResponseCaching();
            services.AddMvc(Options=>{
                Options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
                var homeProfile = new CacheProfile();
                homeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
                homeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
                homeProfile.VaryByQueryKeys= new string[]{"Page"};
                Options.CacheProfiles.Add("Home",homeProfile);
                Configuration.Bind("ResponseCache:Home", homeProfile);
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddTransient<IScadenzeService,EFCoreScadenzaService>();
            services.AddTransient<IBeneficiariService,EFCoreBeneficiarioService>();
            services.AddTransient<IRicevuteService,EFCoreRicevutaService>();
            services.AddTransient<ICachedScadenzaService,MemoryCacheScadenzaService>();
            services.AddTransient<ICachedBeneficiarioService, MemoryCacheBeneficiarioService>();
            IServiceCollection serviceCollection = services.AddDbContext<ApplicationDbContext>(
                   optionsBuilder => optionsBuilder.UseSqlServer(connectionString));
            
            services.AddDefaultIdentity<IdentityUser>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 3;
                //Conferma Account
                options.SignIn.RequireConfirmedAccount = true;
                //Blocco dell'account
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddPasswordValidator<CommonPasswordValidator<IdentityUser>>();
            services.AddSingleton<IEmailSender, MailKitEmailSender>();
            //Options
            services.Configure<SmtpOptions>(Configuration.GetSection("Smtp"));
            services.Configure<MemoryCacheOptions>(Configuration.GetSection("MemoryCache"));
            services.Configure<ScadenzeOptions>(Configuration.GetSection("Scadenze"));
            services.Configure<BeneficiariOptions>(Configuration.GetSection("Beneficiari"));
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration.GetValue<String>("LoginFacebook:AppId");
                facebookOptions.AppSecret = Configuration.GetValue<String>("LoginFacebook:AppSecret");
            });
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration.GetValue<String>("LoginGoogle:ClientId");
                googleOptions.ClientSecret = Configuration.GetValue<String>("LoginGoogle:ClientSecret");
            });
            services.AddAuthentication().AddTwitter(t =>
            {
                t.ConsumerKey = Configuration.GetValue<String>("LoginTwitter:key");
                t.ConsumerSecret = Configuration.GetValue<String>("LoginTwitter:secret");
            });
            services.AddAuthentication().AddYahoo(YahooOptions =>
            {
                YahooOptions.ClientId = Configuration.GetValue<String>("LoginYahoo:clientid");
                YahooOptions.ClientSecret = Configuration.GetValue<String>("LoginYahoo:clientsecret");
            });
            services.AddAuthentication().AddMicrosoftAccount(microsoft =>
            {
                microsoft.ClientId = Configuration.GetValue<String>("LoginMicrosoft:id");
                microsoft.ClientSecret = Configuration.GetValue<String>("LoginMicrosoft:key");
            });
            services.AddAuthentication().AddGitHub(git =>
            {
                git.ClientId = Configuration.GetValue<String>("Logingithub:clientid");
                git.ClientSecret = Configuration.GetValue<String>("Logingithub:clientsecret");
                git.CallbackPath = "/signin-github";
                git.Scope.Add("read:user");
            });
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
                app.UseExceptionHandler("/Error");
            }
            SeedData.Initialize(app.ApplicationServices);
            app.UseStaticFiles();
            //Endpoint routing Middleware
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //EndpointMiddleware
            var supportedCultures = new string[] { "it-IT", "fr-FR" };
            app.UseRequestLocalization(options =>
                 options.AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .SetDefaultCulture("it-IT")
            );
            /*--Una route viene identificata da un nome, in questo caso default
            e tre frammenti controller,action e id. Grazie a questa Route il
            meccanismo di routing è in grado di soddisfare le richieste. Facciamo un esempio
            Supponiamo che arrivi la seguente richiesta HTTP /Scadenze/Detail/5
            Grazie a questo template il meccanismo di routing sa che deve andare a chiamare
            un controller chiamato Scadenze, la cui action è Detail e a cui passa
            l'id 5.*/
            app.UseResponseCaching();
            app.UseEndpoints(routeBuilder => {
                routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routeBuilder.MapRazorPages();
            });

            
        }
    }
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                // auto migration
                context.Database.Migrate();
            }
        }

    }
}
