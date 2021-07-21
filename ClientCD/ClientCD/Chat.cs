using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace ClientCD
{
    public partial class Chat : Form
    {
        private TimeSpan DifDate;
        static bool alive = false; 
        string url = "https://192.168.56.108:53242";
        Agent agent1;
        private string _cookies;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            alive = false;
        }
        public Chat()
        {
            InitializeComponent();

        }


        private void ReceiveFiles()
        {
           
            alive = true;
            string responseInString = "";
            try
            {
                while (alive)
                {
                    string url = "https://192.168.56.108:53242/script/agent.php";
                    Thread.Sleep(1000);
                    using (var wb = new WebClient())
                    {
                        wb.Headers.Add("Cookie", _cookies);
                        var data = new NameValueCollection();
                        data["typeMessage"] = "getListFile";
                        var response = wb.UploadValues(url, "POST", data);
                        responseInString = Encoding.UTF8.GetString(response);
                    }
                    url = "https://192.168.56.108:53242/users/agents/agent/send/files/";
                    var jsonfiles = JsonSerializer.Deserialize<string[][]>(responseInString);

                    String mypath = @"./save/crypto/received";
                    var second = Directory
                        .GetFiles(mypath, "*", SearchOption.AllDirectories).ToList();
                    foreach (var file in jsonfiles)
                    {
                        if (second.Find(x => x.Split('\\')[x.Split('\\').Length - 1] == file[0]) == null)
                        {
                            if (File.GetLastWriteTime("./save/crypto/received/" + file[0]) + DifDate < DateTime.Parse(file[1]))
                            {
                                using (var wb = new WebClient())
                                {
                                string path = "./save/crypto/received/"+ file[0].Split('/')[0];
                                DirectoryInfo dirInfo = new DirectoryInfo(path);
                                if (!dirInfo.Exists)
                                {
                                    dirInfo.Create();
                                }
                                wb.Headers.Add("Cookie", _cookies);
                                    wb.DownloadFile(url + file[0], file[0].Split('/')[1]);

                                }
                                var writePath = "./save/crypto/received/" + file[0];
                                try
                                {
                                    CryptoHelper.EncryptFile(file[0].Split('/')[1], "./save/crypto/received/" + file[0]);
                                    File.Delete(file[0]);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                if (!alive)
                    return;
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ReceiveMessages()
        {
            string url = "https://192.168.56.108:53242/script/agent.php";
            alive = true;
            string responseInString = "";
            try
            {
                while (alive)
                {
                    Thread.Sleep(1000);
                    using (var wb = new WebClient())
                    {
                        wb.Headers.Add("Cookie", _cookies);
                        var data = new NameValueCollection();
                        data["typeMessage"] = "getMessage";
                        data["messageFromAgent"] = "getMessage";
                        var response = wb.UploadValues(url, "POST", data);
                        responseInString = Encoding.UTF8.GetString(response);
                    }
                    var jsonfiles = JsonSerializer.Deserialize<string[][]>(responseInString);
                    String mypath = @"./save/messages/received";
                    var second = Directory
                        .GetFiles(mypath, "*", SearchOption.AllDirectories).ToList();
                    foreach (var file in jsonfiles)
                    {
                        if (second.Find(x => x.Split('\\')[x.Split('\\').Length-1] == file[1]) != null)
                        {
                            if(File.GetLastWriteTime("./save/messages/received/" + file[1])+DifDate < DateTime.Parse(file[0]))
                            {
                                using (var wb = new WebClient())
                                {
                                    wb.Headers.Add("Cookie", _cookies);
                                    var data = new NameValueCollection();
                                    data["typeMessage"] = "getMessageFile";
                                    data["fileName"] = file[1];
                                    var response = wb.UploadValues(url, "POST", data);
                                    responseInString = Encoding.UTF8.GetString(response);

                                }
                                var writePath = "./save/messages/received/" + file[1];
                                try
                                {
                                    File.Delete(writePath);
                                    using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default))
                                    {
                                        sw.WriteLine(responseInString);
                                    }
                                   
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                var form = new MessageFromServer(writePath);
                                if (form.ShowDialog() == DialogResult.OK)
                                {

                                }
                            }
                        }
                        else
                        {
                            using (var wb = new WebClient())
                            {
                                wb.Headers.Add("Cookie", _cookies);
                                var data = new NameValueCollection();
                                data["typeMessage"] = "getMessageFile";
                                data["fileName"] = file[1];
                                var response = wb.UploadValues(url, "POST", data);
                                responseInString = Encoding.UTF8.GetString(response);
                            }
                            var writePath = "./save/messages/received/" + file[1];
                            try
                            {
                                
                                using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default))
                                {
                                    sw.WriteLine(responseInString);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            var form = new MessageFromServer(writePath);
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                
                            }
                        }
                        
                    }

                    
                }
                
            }
            catch (ObjectDisposedException)
            {
                if (!alive)
                    return;
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public Chat(Agent agent)
        {
            InitializeComponent();
            agent1 = agent;
            
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain,
                  SslPolicyErrors sslPolicyErrors)
              {
                  return true;
              };
            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    var response = wb.UploadValues(url, data);
                    _cookies = wb.ResponseHeaders["Set-Cookie"];


                    var dataM = new NameValueCollection();
                    dataM.Add("login", agent1.Login);
                    dataM.Add("password", agent1.Password);

                    wb.Headers.Add("Cookie", _cookies);
                    response = wb.UploadValues(url, dataM);
                    string responseInString = Encoding.UTF8.GetString(response);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка авторизации на сервере");
            }
            try
            {
                using (var wb = new WebClient())
                {
                    //Синхронизация времени с сервером
                    string url = "https://192.168.56.108:53242/script/agent.php";
                    var data2 = new NameValueCollection();
                    wb.Headers.Add("Cookie", _cookies);
                    data2.Add("typeMessage", "syncDate");
                    var response = wb.UploadValues(url, data2);
                    var responseInString = Encoding.UTF8.GetString(response);
                    var date = DateTime.Parse(responseInString);
                    DifDate = date - DateTime.Now;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Ошибка сервера");
            }
            Task receiveTask = new Task(ReceiveMessages);
            receiveTask.Start();

            Task receiveTask2 = new Task(ReceiveFiles);
            receiveTask2.Start();

            string path = "./save";
            string subpath = @"messages/received";
            string subpath3 = @"messages/send";
            string subpath2 = @"crypto/received";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            dirInfo.CreateSubdirectory(subpath);
            dirInfo.CreateSubdirectory(subpath2);
            dirInfo.CreateSubdirectory(subpath3);


        }
        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://192.168.56.108:53242/script/agent.php";
            try
            {
                var messageToServer = new Message()
                {
                    TimeMessage = DateTime.Now + DifDate,
                    TextMessage = richTextBox1.Text,
                    Sender = agent1.Login
                };
                var message = JsonSerializer.Serialize(messageToServer);
                richTextBox2.Text = agent1.Login +": "+ messageToServer.TimeMessage + " " + messageToServer.TextMessage + "\r\n" + richTextBox2.Text;

                using (var wb = new WebClient())
                {
                    wb.Headers.Add("Cookie", _cookies);
                    var data = new NameValueCollection();
                    data["typeMessage"] = "message";
                    data["messageFromAgent"] = message;
                    var response = wb.UploadValues(url, "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }

                string writePath = @"./save/messages/send/"+ messageToServer.TimeMessage.ToShortDateString() + ".txt";
                try
                {
                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                foreach (var file in listBox1.Items)
                {
                    string destinationFile = file.ToString();
                    var destinationArr = destinationFile.Split('\\');
                    var destinationFile2 = destinationArr[destinationArr.Length - 1];

                    CryptoHelper.EncryptFile(destinationFile, "./save/crypto/" + destinationFile2);
                    richTextBox2.Text = agent1.Login + ": " + messageToServer.TimeMessage + " file: " + destinationFile + "\r\n" + richTextBox2.Text;
                    using (var client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        client.Headers.Add("Cookie", _cookies);
                        var resp = client.UploadFile(url, destinationFile);
                        var responseInString = Encoding.UTF8.GetString(resp);
                    }
                }
                listBox1.Items.Clear();
                this.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            listBox1.Items.Add(filename);
        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            Directory.Delete(@"./save", true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "./save/";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            var form = new StoreMessages(filename);
            form.Show();            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog2.InitialDirectory = "./save/crypto/received/";
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog2.FileName;
            var form = new CryptoPath();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var result = filename.Split('\\');
                CryptoHelper.DecryptFile(filename, form.Path + "\\"+result[result.Length-1]);
                MessageBox.Show("Файл успешно расшифрован");
            }
        }
    }
}
