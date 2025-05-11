using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WhiteListing_Backend.Models;
using WhiteListing_Backend.Stores;

namespace WhiteListing_Backend.Identity
{
    public class CustomUserManager : UserManager<ApplicationUser>
    {
        public CustomUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<ApplicationUser?> FindByID_NOAsync(string idNo)
        {
            if (Store is ApplicationUserStore customStore)
            {
                return await customStore.FindByID_NOAsync(idNo, CancellationToken.None);
            }

            return null;
        }
        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            if (Store is ApplicationUserStore customStore)
            {
                return await customStore.FindByEmailAsync(email, CancellationToken.None);
            }

            return null;
        }
    }
}
