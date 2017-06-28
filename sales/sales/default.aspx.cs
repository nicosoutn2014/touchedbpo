using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace sales
{
    public partial class _default1 : System.Web.UI.Page
    {
        public string connectionInfo = WebConfigurationManager.AppSettings["connString"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Count >= 1)
                Response.Redirect("main.aspx");
            TextName.Focus();
            this.TextName.Attributes.Add("onkeypress", "button_click(this,'" + this.BtnLogin.ClientID + "')");
            this.TextPass.Attributes.Add("onkeypress", "button_click(this,'" + this.BtnLogin.ClientID + "')");
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            string query = "";
            string user = this.TextName.Value;
            string pass = this.TextPass.Value;

            using (var conn = new SqlConnection(connectionInfo))
            {
                query = string.Format(@"SELECT PERFIL, NOMBRE, APELLIDO, CARGO FROM LOGIN_INFO WHERE [USER_NAME] = '{0}' AND USER_PASS = '{1}'", user, pass);
                SqlCommand comm = new SqlCommand(query, conn);
                conn.Open();
                /*string profile = (string)comm.ExecuteScalar();
                SqlDataReader reg = null;
                reg = comm.ExecuteReader();*/

                DataTable dt = new DataTable();
                dt.Load(comm.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    Session.Add("user_logged", user.ToLower()); //agrego user a Session
                    Session.Add("user_profile", dt.Rows[0]["PERFIL"]); //agrego profile a Session
                    Session.Add("name", dt.Rows[0]["NOMBRE"] + " " + dt.Rows[0]["APELLIDO"]); //agrego nombre usuario
                    Session.Add("cargo", dt.Rows[0]["CARGO"]);
                    if ((string)Session["user_profile"] == "ADMIN")
                        Response.Redirect("main.aspx"); //direcciono al menu
                    else
                        Response.Redirect("main2.aspx");
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('Por favor verifique usuario o contraseña!');", true);
                }

                conn.Close();
            }

        }
    }
}