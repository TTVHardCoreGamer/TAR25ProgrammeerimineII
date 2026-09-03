namespace ShopTARpe25.Data
{
    public static class DbInitializer
    {
        public static void Initializer(ShopTARpe25Context context)
        {
            // Ensure the database is created
            context.Database.EnsureCreated();
        }
    }
}
