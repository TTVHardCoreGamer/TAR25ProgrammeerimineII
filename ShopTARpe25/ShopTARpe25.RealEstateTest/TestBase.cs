using Microsoft.Extensions.DependencyInjection;
using ShopTARpe25.ApplicationServices.Services;
using ShopTARpe25.Core.ServiceInterface;
using ShopTARpe25.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShopTARpe25.RealEstateTest.Macros;

namespace ShopTARpe25.RealEstateTest
{
    // Kõigi testide ühine baasklass, kus seadistatakse DI konteiner ja andmebaas.
    // abstract = sellest ei saa objekti luua, saab ainult pärida.
    public abstract class TestBase
    {
        // DI konteiner, millest testid küsivad teenuseid, ilma et peaks neid käsitsi new-ga looma.
        // protected = nähtav ainult sellele klassile ja pärinevatele testklassidele.
        protected IServiceProvider ServiceProvider { get; set; }

        // Konstruktor: xUnit loob igale testile uue objekti, seega käivitub see enne iga testi
        // ja iga test saab puhta konteineri ning tühja andmebaasi.
        protected TestBase()
        {
            // Loob tühja teenuste kogumi, kuhu hakatakse teenuseid registreerima.
            var services = new ServiceCollection();

            // Registreerib teenused (eraldi meetodis, et seda saaks üle kirjutada).
            SetupServices(services);

            // Registreerimine on alles plaan, siin tehakse sellest töötav konteiner.
            ServiceProvider = services.BuildServiceProvider();
        }

        // Registreerib teenused, mida testid vajavad.
        // virtual = testklass saab üle kirjutada, et lisada teenuseid või panna mock.
        // Üle kirjutades kutsu ka base.SetupServices(services), muidu kaovad siinsed registreeringud.
        public virtual void SetupServices(IServiceCollection services)
        {
            // Küsides IRealEstateServices saad RealEstateServices objekti.
            // Scoped = üks objekt scope'i kohta, testis praktiliselt üks.
            services.AddScoped<IRealEstateServices, RealEstateServices>();

            // Registreerib andmebaasi konteksti, mida RealEstateServices vajab.
            services.AddDbContext<ShopTARpe25Context>(x =>
            {
                // Andmebaas mälus: kiire ja ei puuduta päris SQL Serverit.
                // Sama nimega ("TEST") andmebaasid jagavad andmeid.
                x.UseInMemoryDatabase("TEST");

                // In-memory ei toeta tehinguid ja annaks muidu vea; see rida ignoreerib hoiatust.
                x.ConfigureWarnings(b =>
                    b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            // Registreerib automaatselt kõik macro klassid (vt RegisterMacros).
            RegisterMacros(services);
        }

        // Ressursside vabastamiseks, praegu tühi.
        // xUnit kutsub seda ainult siis, kui klass implementeerib IDisposable, mis siin puudub,
        // seega seda ei kutsuta kunagi.
        public void Dispose()
        {

        }

        // Lühike abimeetod: Svc<IRealEstateServices>() asemel ServiceProvider.GetService<...>().
        protected T Svc<T>()
        {
            // Küsib konteinerist teenuse. GetService tagastab null, kui teenust pole;
            // GetRequiredService viskaks kohe selge vea.
            return ServiceProvider.GetService<T>();
        }

        // Leiab kõik IMacros-i implementeerivad klassid ja registreerib need DI-sse,
        // et uut macrot lisades ei peaks midagi käsitsi registreerima.
        // Macro = abiklass korduvaks testitegevuseks (nt test-kinnisvara loomine).
        private void RegisterMacros(IServiceCollection services)
        {
            // Liides, mille implementeerijaid otsitakse.
            var macroBaseType = typeof(IMacros);

            // Reflection: võtab assembly tüübid, jätab alles IMacros-i implementeerijad,
            // mis pole liides ega abstract (neist ei saa objekti luua).
            var macros = macroBaseType.Assembly.GetTypes()
                .Where(t => macroBaseType.IsAssignableFrom(t)
                && !t.IsInterface && !t.IsAbstract);

            // Registreerib iga leitud macro konkreetse klassina.
            foreach (var macro in macros)
            {
                // Singleton = üks objekt konteineri kogu eluea jooksul.
                services.AddSingleton(macro);
            }
        }
    }
}