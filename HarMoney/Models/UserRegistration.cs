﻿namespace HarMoney.Models
{
    public class UserRegistration : User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}