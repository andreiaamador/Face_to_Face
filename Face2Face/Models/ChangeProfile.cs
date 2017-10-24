using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Face2Face.Models
{
    public class ChangeProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public byte[] Photo { get; set; }
        public string Nacionality { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NativeLanguage { get; set; }
        public string FluentLanguage { get; set; }
        public string InterestedLanguage { get; set; }
    }
}