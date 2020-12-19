using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolServiceForms
{
    public partial class NewUrlForm : Form
    {

        public bool CheckForValidUrl(string _Url)
        {
            Uri uriResult;
            return Uri.TryCreate(_Url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public NewUrlForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TextBox2_KeyDown(object sender, EventArgs e)
        {
            if(textBox1.Text != "" && textBox2.Text != "")
            {
                if (CheckForValidUrl(textBox2.Text))
                {
                    Url _Url = new Url
                    {
                        Name = textBox1.Text,
                        UrlString = textBox2.Text
                    };
                    FileInteraction.TodayLesson.Urls.Add(_Url);
                    FileInteraction.fileSaved = false;

                    this.Close();
                }
                else if(CheckForValidUrl("https://" + textBox2.Text))
                {
                    Url _Url = new Url
                    {
                        Name = textBox1.Text,
                        UrlString = "https://" + textBox2.Text
                    };
                    FileInteraction.TodayLesson.Urls.Add(_Url);
                    FileInteraction.fileSaved = false;

                    this.Close();
                }
                else if(CheckForValidUrl("http://" + textBox2.Text))
                {
                    Url _Url = new Url
                    {
                        Name = textBox1.Text,
                        UrlString = "http://" + textBox2.Text
                    };
                    FileInteraction.TodayLesson.Urls.Add(_Url);
                    FileInteraction.fileSaved = false;

                    this.Close();
                }
                else
                {
                    MessageBox.Show("L'url non è valido", "Errore");
                }
            }
            else
            {
                MessageBox.Show("Compila tutti i campi", "Errore");
            }
        }

        private void TextBox2_KeyDown(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                TextBox2_KeyDown(sender, (EventArgs)e);
            }
        }
    }
}
