using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace UtilityManagement.Models
{
    public class TblUserPermission
    {
        public int UserPermissionId { get; set; }

        public string UserId { get; set; } = null!;

        public int ModuleId { get; set; }
        public int MenuId { get; set; }
        public int ActionId { get; set; }

        public bool IsAllowed { get; set; }


        // 👇 Navigation Properties (IMPORTANT)
        public TblModule? Module { get; set; }
        public TblMenu? Menu { get; set; }
        public TblPermissionAction? Action { get; set; }
    }
}
