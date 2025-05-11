using Microsoft.AspNetCore.Identity;
using WhiteListing_Backend.Models;

namespace WhiteListing_Backend.Stores
{
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>

    {
        private HttpClient _httpClient;
        private readonly Supabase.Client _supabase;

        public ApplicationUserStore(HttpClient httpClient, Supabase.Client Supabase)
        {
            _httpClient = httpClient;
            _supabase = Supabase;

        }
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User is empty." });
            var passwordHash = await GetPasswordHashAsync(user, cancellationToken);

            var newUser = new SupabaseUserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = passwordHash,
                IdNo = user.IdNo,


            };

            //return await _usersTable.CreateAsync(user);
            var response = await _supabase.From<SupabaseUserModel>().Insert(newUser);
            if (response == null || (response.ResponseMessage == null))
                return IdentityResult.Failed(new IdentityError { Description = "Failed to add new user." });

            return IdentityResult.Success;
        }




        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            var response = _supabase.From<SupabaseUserModel>().Where(M => M.Id == user.Id).Delete();
            if (response == null || !response.IsCompletedSuccessfully)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Failed to delete user from Supabase." });
            }
            return IdentityResult.Success;

        }

        public void Dispose()
        {
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            Guid idGuid;
            if (!Guid.TryParse(userId, out idGuid))
            {
                throw new ArgumentException("Not a valid Guid id", nameof(userId));
            }
            //var stringToGuid = new Guid(userId);

            var response = await _supabase.From<SupabaseUserModel>().Where(M => M.Id == idGuid).Get();

            var userModel = response.Models.FirstOrDefault();

            if (userModel == null) throw new ArgumentNullException(nameof(userModel));

            // Map from SupabaseUserModel to ApplicationUser
            return new ApplicationUser
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
                Email = userModel.Email,
                PasswordHash = userModel.PasswordHash,
                NormalizedUserName = userModel.NormalizedUserName,
                IdNo = userModel.IdNo
            };


        }

        public async Task<ApplicationUser?> FindByNameAsync(string userName, CancellationToken cancellationToken)
        {
            //cancellationToken.ThrowIfCancellationRequested();
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            //As identity nomalizes this username into uppercase
            userName = userName.ToLowerInvariant();

            var response = await _supabase.From<SupabaseUserModel>().Where(M => M.UserName == userName).Get();
            Console.WriteLine($"Response is {response}");

            var userModel = response.Models.FirstOrDefault();

            if (userModel == null) return null;

            // Map from SupabaseUserModel to ApplicationUser
            return new ApplicationUser
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
                Email = userModel.Email,
                PasswordHash = userModel.PasswordHash,
                NormalizedUserName = userModel.NormalizedUserName,
                IdNo = userModel.IdNo

            };
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);

        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser?> FindByID_NOAsync(string Id_No, CancellationToken cancellationToken)
        {
            //cancellationToken.ThrowIfCancellationRequested();
            //if (Id_No == null) throw new ArgumentNullException(nameof(userName));
            if (Id_No == null) return null;

            var response = await _supabase.From<SupabaseUserModel>().Where(M => M.IdNo == Id_No).Get();

            var userModel = response.Models.FirstOrDefault();

            if (userModel == null) return null;

            // Map from SupabaseUserModel to ApplicationUser
            return new ApplicationUser
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
                Email = userModel.Email,
                PasswordHash = userModel.PasswordHash,
                NormalizedUserName = userModel.NormalizedUserName,
                IdNo = userModel.IdNo

            };
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            //cancellationToken.ThrowIfCancellationRequested();
            //if (Id_No == null) throw new ArgumentNullException(nameof(userName));
            if (email == null) return null;

            var response = await _supabase.From<SupabaseUserModel>().Where(M => M.Email == email).Get();

            var userModel = response.Models.FirstOrDefault();

            if (userModel == null) return null;

            // Map from SupabaseUserModel to ApplicationUser
            return new ApplicationUser
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
                Email = userModel.Email,
                PasswordHash = userModel.PasswordHash,
                NormalizedUserName = userModel.NormalizedUserName,
                IdNo = userModel.IdNo

            };
        }
    }
}
