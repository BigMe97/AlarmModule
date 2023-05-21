using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AlarmManager
{
    static class User
    {
        public static string Name;
        public static string Role;
        private static int permission;
        public static  event EventHandler PermissionChanged;

        public static int Permission
        {
            get { return permission; }
            set { permission = value;
                OnPermissionChanged(); }
        }
        private static void OnPermissionChanged()
        {
            PermissionChanged?.Invoke(User.permission, EventArgs.Empty);
            
        }
    }
}
