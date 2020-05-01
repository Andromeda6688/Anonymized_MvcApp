using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcApplication1.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("WebSiteDBConnectionString")
        {
        }

        public DbSet<User> Users { get; set; }
    }

    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "E-mail / Логин")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня на этом компьютере")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Email / Логин")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Имя и Фамилия")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Не менее 6 и не более 100 знаков", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Аккаунт активен?")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Админские права")]
        public bool IsAdmin { get; set; }

    }

    public class ManageModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Email / Логин")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Имя и Фамилия")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Аккаунт активен?")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Админские права")]
        public bool IsAdmin { get; set; }
    }


    public class PasswordChangeModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [ReadOnly(true)]
        [Display(Name = "Email / Логин")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [StringLength(100, ErrorMessage = "Не менее 6 и не более 100 знаков", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }


  
}
