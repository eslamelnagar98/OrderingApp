using Ordering.Server.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Server
{
    public class UserHandler
    {
        public oUserInfo AuthenticateUser(oClientLogIn o_oClientLogIn)
        {
            return AuthenticateUser(o_oClientLogIn.UserName, o_oClientLogIn.Password);
        }

        public oUserInfo AuthenticateUser(string userLogin, string password)
        {
            oUserInfo o_oUserInfo = new oUserInfo();

            if (string.IsNullOrWhiteSpace(userLogin) || string.IsNullOrWhiteSpace(password))
            {
                o_oUserInfo.IsClientAuthanticated = false;
            }
            return o_oUserInfo;
        }
    }
}
