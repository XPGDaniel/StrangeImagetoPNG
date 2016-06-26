using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StrangeImagetoPNG
{
    class Program
    {
        static void Main(string[] args)
        {
            int OK = 0, NG = 0;
            string checksumfile = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
            //string[] extensions = new string[] { "jp2", "webp" };
            string patt = ConfigurationManager.AppSettings["Extensions"];
            //foreach (string extension in extensions)
            //{
            //    if (string.IsNullOrEmpty(patt))
            //        patt = extension;
            //    else
            //        patt += "|" + extension;
            //}

            List<string> Filelist = GetFiles(ConfigurationManager.AppSettings["SourceLocation"], patt);
            if (Filelist.Any())
            {
                foreach (var fss in Filelist)
                {
                    try
                    {
                        FIBITMAP dib = FreeImage.LoadEx(fss);
                        FreeImage.Save(FREE_IMAGE_FORMAT.FIF_PNG, dib, Path.GetFileNameWithoutExtension(fss) + ".png", FREE_IMAGE_SAVE_FLAGS.PNG_Z_DEFAULT_COMPRESSION);
                        OK++;
                    }
                    catch (Exception)
                    {
                        NG++;
                        throw;
                    }
                }
                Filelist.Clear();
                Console.WriteLine("Total " + OK + " files converted, Total " + NG + " files failed to convert");
            }
            Console.WriteLine("Finished");
            Console.ReadKey();
        }
        static private List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();

            try
            {
                if (!path.Contains("$RECYCLE.BIN") && !path.Contains("#recycle"))
                {
                    List<string> candidate = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Where(file => Regex.IsMatch(file.ToLowerInvariant(), @"^.+\.(" + pattern + ")$")).ToList();
                    foreach (var s in candidate)
                    {
                        files.Add(s);
                    }
                    foreach (var directory in Directory.GetDirectories(path))
                        files.AddRange(GetFiles(directory, pattern));
                }
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }
    }
}
