using PasswordHashing;
using System.Data;
using System.Data.SqlClient;

namespace webapi
{
    public static class HelperUtility
    {
        public static string GetPasswordHash(string password)
        {
            string hash = string.Empty;

            PasswordHasher.SetDefaultSettings(HashAlgorithm.SHA1, 16);

            hash = PasswordHasher.Hash(password);

            return hash;
        }

        public static bool ValidateHash(string password)
        {
            return PasswordHasher.Validate(password, GetPasswordHash(password));
        }

    
    }

}
