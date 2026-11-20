using Microsoft.Extensions.DependencyInjection;
using ShopTARpe25.ApplicationServices.Services;
using ShopTARpe25.Core.ServiceInterface;
using ShopTARpe25.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShopTARpe25.RealEstateTest.Macros;

namespace ShopTARpe25.RealEstateTest
{
    // Kõigi testklasside ühine baasklass. Iga test pärib selle, et ei peaks
    // igas testis uuesti DI konteinerit, andmebaasi jms üles seadma.
    // "abstract" tähendab, et sellest ei saa otse objekti teha, ainult pärida.
    public abstract class TestBase
    {
        // Siin hoitakse DI konteinerit (ServiceProvider), millest testid
        // küsivad teenuseid (nt RealEstateServices). "protected" = näevad
        // ainult baasklass ja sellest pärinevad testklassid.
        protected IServiceProvider ServiceProvider { get; set; }

        // Konstruktor käivitub automaatselt iga testi jaoks (xUnit loob
        // iga testi jaoks uue klassi objekti). Seega saab iga test oma
        // puhta konteineri ja tühja andmebaasi.
        protected TestBase()
        {
            // 1. Loo tühi teenuste kogum
            var services = new ServiceCollection();

            // 2. Registreeri kõik vajalikud teenused (vt SetupServices)
            SetupServices(services);

            // 3. Ehita kogumist valmis konteiner, millest saab teenuseid küsida
            ServiceProvider = services.BuildServiceProvider();
        }

        // Registreerib teenused, mida testid vajavad. On "virtual", et
        // konkreetne testklass saaks selle üle kirjutada ja lisada oma
        // teenuseid või asendada mõne näiteks mock'iga.
        public virtual void SetupServices(IServiceCollection services)
        {
            // Kui keegi küsib IRealEstateServices, anna talle RealEstateServices.
            // Scoped = üks objekt ühe scope'i kohta (testis praktiliselt üks).
            services.AddScoped<IRealEstateServices, RealEstateServices>();

            // Andmebaasi kontekst. UseInMemoryDatabase("TEST") loob andmebaasi
            // mälus, mitte päris SQL Serveris. See on kiire ja testid ei riku
            // päris andmeid.
            services.AddDbContext<ShopTARpe25Context>(x =>
            {
                x.UseInMemoryDatabase("TEST");

                // In-memory andmebaas ei toeta tehinguid (transaction) ja
                // viskaks muidu hoiatuse/vea. See rida ütleb: ignoreeri seda.
                x.ConfigureWarnings(b =>
                    b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            // Registreeri automaatselt kõik "macro" klassid (vt allpool)
            RegisterMacros(services);
        }

        // Koht ressursside vabastamiseks pärast testi. Praegu tühi, sest
        // midagi pole vaja sulgeda. NB! xUnit kutsub seda automaatselt välja
        // ainult siis, kui klass implementeerib IDisposable. Praegu see
        // klass seda ei tee, seega seda meetodit ei kutsuta kunagi.
        public void Dispose()
        {

        }

        // Lühike abimeetod teenuse küsimiseks. Testis saab kirjutada
        // Svc<IRealEstateServices>() selle asemel, et iga kord
        // ServiceProvider.GetService<...>() välja kirjutada.
        // GetService tagastab null, kui teenust pole registreeritud
        // (GetRequiredService viskaks selle asemel selge vea).
        protected T Svc<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        // Leiab automaatselt kõik klassid, mis implementeerivad IMacros
        // liidest, ja registreerib need DI-sse. Macro on tavaliselt abiklass,
        // mis teeb mõne korduva testi tegevuse (nt tee valmis test-kinnisvara).
        // Nii ei pea iga uut macrot käsitsi registreerima.
        private void RegisterMacros(IServiceCollection services)
        {
            // Liides, mida otsime
            var macroBaseType = typeof(IMacros);

            // Võta selle assembly kõik tüübid ja jäta alles ainult need, mis:
            // - implementeerivad IMacros
            // - pole liides ega abstract klass (neist ei saa objekti teha)
            var macros = macroBaseType.Assembly.GetTypes()
                .Where(t => macroBaseType.IsAssignableFrom(t)
                && !t.IsInterface && !t.IsAbstract);

            // Registreeri iga leitud macro Singletonina (üks objekt kogu
            // konteineri eluea jooksul)
            foreach (var macro in macros)
            {
                services.AddSingleton(macro);
            }
        }
    }
}