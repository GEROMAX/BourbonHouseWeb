using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace BourbonHouseWeb
{
    public class Global : System.Web.HttpApplication
    {
        private HttpApplication toHttpAplication(object sender)
        {
            return (HttpApplication)sender;
        }

        private bool validateUserAndPassOK(string userName, string password)
        {
            return "bourbon".Equals(userName) && "house".Equals(password);
        }

        private GenericPrincipal getRole()
        {
            return new GenericPrincipal(new GenericIdentity("user"), new string[] { "User" });
        }

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var app = this.toHttpAplication(sender);
            var authBase64edValue = app.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authBase64edValue))
            {
                return;
            }

            //Base64からデコード
            var decodedValue = Encoding.GetEncoding("UTF-8").GetString(Convert.FromBase64String(authBase64edValue.TrimStart("Basic ".ToCharArray())));

            //認証を実施
            var nameAndPass = decodedValue.Split(":".ToCharArray());
            var userName = nameAndPass[0];
            var password = nameAndPass[1];
            if (this.validateUserAndPassOK(userName, password))
            {
                app.Context.User = this.getRole();
            }
            else
            {
                // 認証に失敗した場合はアクセス拒否の情報をセットする
                app.Response.StatusCode = 401;
                app.Response.StatusDescription = "Unauthorized";

                // 認証に失敗したことを通知する
                app.Response.Write("<html>\r\n<head>\r\n<title>");
                app.Response.Write("Basic認証失敗</title>\r\n</head>\r\n<body>\r\n");
                app.Response.Write("<h1>401 Access Denied</h1>\r\n");
                app.Response.Write("</body>\r\n</html>\r\n");
                app.CompleteRequest();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var app = this.toHttpAplication(sender);

            if (app.Response.StatusCode.Equals(401))
            {
                app.Response.HeaderEncoding = Encoding.GetEncoding("Shift_JIS");
                app.Response.AppendHeader("WWW-Authenticate", "Basic Realm=独自実装Basic認証のテスト");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}