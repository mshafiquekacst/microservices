using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext context;

        public PlatformRepo( AppDbContext context )
        {
            this.context = context;
        }

        public void CreatePlatform(Platform plat)
        {
            if (plat == null)
            {
                throw new System.ArgumentNullException(nameof(plat));
            }
            context.Platforms.Add(plat);
            
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
           return context.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
             return context.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (context.SaveChanges() >= 0);
        }

        public void UpdatePlatform(Platform platform)
        {
            throw new NotImplementedException();
        }
    }
}
