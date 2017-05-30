using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace wkhtmltopdfCore
{
    public class WkhtmltopdfDriver
    {
        public static byte[] ConvertHtml(string wkhtmltopdfPath, string switches, string html)
        {
            return WkhtmltopdfDriver.Convert(wkhtmltopdfPath, switches, html);
        }

        public static byte[] Convert(string wkhtmltopdfPath, string switches)
        {
            return WkhtmltopdfDriver.Convert(wkhtmltopdfPath, switches, null);
        }

        private static byte[] Convert(string wkhtmltopdfPath, string switches, string html)
        {
            switches = "-q " + switches + " -";
            if (!string.IsNullOrEmpty(html))
            {
                switches += " -";
                html = WkhtmltopdfDriver.SpecialCharsEncode(html);
            }
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(wkhtmltopdfPath, "wkhtmltopdf.exe"),
                    Arguments = switches,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    WorkingDirectory = wkhtmltopdfPath,
                    CreateNoWindow = true
                }
            };
            process.Start();
            if (!string.IsNullOrEmpty(html))
            {
                using (StreamWriter standardInput = process.StandardInput)
                {
                    standardInput.WriteLine(html);
                }
            }
            MemoryStream memoryStream = new MemoryStream();
            using (Stream baseStream = process.StandardOutput.BaseStream)
            {
                byte[] array = new byte[4096];
                int count;
                while ((count = baseStream.Read(array, 0, array.Length)) > 0)
                {
                    memoryStream.Write(array, 0, count);
                }
            }
            string message = process.StandardError.ReadToEnd();
            if (memoryStream.Length == 0L)
            {
                throw new Exception(message);
            }
            process.WaitForExit();
            return memoryStream.ToArray();
        }

        private static string SpecialCharsEncode(string text)
        {
            char[] array = text.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder(text.Length + (int)((double)text.Length * 0.1));
            char[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                char value = array2[i];
                int num = System.Convert.ToInt32(value);
                if (num > 127)
                {
                    stringBuilder.AppendFormat("&#{0};", num);
                }
                else
                {
                    stringBuilder.Append(value);
                }
            }
            return stringBuilder.ToString();
        }
    }
}