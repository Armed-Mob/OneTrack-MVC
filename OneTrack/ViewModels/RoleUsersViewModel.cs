using Microsoft.AspNetCore.Identity;

namespace OneTrack.ViewModels
{
    public class RoleUsersViewModel
    {
        public string RoleName { get; set; }
        public List<IdentityUser> Users { get; set; }
    }
}
