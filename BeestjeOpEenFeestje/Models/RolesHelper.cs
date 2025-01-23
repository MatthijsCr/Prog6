using System.Data;

namespace BeestjeOpEenFeestje.Models
{
    public class RolesHelper
    {
        public string ConvertRoleToShownName(string convertname)
        {
            return convertname switch
            {
                "Employee" => "Werknemer",
                "Customer" => "Gebruiker",
                "Admin" => "Admin",
                _ => "Werknemer",
            };
        }
        public string ConvertRoleToRealName(string convertname)
        {
            return convertname switch
            {
                "Werknemer" => "Employee",
                "Gebruiker" => "Customer",
                "Admin" => "Admin",
                _ => "Employee",
            };
        }
    }
}
