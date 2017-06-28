using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Web.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.Net.Mail;
using System.Net;

namespace sales
{
    public partial class main2 : System.Web.UI.Page
    {
        public string connectionInfo = WebConfigurationManager.AppSettings["connString"];
        public string user = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Count < 1)
                Response.Redirect("default.aspx");
            if ((string)Session["user_profile"] == "OPERADOR")
            {
                scoring.Visible = false;
                tablero.Visible = false;
                retorno.Visible = false;
            }
            else
            {
                checkModif();
            }
            checkData();
            Label.Visible = false;
            LtlDataLoaded.Text = "No se cargaron datos.";
            user = "https://avatars.discourse.org/v2/letter/" + ((string)Session["user_logged"])[0] + "/f6c823/45.png";
        }

        protected void checkModif()
        {
            string q1 = string.Format(@"select top(1) LOTE from ALERTS
                                        where LOTE like '%Scoring%'
                                        order by ID desc");
            using (var conn = new SqlConnection(connectionInfo))
            {
                SqlCommand cmmd1 = new SqlCommand(q1, conn);
                conn.Open();
                SqlDataReader reg1 = null;
                reg1 = cmmd1.ExecuteReader();

                if (reg1.Read())
                {
                    LblLastLote.Text = "";
                    string lote = reg1["LOTE"].ToString();
                    LblLastLote.Text += lote;
                }
                else
                {
                    LblLastLote.Text += " --- ";
                }
                conn.Close();
            }
        }

        protected void checkData()
        {
            List<string> pendVal = new List<string>();
            List<string> pendScor = new List<string>();
            string qryVal = ""; string qryScor = "";

            using (var conn = new SqlConnection(connectionInfo))
            {
                //consulto los lotes que faltan corregir
                //string qLote = "SELECT DISTINCT LOTE FROM ALERTS WHERE ESTADO = 'PENDIENTE'";
                //consulto los lotes de VAL_DATA que tienen pendientes
                if((string)Session["user_profile"] == "OPERADOR")
                {
                    if(((string)Session["user_logged"]).Contains("capa") == true)
                    {
                        qryVal = "= '" + (string)Session["user_logged"] + "'";
                        qryScor = "and Vendedor = '" + (string)Session["user_logged"] + "'";
                    }
                    else
                    {
                        qryVal = "= '" + (string)Session["name"] + "'";
                        qryScor = "and Vendedor = '" + (string)Session["name"] + "'";
                    }
                }
                else
                {
                    qryVal = "is not null";
                }
                string q = string.Format(@"SELECT DISTINCT VENDEDOR, count(distinct DNI) as CANT
                                            FROM VAL_DATA
                                            WHERE Estado <> 'PROCESADO' and Estado <> 'Existente' and Modif is null and Retorno is null and EstadoFinal is null
                                            and Vendedor {0}
                                            group by VENDEDOR", qryVal);
                //consulto los lotes de TEMP_SCOR que tienen pendientes
                string q2 = string.Format(@"SELECT DISTINCT VENDEDOR, count(distinct b.Propuesta) as CANT
                                            FROM VAL_DATA as a
                                            inner join TEMP_SCOR as b on a.Propuesta = b.Propuesta
                                            WHERE b.FOLLOW_STATUS <> 'Positiva' and b.ModifScor is null and b.Retorno_Scor is null and a.Retorno is null
                                            {0}
                                            group by VENDEDOR", qryScor);
                

                //SqlCommand cmd = new SqlCommand(qLote, conn);
                SqlCommand cmd2 = new SqlCommand(q, conn);
                SqlCommand cmd3 = new SqlCommand(q2, conn);
                conn.Open();
                /*using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lotes.Add(reader["LOTE"].ToString());
                    }
                }*/
                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pendVal.Add("Retorno|" + reader["Vendedor"].ToString() + "|" + reader["CANT"].ToString());
                    }
                }
                using (var reader = cmd3.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pendScor.Add("Scoring|" + reader["Vendedor"].ToString() + "|" + reader["CANT"].ToString());
                    }
                }
                
                conn.Close();
            }
            if (pendVal.Count > 0)
            {
                foreach (var elem in pendVal)
                {
                    Button btnVal = new Button();
                    string[] txts = elem.Split(new char[] { '|' });
                    btnVal.Text = txts[0] + "_" + txts[1] + "_(" + txts[2] + ")";
                    btnVal.CssClass = "btn btn-primary";
                    btnVal.Click += showData;
                    Lotes.Controls.Add(btnVal);
                }
            }
            else
            {
                Label text = new Label();
                text.Text = "No hay registros para corregir.<i class=\"glyphicon glyphicon-check\" style=\"color:greenM\"></i>";
                theadN.Text = "";
                tbodyN.Text = "";
                //tfootN.Text = "";
                Lotes.Controls.Clear();
                Lotes.Controls.Add(text);
            }
            if (pendScor.Count > 0)
            {
                foreach (var elem in pendScor)
                {
                    Button btnScor = new Button();
                    string[] txts = elem.Split(new char[] { '|' });
                    btnScor.Text = txts[0] + "_" + txts[1] + "_(" + txts[2] + ")";
                    btnScor.CssClass = "btn btn-warning";
                    btnScor.Click += showData;
                    Lotes.Controls.Add(btnScor);
                }
            }
            Button btnTotVal = new Button();
            btnTotVal.Text = "Retorno_Total_Erroneo";
            btnTotVal.CssClass = "btn btn-danger";
            btnTotVal.Click += showData;

            Button btnTotScor = new Button();
            btnTotScor.Text = "Scoring_Total_Negativo";
            btnTotScor.CssClass = "btn btn-success";
            btnTotScor.Click += showData;

            Lotes.Controls.Add(btnTotVal);
            Lotes.Controls.Add(btnTotScor);
        }

        protected void showData(object sender, EventArgs e)
        {
            thead.Text = "";
            tbody.Text = "";
            theadW.Text = "";
            tbodyW.Text = "";
            DataTable data = new DataTable();
            string[] vend = (sender as Button).Text.Split(new char[] { '_' });
            Session.Add("loteSub", vend[0]);
            Session.Add("vend", vend[1]);
            //LblLote.Text = (string)Session["lote"];
            if ((sender as Button).CssClass == "btn btn-warning")
                data = loadWrong(vend[1]);
            else
                data = loadWrong(vend[1]);
            refreshWrong(data);
        }

        bool flag = false;
        protected void BtnModif_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (((string)Session["loteSub"]).IndexOf("Scoring") >= 0)
            {
                BtnModifScor(btn.CommandName);
                return;
            }
            string query = ""; string dni_old = "";
            DataTable data = new DataTable();

            switch (btn.CommandName)
            {
                case "modif":
                    if (Session["DNI_old"] != null)
                        dni_old = (string)Session["DNI_old"];
                    if(dni_old.Length < 2)
                    {
                        query = string.Format(@"UPDATE a 
                                        SET a.Apellido = '{0}', a.Nombre = '{1}', a.TipoDNI = '{2}', a.DNI = '{3}', a.fechaNacimiento = '{4}',
                                        a.Provincia = '{5}', a.Localidad = '{6}', a.DireCalle = '{7}', a.DireNum = '{8}',
                                        a.DireDpto = '{9}', a.DirePiso = '{10}', a.cp = '{11}', a.Mail = '{12}', a.TarjetaTipo = '{13}',
                                        a.TarjetaNum = '{14}', a.TarjetaVence = '{15}', a.Telefono = '{16}', a.Modif = '*', a.UserModif = '{17}', a.FecModif = '{18}'
                                        FROM VAL_DATA as a
                                        WHERE a.id = {19}",
                                        ape.Value.ToString(), nom.Value.ToString(), tDni.Value.ToString(), dni.Value.ToString(), nac.Value.ToString().Replace(" 12:00:00 a. m.", ""),
                                        prov.Value.ToString(), loc.Value.ToString(), dir.Value.ToString(), num.Value.ToString(), dto.Value.ToString(),
                                        piso.Value.ToString(), cp.Value.ToString(), mail.Value.ToString(), tipoTc.Value.ToString(), nroTc.Value.ToString(),
                                        venTc.Value.ToString().Replace(" 12:00:00 a. m.", ""), tel.Value.ToString(), (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), Hidden1.Value.ToString());
                    }
                    else
                    {
                        query = string.Format(@"UPDATE a 
                                        SET a.Apellido = '{0}', a.Nombre = '{1}', a.TipoDNI = '{2}', a.DNI = '{3}', a.fechaNacimiento = '{4}',
                                        a.Provincia = '{5}', a.Localidad = '{6}', a.DireCalle = '{7}', a.DireNum = '{8}',
                                        a.DireDpto = '{9}', a.DirePiso = '{10}', a.cp = '{11}', a.Mail = '{12}', a.TarjetaTipo = '{13}',
                                        a.TarjetaNum = '{14}', a.TarjetaVence = '{15}', a.Modif = '*', a.UserModif = '{16}', a.FecModif = '{17}', a.DNI_old = '{18}'
                                        FROM VAL_DATA as a
                                        WHERE a.id = {19}",
                                        ape.Value.ToString(), nom.Value.ToString(), tDni.Value.ToString(), dni.Value.ToString(), nac.Value.ToString().Replace(" 12:00:00 a. m.", ""),
                                        prov.Value.ToString(), loc.Value.ToString(), dir.Value.ToString(), num.Value.ToString(), dto.Value.ToString(),
                                        piso.Value.ToString(), cp.Value.ToString(), mail.Value.ToString(), tipoTc.Value.ToString(), nroTc.Value.ToString(),
                                        venTc.Value.ToString().Replace(" 12:00:00 a. m.", ""), (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), dni_old, Hidden1.Value.ToString());
                        Session.Remove("DNI_old");
                    }
                    break;
                case "rech":
                    query = string.Format(@"UPDATE a SET a.EstadoFinal = 'RECHAZADO', a.Modif = '*', a.UserModif = '{0}', a.FecModif = '{1}' FROM VAL_DATA as a WHERE ID = {2}", (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), Hidden1.Value.ToString());
                    break;
                /*case "pend":
                    query = string.Format(@"Update a Set a.Estado = 'PENDIENTE' a.Modif = null FROM VAL_DATA as a WHERE ID = {0}", Hidden1.Value.ToString());
                    break;*/
            }

            
            using (var conn = new SqlConnection(connectionInfo))
            {
                //string q1;
                //q1 = string.Format(@"SELECT count(distinct DNI) FROM VAL_DATA WHERE Vendedor = '{0}' AND Modif is NULL AND Estado <> 'PROCESADO' and EstadoFinal is null and Retorno is null", (string)Session["vend"]);
                try
                {
                    SqlCommand cmmd = new SqlCommand(query, conn);
                    //SqlCommand cmmd2 = new SqlCommand(q1, conn);
                    conn.Open();
                    cmmd.ExecuteNonQuery();
                    //var left = cmmd2.ExecuteScalar();
                    /*string q = "";
                    if ((int)left != 0)
                    {
                        q = string.Format(@"UPDATE a SET a.CANT_DATOS = {0}
                                            FROM CORRECT as a
                                            WHERE USUARIO = '{1}'", (int)left, (string)Session["vend"]);
                    }
                    else
                    {
                        q = string.Format(@"UPDATE a SET a.CANT_DATOS = 0, a.ESTADO = 'LISTO', a.USUARIO_FIN = '{0}', a.FECHA_FIN = '{1}'
                                            FROM CORRECT as a
                                            WHERE USUARIO = '{2}'", (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), (string)Session["vend"]);
                        sendMail((string)Session["vend"], "val");
                        flag = true;
                    }
                    SqlCommand cmmd3 = new SqlCommand(q, conn);
                    cmmd3.ExecuteNonQuery(); */
                    conn.Close();
                    data = loadWrong((string)Session["vend"]);
                    refreshWrong(data);
                    /*if (flag)
                    {
                        checkData();
                        flag = false;
                    }*/
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('Cambios guardados con éxito!');", true);
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblNomLotVal.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                }
            }
        }

        protected void BtnModifScor(string action)
        {
            string query = ""; string dni_old = "";
            DataTable data = new DataTable();

            switch (action)
            {
                case "modif":
                    if (Session["DNI_old"] != null)
                        dni_old = (string)Session["DNI_old"];
                    if (dni_old.Length < 2)
                    {
                        query = string.Format(@"BEGIN TRANSACTION UPDATE a 
                                        SET a.Apellido = '{0}', a.Nombre = '{1}', a.TipoDNI = '{2}', a.DNI = '{3}', a.fechaNacimiento = '{4}',
                                        a.Provincia = '{5}', a.Localidad = '{6}', a.DireCalle = '{7}', a.DireNum = '{8}',
                                        a.DireDpto = '{9}', a.DirePiso = '{10}', a.cp = '{11}', a.Mail = '{12}', a.TarjetaTipo = '{13}',
                                        a.TarjetaNum = '{14}', a.TarjetaVence = '{15}', a.Telefono = '{16}'
                                        FROM VAL_DATA as a
                                        INNER JOIN TEMP_SCOR as b on a.Propuesta = b.Propuesta 
                                        WHERE a.id = {19} and a.Retorno is null
                                        UPDATE b
                                        SET b.ModifScor = '*', b.UserModif = '{17}', b.FecModif = '{18}'
                                        FROM TEMP_SCOR as b
                                        INNER JOIN VAL_DATA as a on b.Propuesta = a.Propuesta
                                        WHERE b.id = {20} and a.Retorno is null COMMIT",
                                        ape.Value.ToString(), nom.Value.ToString(), tDni.Value.ToString(), dni.Value.ToString(), nac.Value.ToString().Replace(" 12:00:00 a. m.", ""),
                                        prov.Value.ToString(), loc.Value.ToString(), dir.Value.ToString(), num.Value.ToString(), dto.Value.ToString(),
                                        piso.Value.ToString(), cp.Value.ToString(), mail.Value.ToString(), tipoTc.Value.ToString(), nroTc.Value.ToString(),
                                        venTc.Value.ToString().Replace(" 12:00:00 a. m.", ""), tel.Value.ToString(), (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), Hidden1.Value.ToString(), Hidden2.Value.ToString());
                    }
                    else
                    {
                        query = string.Format(@"BEGIN TRANSACTION UPDATE a 
                                        SET a.Apellido = '{0}', a.Nombre = '{1}', a.TipoDNI = '{2}', a.DNI = '{3}', a.fechaNacimiento = '{4}',
                                        a.Provincia = '{5}', a.Localidad = '{6}', a.DireCalle = '{7}', a.DireNum = '{8}',
                                        a.DireDpto = '{9}', a.DirePiso = '{10}', a.cp = '{11}', a.Mail = '{12}', a.TarjetaTipo = '{13}',
                                        a.TarjetaNum = '{14}', a.TarjetaVence = '{15}', a.DNI_old = '{16}', a.Telefono = '{17}'
                                        FROM VAL_DATA as a
                                        INNER JOIN TEMP_SCOR as b on a.Propuesta = b.Propuesta 
                                        WHERE a.id = {20} and a.Retorno is null
                                        UPDATE b
                                        SET b.ModifScor = '*', b.UserModif = '{18}', b.FecModif = '{19}'
                                        FROM TEMP_SCOR as b
                                        INNER JOIN VAL_DATA as a on b.Propuesta = a.Propuesta
                                        WHERE b.id = {21} and a.Retorno is null COMMIT",
                                        ape.Value.ToString(), nom.Value.ToString(), tDni.Value.ToString(), dni.Value.ToString(), nac.Value.ToString().Replace(" 12:00:00 a. m.", ""),
                                        prov.Value.ToString(), loc.Value.ToString(), dir.Value.ToString(), num.Value.ToString(), dto.Value.ToString(),
                                        piso.Value.ToString(), cp.Value.ToString(), mail.Value.ToString(), tipoTc.Value.ToString(), nroTc.Value.ToString(),
                                        venTc.Value.ToString().Replace(" 12:00:00 a. m.", ""), dni_old, tel.Value.ToString(), (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), Hidden1.Value.ToString(), Hidden2.Value.ToString());
                        Session.Remove("DNI_old");
                    }
                    break;
                case "rech":
                    query = string.Format(@"BEGIN TRANSACTION UPDATE a SET a.EstadoFinal = 'RECHAZADO' 
                                         FROM VAL_DATA as a INNER JOIN TEMP_SCOR as b on a.Propuesta = b.Propuesta 
                                         WHERE a.id = {2}
                                         UPDATE b SET b.ModifScor = '*', b.UserModif = '{0}', b.FecModif = '{1}'
                                         FROM TEMP_SCOR as b
                                         INNER JOIN VAL_DATA as a on b.Propuesta = a.Propuesta
                                         WHERE a.id= {2} COMMIT", (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), Hidden1.Value.ToString());
                    break;
                /*case "pend":
                    query = string.Format(@"UPDATE a
                                        SET a.FOLLOW_STATUS = 'PENDIENTE' a.ModifScor = null
                                        FROM TEMP_SCOR as a
                                        INNER JOIN VAL_DATA as b on a.DNI = b.DNI
                                        WHERE b.id = {0} and b.Retorno is null", Hidden1.Value.ToString());
                    break;*/
            }

            using (var conn = new SqlConnection(connectionInfo))
            {
                //string q1;
                //q1 = string.Format(@"SELECT count(distinct DNI) FROM VAL_DATA WHERE Vendedor = '{0}' AND Modif is NULL AND Estado <> 'PROCESADO' and EstadoFinal is null and Retorno is null", (string)Session["vend"]);
                try
                {
                    SqlCommand cmmd = new SqlCommand(query, conn);
                    //SqlCommand cmmd2 = new SqlCommand(q1, conn);
                    conn.Open();
                    cmmd.ExecuteNonQuery();
                    //var left = cmmd2.ExecuteScalar();
                    /*string q = "";
                    if ((int)left != 0)
                    {
                        q = string.Format(@"UPDATE a SET a.CANT_DATOS = {0}
                                            FROM CORRECT as a
                                            WHERE USUARIO = '{1}'", (int)left, (string)Session["vend"]);
                    }
                    else
                    {
                        q = string.Format(@"UPDATE a SET a.CANT_DATOS = 0, a.ESTADO = 'LISTO', a.USUARIO_FIN = '{0}', a.FECHA_FIN = '{1}'
                                            FROM CORRECT as a
                                            WHERE USUARIO = '{2}'", (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), (string)Session["vend"]);
                        sendMail((string)Session["vend"], "val");
                        flag = true;
                    }
                    SqlCommand cmmd3 = new SqlCommand(q, conn);
                    cmmd3.ExecuteNonQuery(); */
                    conn.Close();
                    data = loadWrong((string)Session["vend"]);
                    refreshWrong(data);
                    /*if (flag)
                    {
                        checkData();
                        flag = false;
                    }*/
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('Cambios guardados con éxito!');", true);
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblNomLotVal.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                }
            }
        }

        protected DataTable loadWrong(string vend)
        {
            DataTable data = new DataTable();
            string q = "";
            if (((string)Session["loteSub"]).IndexOf("Scoring") >= 0)
            {
                if (vend == "Total")
                    q = "SELECT * FROM TEMP_SCOR as a INNER JOIN VAL_DATA as b on a.Propuesta = b.Propuesta WHERE a.FOLLOW_STATUS <> 'Positiva' and a.Retorno_Scor is null and ModifScor is null and b.EstadoFinal is null ";
                else
                    q = "SELECT * FROM TEMP_SCOR as a INNER JOIN VAL_DATA as b on a.Propuesta = b.Propuesta WHERE b.Vendedor = '" + vend + "' and a.FOLLOW_STATUS <> 'Positiva' and a.Retorno_Scor is null and ModifScor is null and b.EstadoFinal is null ";
            }
            else
            {
                if(vend == "Total")
                    q = "SELECT * FROM VAL_DATA WHERE Estado <> 'PROCESADO' and EstadoFinal is null and Modif is null";
                else
                    q = "SELECT * FROM VAL_DATA WHERE Estado <> 'PROCESADO' and EstadoFinal is null and Vendedor = '" + vend + "' and Modif is null";
            }
            using (var conn = new SqlConnection(connectionInfo))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(q, conn);
                    conn.Open();
                    data.Load(cmd.ExecuteReader());
                    conn.Close();
                    return data;
                }
                catch (SqlException sqlEx)
                {
                    LblNomLotVal.Text = "Query: " + q + ". ERROR: " + sqlEx.Message;
                    return null;
                }
            }
        }
        protected void refreshWrong(DataTable dt)
        {
            int count = 0;
            StringBuilder html = new StringBuilder();

            html.Append("<thead>");
            html.Append("<tr>");

            for (int i = 0; i < dt.Columns.Count - 4; i++)
            {
                html.Append("<th>");
                html.Append(dt.Columns[i].ColumnName.ToString());
                html.Append("</th>");
            }
            html.Append("</tr>");
            html.Append("</thead>");
            theadN.Text = html.ToString();
            //tfootN.Text = html.ToString();

            html.Clear();
            html.Append("<tbody>");
            bool flag = false;
            string cond = ""; string modif = ""; string estado = "";
            if (((string)Session["loteSub"]).IndexOf("Scoring") >= 0)
            {
                estado = "FOLLOW_STATUS";
                cond = "Positiva";
                modif = "ModifScor";
            }
            else
            {
                estado = "Estado";
                cond = "PROCESADO";
                modif = "Modif";
            }

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                if (dt.Rows[j][estado].ToString() != cond)
                {
                    count++;
                    flag = true;
                    if (dt.Rows[j][modif].ToString() == "*")
                    {
                        html.Append("<tr title=\"");
                        if (((string)Session["loteSub"]).IndexOf("Scoring") >= 0)
                        {
                            html.Append(dt.Rows[j]["FOLLOW_DESC"].ToString());
                            html.Append(". ");
                            html.Append(dt.Rows[j]["AUDITORIA_DESC"].ToString());
                            html.Append("\" style=\"color:green;\" data-toggle=\"modal\" data-id=\"");
                            html.Append(count.ToString());
                            html.Append("\" data-target=\"#modalRow\" >");
                        }
                        else
                        {
                            html.Append(dt.Rows[j]["ObservacionEstado"].ToString());
                            html.Append("\" style=\"color:green;\" data-toggle=\"modal\" data-id=\"");
                            html.Append(count.ToString());
                            html.Append("\" data-target=\"#modalRow\" >");
                        }
                    }
                    else
                    {
                        html.Append("<tr title=\"");
                        if (((string)Session["loteSub"]).IndexOf("Scoring") >= 0)
                        {
                            html.Append(dt.Rows[j]["FOLLOW_DESC"].ToString());
                            html.Append(". ");
                            html.Append(dt.Rows[j]["AUDITORIA_DESC"].ToString());
                            if (dt.Rows[j]["FOLLOW_STATUS"].ToString() == "PENDIENTE")
                                html.Append("\" style=\"color:orange;\" data-toggle=\"modal\" data-id=\"");
                            else
                                html.Append("\" style=\"color:red;\" data-toggle=\"modal\" data-id=\"");
                            html.Append(count.ToString());
                            html.Append("\" data-target=\"#modalRow\" >");
                        }
                        else
                        {
                            html.Append(dt.Rows[j]["ObservacionEstado"].ToString());
                            if (dt.Rows[j]["Estado"].ToString() == "PENDIENTE")
                                html.Append("\" style=\"color:orange;\" data-toggle=\"modal\" data-id=\"");
                            else
                                html.Append("\" style=\"color:red;\" data-toggle=\"modal\" data-id=\"");
                            html.Append(count.ToString());
                            html.Append("\" data-target=\"#modalRow\" >");
                        }
                    }
                }
                for (int k = 0; k < dt.Columns.Count - 4; k++)
                {
                    if (flag)
                    {
                        html.Append("<td>");
                        html.Append(dt.Rows[j][k].ToString());
                        html.Append("</td>");
                    }
                }
                if (flag)
                    html.Append("</tr>");
                flag = false;
            }
            html.Append("</tbody>");
            tbodyN.Text = html.ToString();
            html.Clear();
        }

        protected void BtnCargaScor_Click(object sender, EventArgs e)
        {
            if (FileUpload.HasFile)
            {
                foreach (HttpPostedFile uploadedFile in FileUpload.PostedFiles)
                {
                    string FileName = Path.GetFileName(uploadedFile.FileName);
                    string Extension = Path.GetExtension(uploadedFile.FileName);
                    if (Extension != ".txt")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Formato de Archivo no soportado.');", true);
                        return;
                    }
                    string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                    string FilePath = Server.MapPath(FolderPath + FileName);
                    FileName = FileName.Replace(".txt", "");
                    string query = "";
                    bool exists = false;
                    using (var conn = new SqlConnection(connectionInfo))
                    {
                        query = string.Format(@"SELECT NomLote FROM TEMP_SCOR WHERE NomLote = '{0}'", FileName);
                        SqlCommand comm = new SqlCommand(query, conn);
                        conn.Open();
                        SqlDataReader reg = null;
                        reg = comm.ExecuteReader();

                        if (reg.Read())
                        {
                            exists = true;
                        }
                        conn.Close();
                    }
                    if (exists == false)
                    {
                        Label.Visible = true;
                        Session.Add("loteSub", FileName);
                        //LblLote.Text = (string)Session["loteSub"];
                        //LblNomLotVal.Text = (string)Session["loteSub"];
                        uploadedFile.SaveAs(FilePath);
                        Import_To_Grid(FilePath);
                        theadN.Text = "";
                        tbodyN.Text = "";
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] El lote ya existe en la Base de Datos.');", true);
                        return;
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Por Favor Seleccione un Archivo');", true);
                return;
            }
        }

        protected void Import_To_Grid(string FilePath)
        {
            DataTable dt = new DataTable();
            string[] cols = null;

            var lines = File.ReadAllLines(FilePath);

            if (lines.Count() > 0)
            {
                cols = lines[0].Split(new char[] { ';' });

                foreach (var col in cols)
                    dt.Columns.Add(col);
            }

            for (int i = 1; i < lines.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                string[] values = lines[i].Split(new char[] { ';' });

                for (int j = 0; j < values.Count() && j < cols.Count(); j++)
                    dr[j] = values[j];

                dt.Rows.Add(dr);
            }

            DataColumn newCol = new DataColumn("NomLote", typeof(System.String));
            newCol.DefaultValue = (string)Session["loteSub"];
            dt.Columns.Add(newCol);

            string qry = ""; string q = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                using (var conn = new SqlConnection(connectionInfo))
                {
                    qry = string.Format(@"select Propuesta from TEMP_SCOR where Propuesta = '{0}'", dt.Rows[i]["PROPUESTA"].ToString());
                    SqlCommand cmd = new SqlCommand(qry, conn);
                    conn.Open();
                    SqlDataReader reg = null;
                    reg = cmd.ExecuteReader();
                    if (reg.Read())
                    {
                        q = string.Format(@"update a set a.Retorno_Scor = '*' from TEMP_SCOR as a where a.PROPUESTA = '{0}'", reg["PROPUESTA"].ToString());
                    }
                    conn.Close();
                    if(q.Length > 5)
                    {
                        SqlCommand cmd2 = new SqlCommand(q, conn);
                        conn.Open();
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        q = "";
                    }
                }
            }

            using (var conn = new SqlConnection(connectionInfo))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
                {
                    try
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "dbo.TEMP_SCOR";

                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("NomLote", "NomLote");
                        sqlBulkCopy.ColumnMappings.Add("OFFICE", "OFFICE");
                        sqlBulkCopy.ColumnMappings.Add("CIA", "CIA");
                        sqlBulkCopy.ColumnMappings.Add("FOLLOW_FECHA", "FOLLOW_FECHA");
                        sqlBulkCopy.ColumnMappings.Add("PROPUESTA", "PROPUESTA");
                        sqlBulkCopy.ColumnMappings.Add("DNI", "DNI");
                        sqlBulkCopy.ColumnMappings.Add("PLAN", "PLAN");
                        sqlBulkCopy.ColumnMappings.Add("FOLLOW_NUMERO", "FOLLOW_NUMERO");
                        sqlBulkCopy.ColumnMappings.Add("FOLLOW_STATUS", "FOLLOW_STATUS");
                        sqlBulkCopy.ColumnMappings.Add("AUDIOS", "AUDIOS");
                        sqlBulkCopy.ColumnMappings.Add("RESOLUCION", "RESOLUCION");
                        sqlBulkCopy.ColumnMappings.Add("OBSERVACION", "OBSERVACION");
                        sqlBulkCopy.ColumnMappings.Add("FOLLOW_DESC", "FOLLOW_DESC");
                        sqlBulkCopy.ColumnMappings.Add("AUDITORIA_DESC", "AUDITORIA_DESC");
                        RemoveNullColumnFromDataTable(dt);
                        conn.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        conn.Close();
                    }
                    catch (SqlException sqlEx)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                        string sqlerro = "alert('[ERROR DESCRIPTION] " + sqlEx.Message + "');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", sqlerro, true);
                    }
                }
            }

            int countNeg = 0;
            int countPos = 0;

            if (dt.Rows.Count > 1)
            {
                DataTable data = new DataTable();
                data = innerjoin(dt);
                LtlVta.Text = data.Rows.Count.ToString();
                StringBuilder html = new StringBuilder();
                StringBuilder html2 = new StringBuilder();

                html.Append("<thead>");
                html.Append("<tr>");
                for (int j = 0; j < data.Columns.Count - 4; j++)
                {
                    html.Append("<th>");
                    html.Append(data.Columns[j].ColumnName.ToString());
                    html.Append("</th>");
                }
                html.Append("</tr>");
                html.Append("</thead>");
                thead.Text = html.ToString();
                theadW.Text = html.ToString();
                //tfoot.Text = html.ToString();
                //tfootW.Text = html.ToString();

                html.Clear();
                html.Append("<tbody>");
                html2.Append("<tbody>");
                bool flag = false;

                for (int k = 0; k < data.Rows.Count; k++)
                {
                    if (data.Rows[k]["FOLLOW_STATUS"].ToString() != "Positiva")
                    {
                        countNeg++;
                        flag = true;
                        html2.Append("<tr title=\"");
                        html2.Append(data.Rows[k]["FOLLOW_DESC"].ToString());
                        html2.Append(". ");
                        html2.Append(data.Rows[k]["AUDITORIA_DESC"].ToString());
                        html2.Append("\" style=\"color:red;\" data-toggle=\"modal\" data-id=\"");
                        html2.Append(countNeg.ToString());
                        html2.Append("\" data-target=\"#modalRow\" >");
                    }
                    else
                    {
                        countPos++;
                        html.Append("<tr>");
                    }
                    for (int l = 0; l < data.Columns.Count - 4; l++)
                    {
                        if (flag)
                        {
                            html2.Append("<td>");
                            html2.Append(data.Rows[k][l].ToString());
                            html2.Append("</td>");
                        }
                        if (data.Columns[l].ColumnName == "TarjetaNum")
                        {
                            html.Append("<td>");
                            html.Append("XXXX XXXX XXXX XXXX");
                            html.Append("</td>");
                        }
                        else
                        {
                            html.Append("<td>");
                            html.Append(data.Rows[k][l].ToString());
                            html.Append("</td>");
                        }
                    }
                    if (flag)
                        html2.Append("</tr>");
                    html.Append("</tr>");
                    flag = false;
                }
                if (html2.Length < 3)
                    LtlScorWrong.Text = "No hay registros para corregir.<i class=\"glyphicon glyphicon-check\" style=\"color:greenM\"></i>";
                html.Append("</tbody>");
                html2.Append("</tbody>");
                tbody.Text = html.ToString();
                tbodyW.Text = html2.ToString();
                html.Clear();
                html2.Clear();

                if (countPos != 0)
                    LtlOk.Text = countPos.ToString();
                if (countNeg != 0)
                    LtlNeg.Text = countNeg.ToString();

                LtlDataLoaded.Visible = false;

                using (var conn = new SqlConnection(connectionInfo))
                {
                    string query = "";
                    
                   query = string.Format(@"INSERT INTO ALERTS (FECHA, TIPO, USUARIO, LOTE, TOT_DATOS, DESCRIPCION)
                            VALUES('{0}', '{1}', '{2}', '{3}', {4}, '{5}')", DateTime.Now.ToString("dd/MM/yyyy"),
                            "CARGA", (string)Session["user_logged"], (string)Session["loteSub"], data.Rows.Count, "Carga de Scoring");
                    
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                }
                dt.Clear();
                data.Clear();
                if (countNeg > 0)
                    sendMail((string)Session["loteSub"], "subida");
            }
            else
            {
                LtlDataLoaded.Visible = true;
                LtlDataLoaded.Text = "No hay registros Cargados.<i class=\"glyphicon glyphicon-check\" style=\"color:greenM\"></i>";
            }
        }

        protected void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][4] == DBNull.Value)
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        protected DataTable innerjoin(DataTable dt)
        {
            DataTable data = new DataTable();
            using (var conn = new SqlConnection(connectionInfo))
            {
                string query = "";
                query = string.Format(@"Select * from TEMP_SCOR as a 
                                        inner join VAL_DATA as b on a.Propuesta = b.Propuesta
                                        where a.NomLote = '{0}' and b.Retorno is null and a.Retorno_Scor is null", (string)Session["loteSub"]);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                data.Load(cmd.ExecuteReader());
                conn.Close();
                return data;
            }
        }

        protected void sendMail(string lote, string tipo)
        {
            try
            {
                if (tipo == "subida")
                {
                    MailMessage msg = new MailMessage();
                    msg.Subject = "[" + lote + "] - Negativas para Corregir";
                    msg.From = new MailAddress("sales@touchedbpo.com");
                    msg.To.Add("mariano@touchedbpo.com");
                    msg.Body = "Hay nuevos registros praa corregir, por favor ingrese a: http://192.168.10.210";
                    msg.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    NetworkCredential netCre = new NetworkCredential("sales.tbpo@gmail.com", "123Cambiazo");
                    smtp.Credentials = netCre;

                    smtp.Send(msg);
                }
                else
                {
                    MailMessage msg = new MailMessage();
                    if (tipo == "scor")
                        msg.Subject = "[" + (string)Session["vend"] + "] - Negativas Corregidas";
                    if (tipo == "val")
                        msg.Subject = "[" + (string)Session["vend"] + "] - Errores e Inconsistencias Corregidos";
                    msg.From = new MailAddress("sales@touchedbpo.com");
                    msg.To.Add("nicolas@pampa.com");
                    //if (tipo == "scor")
                    //    msg.To.Add("nicolasmp920@gmail.com");
                    if (tipo == "scor")
                        msg.Body = "El vendedor "+ (string)Session["vend"] +" finalizó las correcciones Negativas de Scoring.";
                    if (tipo == "val")
                        msg.Body = "El vendedor "+ (string)Session["vend"] +" finalizó las correcciones de los Errores e Inconsistencias de Retorno.";
                    msg.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.EnableSsl = true;
                    NetworkCredential netCre = new NetworkCredential("sales.tbpo@gmail.com", "123Cambiazo");
                    smtp.Credentials = netCre;

                    smtp.Send(msg);

                    string alert;
                    alert = "alert('Todo Listo. Se informaron las correcciones realizadas vía E-mail')";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "WARNING", alert, true);
                }
            }
            catch (Exception ex)
            {
                string alert = "alert('Error al enviar el mail: " + ex.Message + "')";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "WARNING", alert, true);
            }
        }

        protected void BtnLogOut_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Response.Redirect("default.aspx");
        }

        protected void BtnMdfDNI_Click(object sender, EventArgs e)
        {
            Session.Add("DNI_old", dni.Value.ToString());
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "none", "<script>changeDNI();</script>", false);
        }
    }
}