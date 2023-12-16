using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DatabaseProject.Models
{
    public class Advisor
    {
        public int advisor_id { get; set; }
        public string name { get; set; }
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string email { get; set; }
        public string office { get; set; }
        public string password { get; set; }

        public Advisor()
        {
        }
        public Advisor(string name, string email, string password, string office)
        {
            this.name = name;
            this.email = email;
            this.password = password;
            this.office = office;
        }

        public Advisor(int advisor_id, string name, string email, string password, string office)
        {
            this.advisor_id = advisor_id;
            this.name = name;
            this.email = email;
            this.password = password;
            this.office = office;
        }
    }
}