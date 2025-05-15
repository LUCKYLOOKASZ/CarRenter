using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema; // Dodaj ten using

namespace CarRenter.Auth.Models
{
    [Table("titiUsers")] // Dodaj ten atrybut
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User";
    }
}
