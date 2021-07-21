using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientCD
{
    public partial class StoreMessages : Form
    {
        public StoreMessages(string path)
        {
            InitializeComponent();
            try
            {
                var strings = File.ReadAllLines(path);
                foreach (var str in strings)
                {
                    if (str != "")
                    {
                        var message = JsonSerializer.Deserialize<Dictionary<string,string>>(str);
                        richTextBox1.Text = message["Sender"] + " : " + message["TimeMessage"] + " " + message["TextMessage"] + "\r\n" + richTextBox1.Text;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
