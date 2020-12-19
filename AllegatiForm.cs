using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace SchoolServiceForms
{
    public partial class AllegatiForm : Form
    {
        System.Timers.Timer timer = new System.Timers.Timer
        {
            Interval = 50000
        };

        public AllegatiForm()
        {
            InitializeComponent();
        }

        delegate void AllegatiForm_LoadCallback(object sender, EventArgs e);

        private void AllegatiForm_Load(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            for (int i = 0; i < FileInteraction.TodayLesson.FileAllegati.Count; i++)
            {
                Files file = FileInteraction.TodayLesson.FileAllegati[i];
                ListViewItem Item = new ListViewItem()
                {
                    Text = FileInteraction.TodayLesson.FileAllegati[i].Name,
                    Tag = FileInteraction.TodayLesson.FileAllegati[i]
                };
                listView1.Items.Add(Item);
            }
            for (int i = 0; i < FileInteraction.TodayLesson.Urls.Count; i++)
            {
                Url url = FileInteraction.TodayLesson.Urls[i];
                ListViewItem Item = new ListViewItem()
                {
                    Text = FileInteraction.TodayLesson.Urls[i].Name,
                    Tag = FileInteraction.TodayLesson.Urls[i]
                };
                listView1.Items.Add(Item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();
            foreach (string path in openFileDialog.FileNames)
            {
                if (File.Exists(path))
                {
                    byte[] data = File.ReadAllBytes(path);

                    Files _Files = new Files
                    {
                        Name = Path.GetFileName(path),
                        Allegati = File.ReadAllBytes(path)
                    };

                    FileInteraction.TodayLesson.FileAllegati.Add(_Files);
                }
            }
            FileInteraction.fileSaved = false;
            AllegatiForm_Load(this, new EventArgs());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewUrlForm urlForm = new NewUrlForm();
            urlForm.ShowDialog();
            AllegatiForm_Load(this, new EventArgs());
            FileInteraction.fileSaved = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                try
                {
                    Url Obj = (Url)listView1.SelectedItems[0].Tag;
                    Process.Start(Obj.UrlString);
                }
                catch
                {
                    Files files = (Files)listView1.SelectedItems[0].Tag;
                    string path = Path.Combine(Path.GetTempPath(), files.Name);
                    string path1 = Path.Combine(Path.GetTempPath(), files.Name + ".tmp");
                    File.WriteAllBytes(path, files.Allegati);
                    File.WriteAllBytes(path1, files.Allegati);

                    Process app = Process.Start(path);
                    app.EnableRaisingEvents = true;
                    timer.Elapsed += (_sender, _e) => { CheckForProgress(path1, path, files); };
                    timer.Start();
                    app.Exited += (_sender, _e) => { CheckForProgress(path1, path, files); timer.Stop(); app.Dispose(); };
                    this.FormClosing += (_sender, _e) => { CheckForProgress(path1, path, files); timer.Stop(); app.Dispose(); };
                }
            }
        }
        
        public void CheckForProgress(string OriginalPath, string NewPath, Files file)
        {
            byte[] newFile = File.ReadAllBytes(NewPath);
            if (File.ReadAllBytes(OriginalPath) != newFile)
            {
                foreach(Files _file in FileInteraction.TodayLesson.FileAllegati)
                {
                    if(_file == file)
                    {
                        _file.Allegati = newFile;
                        FileInteraction.fileSaved = false;
                        if (listView1.InvokeRequired)
                        {
                            AllegatiForm_LoadCallback a = new AllegatiForm_LoadCallback(AllegatiForm_Load);
                            this.Invoke(a, new object[] { this, new EventArgs() });
                        }
                    }
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                try
                {
                    Url Obj = (Url)listView1.SelectedItems[0].Tag;

                    FileInteraction.TodayLesson.Urls.Remove(Obj);
                }
                catch
                {
                    Files files = (Files)listView1.SelectedItems[0].Tag;

                    FileInteraction.TodayLesson.FileAllegati.Remove(files);
                }
                finally
                {
                    FileInteraction.fileSaved = false;
                    if (listView1.InvokeRequired)
                    {
                        AllegatiForm_LoadCallback a = new AllegatiForm_LoadCallback(AllegatiForm_Load);
                        this.Invoke(a, new object[] { this, new EventArgs() });
                    }
                    else
                    {
                        AllegatiForm_Load(this, new EventArgs());
                    }
                }
            }
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }
    }
}
