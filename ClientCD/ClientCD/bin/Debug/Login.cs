using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Text.Json;
using System.Text;

namespace ClientCD
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        static string ComputeSha256Hash(string rawData)
        { 
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var login = textBox1.Text;
            var password = textBox2.Text;
            string hashedData = ComputeSha256Hash(password);
            Agent agent = new Agent()
            {
                HashPassword = hashedData,
                Login = login
            };

            string fileName = "file.txt";
            var person = JsonSerializer.Deserialize<Agent>(File.ReadAllText(fileName));

            //if (agent.Login == person.Login && agent.HashPassword == person.HashPassword)
            //{
            var form = new Chat(agent);
            form.Show();
            //}
            //else{
            //MessageBox.Show("Ошибка авторизации");
            //}
            //this.Hide();
        }
    }
}
