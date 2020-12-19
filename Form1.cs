using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolServiceForms
{
    public partial class Form1 : Form
    {
        string CurrentFilePath = "";

        public Form1()
        {
            InitializeComponent();
            KeyDown += Form1_KeyPress;
            KeyPreview = true;
        }

        private void Form1_KeyPress(object sender, KeyEventArgs e)
        {
            if(e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                button1_Click(sender, e);
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                button5_Click(sender, e);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            foreach(string path in openFileDialog.FileNames)
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
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FileInteraction.TodayLesson.Appunti = textBox1.Text;
            FileInteraction.fileSaved = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewUrlForm urlForm = new NewUrlForm();
            urlForm.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileInteraction.TodayLesson.Subject = comboBox1.Text;
            FileInteraction.fileSaved = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AllegatiForm allegati = new AllegatiForm();
            allegati.ShowDialog();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if(CurrentFilePath == "")
            {
                saveFileDialog.ShowDialog();
                if (saveFileDialog.FileName != "")
                {
                    CurrentFilePath = saveFileDialog.FileName;
                    await FileInteraction.SaveData(saveFileDialog.FileName);
                }
            }
            else
            {
                await FileInteraction.SaveData(CurrentFilePath);
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (!FileInteraction.fileSaved)
            {
                DialogResult dialog = MessageBox.Show("Vuoi salvare?", "attenzione", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                if (dialog == DialogResult.Cancel)
                {
                    return;
                }
                else if(dialog == DialogResult.Yes)
                {
                    button1_Click(sender, e);
                }
            }

            OpenFileDialog fileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "File School Service | *.abs"
            };

            fileDialog.ShowDialog();
            if (File.Exists(fileDialog.FileName))
            {
                await FileInteraction.OpenData(fileDialog.FileName);
                textBox1.Text = FileInteraction.TodayLesson.Appunti;
                CurrentFilePath = fileDialog.FileName;
                foreach(string item in comboBox1.Items)
                {
                    if(item == FileInteraction.TodayLesson.Subject)
                    {
                        comboBox1.SelectedItem = item;
                    }
                }
                FileInteraction.fileSaved = true;
            }
            
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (File.Exists(args[1]))
                {
                    await FileInteraction.OpenData(args[1]);
                    textBox1.Text = FileInteraction.TodayLesson.Appunti;
                    CurrentFilePath = args[1];
                    foreach (string item in comboBox1.Items)
                    {
                        if (item == FileInteraction.TodayLesson.Subject)
                        {
                            comboBox1.SelectedItem = item;
                        }
                    }
                    FileInteraction.FileSaved = true;
                }
            }
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (!FileInteraction.fileSaved)
            {
                DialogResult dialog = MessageBox.Show("Vuoi salvare?", "attenzione", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
                if (dialog == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dialog == DialogResult.Yes)
                {
                    button1_Click(sender, e);
                }
            }
        }
    }
}
