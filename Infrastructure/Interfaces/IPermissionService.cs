using UtilityManagement.Models;

namespace UtilityManagement.Infrastructure.Interfaces
{
    public interface IPermissionRepository
    {
        Task SavePermissions(
            string userId,
            List<TblUserPermission> permissions);

        Task<List<TblUserPermission>> GetPermissionsByUser(
            string userId);

        Task<bool> HasPermission(
            string userId,
            string module,
            string menu,
            string action);
    }
}
