using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace FeedbackPlatform.Extensions.Validator
{
    public class EnglishLettersOnlyPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (Regex.IsMatch(password, "^[a-zA-Z0-9]+$"))
            {
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordKeyboardLayout",
                Description = "Passwords must contain only English alphabet letters."
            }));
        }
    }
}
