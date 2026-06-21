namespace UtilityManagement.Infrastructure.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using UtilityManagement.Data;
    using UtilityManagement.Infrastructure.Interfaces;
    using UtilityManagement.Models;

    public class PermissionRepository
        : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SavePermissions(
            string userId,
            List<TblUserPermission> permissions)
        {
            var oldPermissions = _context.TblUserPermission
                .Where(x => x.UserId.Equals(userId));

            _context.TblUserPermission.RemoveRange(oldPermissions);

            await _context.TblUserPermission.AddRangeAsync(permissions);

            await _context.SaveChangesAsync();
        }

        public async Task<List<TblUserPermission>>
            GetPermissionsByUser(string userId)
        {
            return await _context.TblUserPermission
                .Where(x => x.UserId.Equals(userId))
                .ToListAsync();
        }

        public async Task<bool> HasPermission(
            string userId,
            string module,
            string menu,
            string action)
        {
            return await
            (
                from up in _context.TblUserPermission
                join m in _context.TblModule
                    on up.ModuleId equals m.ModuleId

                join me in _context.TblMenu
                    on up.MenuId equals me.MenuId

                join a in _context.TblPermissionAction
                    on up.ActionId equals a.ActionId

                where up.UserId == userId
                    && m.ModuleName == module
                    && me.MenuName == menu
                    && a.ActionName == action
                    && up.IsAllowed

                select up

            ).AnyAsync();
        }

        //public Task SavePermissions(int userId, List<TblUserPermissions> permissions)
        //{
        //    throw new NotImplementedException();
        //}

        Task<List<TblUserPermission>> IPermissionRepository.GetPermissionsByUser(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
