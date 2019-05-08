using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MVCDiscografia
{
    public class Encriptacion
    {
        public static string MD5Hash(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //genera el hash
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(s));

            //almacena el hash
            byte[] result = md5.Hash;

            //convierte de byte[] a string
            StringBuilder textoEncriptado = new StringBuilder();
            result.ToList<byte>().ForEach(b => textoEncriptado.Append(b.ToString("x2")));

            return textoEncriptado.ToString();
        }
    }
}
