using System;

namespace PasswordHasher
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = "Admin@123";
            string salt = "$2a$11$RVP1S9G0S8T.m6/oM8W9S1."; // Custom salt for consistency
            string hash = BCrypt.Net.BCrypt.HashPassword(password, 11);
            Console.WriteLine(hash);
        }
    }
}
