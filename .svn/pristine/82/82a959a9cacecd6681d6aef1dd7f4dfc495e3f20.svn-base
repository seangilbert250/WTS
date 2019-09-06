using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using ICSharpCode.SharpZipLib.Zip;

namespace WTS.Util
{
    public class FileUtil
    {

        public static FileInfo CreateZipFileAtPath(List<FileInfo> files, string path, string fileName, int compressionLevel = 9)
        {
            FileInfo zipFile = null;

            DirectoryInfo dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                dir.Create();
            }

            byte[] zip = CreateZipFile(files, compressionLevel);

            if (zip != null)
            {
                using (FileStream fs = new FileStream(path + "\\" + fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(zip, 0, zip.Length);
                }

                zipFile = new FileInfo(path + "\\" + fileName);
            }

            return zipFile;
        }

        public static byte[] CreateZipFile(List<FileInfo> files, int compressionLevel = 9)
        {
            byte[] zip = null;

            if (files != null && files.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipOutputStream zos = new ZipOutputStream(ms))
                    {
                        zos.SetLevel(compressionLevel); // 0 - no compression, 9 - best compression

                        foreach (FileInfo fi in files)
                        {
                            // the zip library does NOT like special characters, so take out the worse offenders
                            string fn = fi.Name;
                            string fixedFn = fn.Replace("/", "_").Replace("\\", "_").Replace(" ", "_").Replace(":", "_").Replace("'", "_");

                            var entry = new ZipEntry(fn);

                            entry.DateTime = DateTime.Now;
                            zos.PutNextEntry(entry);

                            byte[] buffer = new byte[4096];

                            using (FileStream fs = fi.OpenRead())
                            {
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    zos.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                            }
                        }

                        zos.Finish();

                        zip = ms.ToArray();

                        zos.Close();
                    }
                }
            }

            return zip;
        }

        public static FileInfo CreateZipFileAtPath(Dictionary<string, byte[]> files, string path, string fileName, int compressionLevel = 9)
        {
            FileInfo zipFile = null;

            DirectoryInfo dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                dir.Create();
            }

            byte[] zip = CreateZipFile(files, compressionLevel);

            if (zip != null)
            {                
                using (FileStream fs = new FileStream(path + "\\" + fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(zip, 0, zip.Length);
                }

                zipFile = new FileInfo(path + "\\" + fileName);
            }

            return zipFile;
        }

        public static byte[] CreateZipFile(Dictionary<string, byte[]> files, int compressionLevel = 9)
        {
            byte[] zip = null;

            if (files != null && files.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipOutputStream zos = new ZipOutputStream(ms))
                    {
                        zos.SetLevel(compressionLevel); // 0 - no compression, 9 - best compression

                        foreach (string fn in files.Keys)
                        {
                            byte[] fd = files[fn];

                            // the zip library does NOT like special characters, so take out the worse offenders
                            string fixedFn = fn.Replace("/", "_").Replace("\\", "_").Replace(" ", "_").Replace(":", "_").Replace("'", "_");

                            ZipEntry ze = new ZipEntry(fixedFn);
                            ze.DateTime = DateTime.Now;

                            zos.PutNextEntry(ze);

                            if (fd != null && fd.Length > 0)
                            {
                                zos.Write(fd, 0, fd.Length);
                            }

                            zos.CloseEntry();
                        }                        

                        zos.Finish();

                        zip = ms.ToArray();

                        zos.Close();
                    }                    
                }
            }
            
            return zip;
        }
    }
}