using Domain.DB.Model;
using Domain.Model;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Domain.Service
{
    /// <summary>
    /// Zip作成用のサービス。
    /// </summary>
    public class ZipService
    {
        static readonly Encoding DefaultEncoding = Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// Zip作成処理を行う。
        /// </summary>
        /// <param name="entryList"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public MemoryStream CreateZipStream(List<ZipFileEntryInfo> entryList, Encoding encoding = null)
        {
            if(encoding == null)
                encoding = DefaultEncoding;

            // zipファイル生成
            MemoryStream zipStream = new MemoryStream();

            using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true, encoding))
            {
                foreach (var each in entryList)
                {
                    ZipArchiveEntry entry = zip.CreateEntry(each.Name);
                    using (Stream entryStream = entry.Open())
                    {
                        each.Content.Seek(0, SeekOrigin.Begin);
                        each.Content.CopyTo(entryStream);
                    }
                }
            }
            
            zipStream.Seek(0, SeekOrigin.Begin);
            return zipStream;
        }
    }

    public class ZipFileEntryInfo
    {
        public ZipFileEntryInfo()
        {
        }

        public ZipFileEntryInfo(string name, Stream content)
        {
            Name = name;
            Content = content;
        }

        public string Name {get;set;}
        public Stream Content {get;set;}
    }

}
