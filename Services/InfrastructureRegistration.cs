//namespace UtilityManagement.Services
//{
//    public class PersistenceRegistration
//    {
//    }
//}


using UtilityManagement.Infrastructure.Interfaces;
using UtilityManagement.Infrastructure.Repositories;

namespace UtilityManagement.InfrastructureRegistration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddProjectServices(
            this IServiceCollection services)
        {
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            return services;
        }
    }
}