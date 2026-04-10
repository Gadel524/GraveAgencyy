using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraveAgency.Helpers
{
    public class Session
    {
        public static int CurrentUserID { get; set; }
        public static string CurrentUsername { get; set; }
        public static int CurrentRoleID { get; set; }
        public static string CurrentRoleName { get; set; }
        public static int GriefLevel { get; set; } = 5; // Добавлено для уровня траура

        public static bool IsAuthenticated
        {
            get { return CurrentUserID > 0; }
        }

        public static bool IsAdmin
        {
            get { return CurrentRoleName == "Администратор"; }
        }

        public static bool IsNecromancer
        {
            get { return CurrentRoleName == "Главный некромант"; }
        }

        public static bool IsClient
        {
            get { return CurrentRoleName == "Клиент"; }
        }

        public static void Logout()
        {
            CurrentUserID = 0;
            CurrentUsername = "";
            CurrentRoleID = 0;
            CurrentRoleName = "";
            GriefLevel = 5;
        }
    }
}
