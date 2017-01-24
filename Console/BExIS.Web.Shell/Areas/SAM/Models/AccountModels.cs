﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BExIS.Security.Entities.Subjects;
using BExIS.Modules.Sam.UI.Helpers;

namespace BExIS.Modules.Sam.UI.Models
{
    public class AccountRegisterModel
    {
        [Display(Name = "Username")]
        [RegularExpression("^[\\S]*$", ErrorMessage = "The Username must be without spaces.")]
        [Remote("ValidateUsername", "Account")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(64, ErrorMessage = "The Username must be {2} - {1} characters long.", MinimumLength = 3)]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [RegularExpression("^[\\S]*$", ErrorMessage = "The Password must not contain spaces.")]
        [Required]
        [StringLength(24, ErrorMessage = "The Password must be {2} - {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [global::System.Web.Mvc.Compare("Password", ErrorMessage = "The Password and Confirm Password do not match.")]
        [Display(Name = "Confirm Password")]
        [Required]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Full Name")]
        [Required]
        public string FullName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        [Remote("ValidateEmail", "Account")]
        [Required]
        [StringLength(250, ErrorMessage = "The Email Address must be {2} - {1} characters long.", MinimumLength = 5)]
        public string Email { get; set; }

        [Display(Name = "Security Question")]
        [Required]
        public long SecurityQuestion { get; set; }

        [Display(Name = "Security Answer")]
        [RegularExpression("^[^\\s]+(\\s+[^\\s]+)*", ErrorMessage = "The Security Answer must start and end with no space.")]
        [Required]
        [StringLength(50, ErrorMessage = "The Security Answer must be less than {1} characters long.")]
        public string SecurityAnswer { get; set; }

        public SecurityQuestionSelectListModel SecurityQuestionList { get; set; }

        public AuthenticatorSelectListModel AuthenticatorList { get; set; }

        [Display(Name = "Terms and Conditions")]
        [MustBeTrue(ErrorMessage = "You must agree to the Terms and Conditions before register.")]
        public bool TermsAndConditions { get; set; }

        public AccountRegisterModel()
        {
            SecurityQuestionList = new SecurityQuestionSelectListModel();
            AuthenticatorList = new AuthenticatorSelectListModel(true);
        }
    }

    public class MyAccountModel
    {
        [Display(Name = "User Id")]
        public long UserId { get; set; }

        public long AuthenticatorId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [RegularExpression("^[\\S]*$", ErrorMessage = "The Password must not contain spaces.")]
        [StringLength(24, ErrorMessage = "The Password must be {2} - {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [global::System.Web.Mvc.Compare("Password", ErrorMessage = "The Password and Confirm Password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Full Name")]
        [Required]
        public string FullName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        [Remote("ValidateEmail", "Account", AdditionalFields = "UserId")]
        [Required]
        [StringLength(250, ErrorMessage = "The email must be {2} - {1} characters long.", MinimumLength = 5)]
        public string Email { get; set; }

        [Display(Name = "Security Answer")]
        [RegularExpression("^[^\\s]+(\\s+[^\\s]+)*", ErrorMessage = "The Security Answer must start and end with no space.")]
        [StringLength(50, ErrorMessage = "The Security Answer must be less than {1} characters long.")]
        public string SecurityAnswer { get; set; }

        public long SecurityQuestionId { get; set; }

        public SecurityQuestionSelectListModel SecurityQuestionList { get; set; }

        public AuthenticatorSelectListModel AuthenticatorList { get; set; }

        public MyAccountModel()
        {
            SecurityQuestionList = new SecurityQuestionSelectListModel();
            AuthenticatorList = new AuthenticatorSelectListModel(true);
        }

        public static MyAccountModel Convert(User user)
        {
            return new MyAccountModel()
            {
                UserId = user.Id,
                Username = user.Name,
                FullName = user.FullName,
                Email = user.Email,
                AuthenticatorId = user.Authenticator.Id,
                SecurityQuestionId = user.Authenticator.Id == 1 ? user.SecurityQuestion.Id : 0
            };
        }
    }

    public class AccountLogOnModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public AuthenticatorSelectListModel AuthenticatorList { get; set; }

        public AccountLogOnModel()
        {
            AuthenticatorList = new AuthenticatorSelectListModel();
        }
    }

    public class ChangePasswordModel
    {
        public string Username { get; set; }

        public string SecurityQuestion { get; set; }

        public string SecurityAnswer { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Username { get; set; }
    }
}