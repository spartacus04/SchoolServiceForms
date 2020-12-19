using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolServiceForms
{
    [Serializable]
    public class Lesson
    {
        public string Subject { get; set; }
        public string Appunti { get; set; }
        public List<Files> FileAllegati = new List<Files>();
        public List<Url> Urls = new List<Url>();
    }

    [Serializable]
    public class Url
    {
        public string Name { get; set; }
        public string UrlString { get; set; }
    }

    [Serializable]
    public class Files
    {
        public string Name { get; set; }
        public byte[] Allegati { get; set; }
    }


    public static class FileInteraction
    {
        public static Form1 MainForm;

        public static bool FileSaved = true;
        public static bool fileSaved
        {
            get 
            {
                return FileSaved;
            }
            set
            {
                if (value)
                {
                    Application.OpenForms["Form1"].Controls["button1"].Font = new System.Drawing.Font(Application.OpenForms["Form1"].Controls["button1"].Font, FontStyle.Regular);
                }
                else
                {
                    Application.OpenForms["Form1"].Controls["button1"].Font = new System.Drawing.Font(Application.OpenForms["Form1"].Controls["button1"].Font, FontStyle.Italic);
                }
                FileSaved = value;
            }
        }

        public static Lesson TodayLesson = new Lesson();
        private static byte[] Serialize()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, TodayLesson);
                return ms.ToArray();
            }
        }
        
        private static Lesson Deserialize(byte[] _Lesson)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(_Lesson, 0, _Lesson.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (Lesson)bf.Deserialize(ms);
            }
        }

        public static async Task SaveData(string path)
        {
            using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] ToWrite = Serialize();
                await fs.WriteAsync(ToWrite, 0, ToWrite.Length);
                fs.Dispose();
            }
            fileSaved = true;
        }

        public static async Task OpenData(string path)
        {
            using (FileStream fsO = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] ToRead = new byte[fsO.Length];
                await fsO.ReadAsync(ToRead, 0, (int)fsO.Length);
                TodayLesson = Deserialize(ToRead);
                fsO.Dispose();
            }
        }
    }
}
