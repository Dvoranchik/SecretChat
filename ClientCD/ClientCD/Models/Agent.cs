using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCD
{
    public class Agent
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public override bool Equals(object obj)
        {
            return this.Password == ((Agent)obj).Password && this.Login== ((Agent)obj).Login;
        }
    }
}
