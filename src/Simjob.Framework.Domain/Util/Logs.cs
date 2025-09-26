using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Util
{
    public class Logs
    {
        public static void AddLog(string cInfo)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            lock (filePath)
            {
                FileStream fs = null;
                try
                {
                    string conteudo = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": " + cInfo + "\r\n";
                    string fileName = "log-" + DateTime.Now.ToString("yyyyMMdd") + ".TXT";
                    string path = Path.Combine(filePath, fileName);

                    fs = System.IO.File.OpenWrite(path);
                    fs.Seek(0, SeekOrigin.End);
                    fs.Write(Encoding.Default.GetBytes(conteudo), 0, conteudo.Length);
                    fs.Close();
                }
                catch
                {
                }
                finally
                {
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}


