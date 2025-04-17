
namespace eFakturADM.Logic.Utilities
{
    public static class CryptographyHelperExtension
    {
        const string EncKey = @"0F2F1ABF";
        public static string EncryptAndHash(string input)
        {
            return CryptographyHelper.EncryptAndHash(EncKey, input);
        }

        public static string DecryptWithHash(string input)
        {
            return CryptographyHelper.DecryptWithHash(EncKey, input);
        }
    }
}
