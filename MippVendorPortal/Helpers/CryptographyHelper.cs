namespace MippVendorPortal.Helpers
{
        public class CryptographyHelper
        {
            public string DecryptString(string encrString)
            {
                byte[] b;
                string decrypted;
                try
                {
                    b = Convert.FromBase64String(encrString);
                    decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
                }
                catch (FormatException fe)
                {
                    decrypted = "";
                }
                return decrypted;
            }

            public string EncryptString(string strEncrypted)
            {
                byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
                string encrypted = Convert.ToBase64String(b);
                return encrypted;
            }
        }
    

}
