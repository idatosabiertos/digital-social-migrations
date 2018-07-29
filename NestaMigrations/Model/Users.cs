using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class Users
    {
        public int id { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public bool showEmail { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string bio { get; set; }
        public string location { get; set; }
        public string cityName { get; set; }
        public string countryName { get; set; }
        public string jobTitle { get; set; }
        public string company{ get; set; }
        public string facebookUID { get; set; }
        public string googleUID { get; set; }
        public string twitterUID { get; set; }
        public string profileURL { get; set; }
        public string gitHubUID { get; set; }
        public string profilePic { get; set; }
        public bool isAdmin { get; set; }
        public bool isSuperAdmin { get; set; }
        public bool isDisabled { get; set; }
        public string role { get; set; }
    }

    public class UserRoles {
        public static readonly string User = "user";
        public static readonly string SysAdmin = "sys-admin";
        public static readonly string ComunityAdmin = "community-admin";
        public static readonly string EditorialAdmin = "editorial-admin";
    }
}
