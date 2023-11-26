using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework10
{
    public class SentFile
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Тип файла
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="FileType">Тип файла</param>
        public SentFile(string FileName, string FileType)
        {
            this.FileName = FileName;
            this.FileType = FileType;
        }

    }
}
