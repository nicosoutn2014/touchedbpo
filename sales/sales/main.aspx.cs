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
using System.Collections;

namespace sales
{
    public partial class _default : System.Web.UI.Page
    {
        public string connectionInfo = WebConfigurationManager.AppSettings["connString"];
        public string user = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Count < 1)
                Response.Redirect("default.aspx");
            if ((string)Session["user_profile"] == "OPERADOR")
                Response.Redirect("main2.aspx");
            user = "https://avatars.discourse.org/v2/letter/" + ((string)Session["user_logged"])[0] + "/f6c823/45.png";
            checkModif();
        }

        protected void checkModif()
        {
            string q1 = string.Format(@"select top(1) LOTE from ALERTS
                                        where LOTE not like '%Scoring%'
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


        protected void BtnCargaVta_Click(object sender, EventArgs e)
        {
            if (FileUpload2.HasFile)
            {
                foreach (HttpPostedFile uploadedFile in FileUpload2.PostedFiles)
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
                    if(FileName.Contains("Retorno"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] EL ARCHIVO TIENE QUE SER DE VENTAS NO DE RETORNO.');", true);
                        return;
                    }
                    string query = "";
                    bool exists = false;
                    using (var conn = new SqlConnection(connectionInfo))
                    {
                        query = string.Format(@"SELECT NomLote FROM VAL_DATA WHERE NomLote = '{0}'", FileName);
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
                        Session.Add("loteSub", FileName);
                        uploadedFile.SaveAs(FilePath);
                        Import_To_Grid(FilePath);
                        File.Delete(FilePath);
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

        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        protected void BtnCargaVal_Click(object sender, EventArgs e)
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
                    Session.Add("loteSub", FileName);
                    uploadedFile.SaveAs(FilePath);
                    richData(FilePath);
                    //File.Delete(FilePath);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Por Favor Seleccione un Archivo');", true);
                return;
            }
        }

        private void richData (string FilePath)
        {
            DataTable dt = new DataTable();
            DataTable data = new DataTable();
            string[] cols = null;

            var lines = File.ReadAllLines(FilePath);

            if (lines.Count() > 0)
            {
                cols = lines[0].Split(new char[] { '\t' });

                foreach (var col in cols)
                    dt.Columns.Add(col);
            }

            for (int i = 1; i < lines.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                string[] values = lines[i].Split(new char[] { '\t' });

                for (int j = 0; j < values.Count() && j < cols.Count(); j++)
                    dr[j] = values[j];

                dt.Rows.Add(dr);
            }

            DataColumn newCol = new DataColumn("EstadoFinal", typeof(System.String));
            newCol.DefaultValue = DBNull.Value;
            dt.Columns.Add(newCol);

            foreach (DataRow row in dt.Rows)
            {
                if (row["Resultado"].ToString().Length < 2)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] El Campo RESULTADO del cliente DNI "+ row["DNI"].ToString() +" no puede estar vacío. Operación Cancelada.');", true);
                    return;
                }
            }

            int count = 0;
            //int left = 0;
            string query = "";
            //enriquezco los datos
            using (var conn = new SqlConnection(connectionInfo))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Resultado"].ToString() == "PROCESADO")
                    {
                        query = string.Format(@"Update a
                                    set a.Propuesta = '{0}', a.Estado = '{1}', a.Definicion = '{2}'
                                    from VAL_DATA as a
                                    where a.DNI = '{3}'", row["Motivo"].ToString(), row["Resultado"].ToString(),
                                    row["Definicion"].ToString(), row["DNI"].ToString());
                    }
                    else
                    {
                        if(row["Resultado"].ToString() == "Existente")
                        {
                            query = string.Format(@"Update a
                                    set a.ObservacionEstado = '{0}', a.Estado = '{1}', a.Definicion = '{2}', a.EstadoFinal = 'RECHAZADO'
                                    from VAL_DATA as a
                                    where a.DNI = '{3}'", row["Motivo"].ToString(), row["Resultado"].ToString(),
                                    row["Definicion"].ToString(), row["DNI"].ToString());
                        }
                        else
                        {
                            query = string.Format(@"Update a
                                    set a.ObservacionEstado = '{0}', a.Estado = '{1}', a.Definicion = '{2}'
                                    from VAL_DATA as a
                                    where a.DNI = '{3}'", row["Motivo"].ToString(), row["Resultado"].ToString(),
                                    row["Definicion"].ToString(), row["DNI"].ToString());
                        }
                    }
                    SqlCommand comm = new SqlCommand(query, conn);
                    conn.Open();
                    int a = comm.ExecuteNonQuery();
                    if (a > 0)
                        count++;
                    conn.Close();
                }

                string query2 = string.Format(@"INSERT INTO ALERTS (FECHA, TIPO, USUARIO, LOTE, TOT_DATOS, DESCRIPCION, CANT_ENRIQ)
                                        VALUES('{0}', '{1}', '{2}', '{3}', {4}, '{5}', {6})", DateTime.Now.ToString("dd/MM/yyyy"),
                                        "CARGA", (string)Session["user_logged"], (string)Session["loteSub"], dt.Rows.Count, "Enriquecer Ventas", count);

                SqlCommand comm2 = new SqlCommand(query2, conn);
                conn.Open();
                comm2.ExecuteNonQuery();
                conn.Close();
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[INFO] Se Enriquecieron "+ count.ToString() +" registros.');", true);
        }

        private void Import_To_Grid(string FilePath)
        {
            DataTable dt = new DataTable();
            DataTable data = new DataTable();
            string[] cols = null;

            var lines = File.ReadAllLines(FilePath);

            if (lines.Count() > 0)
            {
                cols = lines[0].Split(new char[] { '\t' });

                foreach (var col in cols)
                    dt.Columns.Add(col);
            }

            for (int i = 1; i < lines.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                string[] values = lines[i].Split(new char[] { '\t' });

                for (int j = 0; j < values.Count() && j < cols.Count(); j++)
                    dr[j] = values[j];

                dt.Rows.Add(dr);
            }

            //DataTable dtRemoved = RemoveDuplicateRows(dt, "DNI");
            //dt.Clear();
            //dt = dtRemoved;

            /*foreach (DataRow row in dt.Rows)
            {
                if (row["Vendedor"].ToString().Length < 2)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] El Campo VENDEDOR no puede estar vacío!');", true);
                    return;
                }
            }*/

            DataColumn newCol = new DataColumn("NomLote", typeof(System.String));
            //DataColumn newCol2 = new DataColumn("EstadoFinal", typeof(System.String));
            newCol.DefaultValue = (string)Session["loteSub"];
            //newCol2.DefaultValue = DBNull.Value;
            dt.Columns.Add(newCol);
            //dt.Columns.Add(newCol2);
            //pego nro de propuesta
            /*for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["Motivo"].ToString().Length < 6)
                {
                    dt.Rows[i]["Propuesta"] = dt.Rows[i]["Motivo"];
                    dt.Rows[i]["Motivo"] = DBNull.Value;
                }
                if (dt.Rows[i]["Resultado"].ToString() == "Existente")
                {
                    dt.Rows[i]["EstadoFinal"] = "RECHAZADO";
                }
                else
                    dt.Rows[i]["EstadoFinal"] = DBNull.Value;

            }*/

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string query = ""; string q = "";
                using (var conn = new SqlConnection(connectionInfo))
                {
                    query = string.Format(@"select DNI from VAL_DATA where DNI = '{0}'", dt.Rows[i]["DNI"].ToString());
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reg = null;
                    reg = cmd.ExecuteReader();
                    if (reg.Read())
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[WARNING] El DNI "+ dt.Rows[i]["DNI"].ToString() + " ya existe en la Base. Se ignora el dato.');", true);
                        addExistente(dt.Rows[i]);
                        dt.Rows[i].Delete();
                        /*q = string.Format(@"update a set a.Retorno = '*', a.Propuesta = '{0}' from VAL_DATA as a where a.DNI = '{1}'", dt.Rows[i]["Propuesta"].ToString(), reg["DNI"].ToString());
                        SqlCommand cmd2 = new SqlCommand(q, conn);
                        cmd2.ExecuteNonQuery();*/
                    }
                    conn.Close();
                }
            }

            using (var conn = new SqlConnection(connectionInfo))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
                {
                    try
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "dbo.VAL_DATA";

                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("NomLote", "NomLote");
                        sqlBulkCopy.ColumnMappings.Add("ProductorCod", "ProductorCod");
                        sqlBulkCopy.ColumnMappings.Add("VendedorCod", "VendedorCod");
                        sqlBulkCopy.ColumnMappings.Add("Producto", "Producto");
                        sqlBulkCopy.ColumnMappings.Add("fechaVenta", "fechaVenta");
                        sqlBulkCopy.ColumnMappings.Add("Apellido", "Apellido");
                        sqlBulkCopy.ColumnMappings.Add("Nombre", "Nombre");
                        sqlBulkCopy.ColumnMappings.Add("TipoDNI", "TipoDNI");
                        sqlBulkCopy.ColumnMappings.Add("DNI", "DNI");
                        sqlBulkCopy.ColumnMappings.Add("fechaNacimiento", "fechaNacimiento");
                        sqlBulkCopy.ColumnMappings.Add("Provincia", "Provincia");
                        sqlBulkCopy.ColumnMappings.Add("Localidad", "Localidad");
                        sqlBulkCopy.ColumnMappings.Add("DireCalle", "DireCalle");
                        sqlBulkCopy.ColumnMappings.Add("DireNum", "DireNum");
                        sqlBulkCopy.ColumnMappings.Add("DireDpto", "DireDpto");
                        sqlBulkCopy.ColumnMappings.Add("DirePiso", "DirePiso");
                        sqlBulkCopy.ColumnMappings.Add("cp", "cp");
                        sqlBulkCopy.ColumnMappings.Add("Mail", "Mail");
                        sqlBulkCopy.ColumnMappings.Add("TarjetaTipo", "TarjetaTipo");
                        sqlBulkCopy.ColumnMappings.Add("TarjetaNum", "TarjetaNum");
                        sqlBulkCopy.ColumnMappings.Add("TarjetaVence", "TarjetaVence");
                        sqlBulkCopy.ColumnMappings.Add("IdContacto", "IdContacto");
                        sqlBulkCopy.ColumnMappings.Add("Propuesta", "Propuesta");
                        sqlBulkCopy.ColumnMappings.Add("Observaciones", "Observaciones");
                        sqlBulkCopy.ColumnMappings.Add("ObservacionesBo", "ObservacionesBo");
                        sqlBulkCopy.ColumnMappings.Add("Telefono", "Telefono");
                        sqlBulkCopy.ColumnMappings.Add("estado_civil", "estado_civil");
                        sqlBulkCopy.ColumnMappings.Add("iva", "iva");
                        sqlBulkCopy.ColumnMappings.Add("nacimiento_lugar", "nacimiento_lugar");
                        sqlBulkCopy.ColumnMappings.Add("profesion", "profesion");
                        sqlBulkCopy.ColumnMappings.Add("sexo", "sexo");
                        sqlBulkCopy.ColumnMappings.Add("TelefonoAlt", "TelefonoAlt");
                        sqlBulkCopy.ColumnMappings.Add("Banco", "Banco");
                        sqlBulkCopy.ColumnMappings.Add("Vendedor", "Vendedor");
                        //sqlBulkCopy.ColumnMappings.Add("Resultado", "Estado");
                        //sqlBulkCopy.ColumnMappings.Add("Motivo", "ObservacionEstado");
                        //sqlBulkCopy.ColumnMappings.Add("EstadoFinal", "EstadoFinal");
                        RemoveNullColumnFromDataTable(dt);
                        conn.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        conn.Close();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[INFO] Se cargaron "+ dt.Rows.Count.ToString() +" registros.');", true);
                        //LtlVta.Text = dt.Rows.Count.ToString();
                        //data = loadData(data);
                    }
                    catch (SqlException sqlEx)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                        string sqlerro = "alert('[ERROR DESCRIPTION] " + sqlEx.Message + "');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", sqlerro, true);
                    }
                }
            }
            /*
            int countErro = 0; //contador para errores e inconsistencias
            int countProc = 0; //contador para procesadas
            int countExist = 0; //contador para existentes
            dt = data;
            //Llenar gridview desde dataset
            StringBuilder html = new StringBuilder();
            StringBuilder html2 = new StringBuilder();

            html.Append("<thead>");
            html.Append("<tr>");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                html.Append("<th>");
                html.Append(dt.Columns[i].ColumnName.ToString());
                html.Append("</th>");
            }
            html.Append("</tr>");
            html.Append("</thead>");
            thead.Text = html.ToString();
            theadN.Text = html.ToString();
            //tfoot.Text = html.ToString();
            //tfootN.Text = html.ToString();

            html.Clear();
            html.Append("<tbody>");
            html2.Append("<tbody>");
            bool flag = false;

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                if (dt.Rows[j]["Estado"].ToString() != "PROCESADO")
                {
                    if (dt.Rows[j]["Estado"].ToString() != "Existente")
                    {
                        countErro++;
                        flag = true;
                        if (dt.Rows[j]["Modif"].ToString() == "*")
                        {
                            html2.Append("<tr title=\"");
                            html2.Append(dt.Rows[j]["ObservacionEstado"].ToString());
                            html2.Append("\" style=\"color:green;\" data-toggle=\"modal\" data-id=\"");
                            html2.Append(countErro.ToString());
                            html2.Append("\" data-target=\"#modalRow\" >");
                        }
                        else
                        {
                            html2.Append("<tr title=\"");
                            html2.Append(dt.Rows[j]["ObservacionEstado"].ToString());
                            html2.Append("\" style=\"color:red;\" data-toggle=\"modal\" data-id=\"");
                            html2.Append(countErro.ToString());
                            html2.Append("\" data-target=\"#modalRow\" >");
                        }
                    }
                    if (dt.Rows[j]["Estado"].ToString() == "Existente")
                        countExist++;
                }
                html.Append("<tr>");
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    if (flag)
                    {
                        html2.Append("<td>");
                        html2.Append(dt.Rows[j][k].ToString());
                        html2.Append("</td>");
                    }
                    html.Append("<td>");
                    html.Append(dt.Rows[j][k].ToString());
                    html.Append("</td>");
                }
                if (flag)
                    html2.Append("</tr>");
                html.Append("</tr>");
                flag = false;
            }
            html.Append("</tbody>");
            html2.Append("</tbody>");
            tbody.Text = html.ToString();
            tbodyN.Text = html2.ToString();
            html.Clear();
            html2.Clear();

            countProc = dt.Rows.Count - countErro - countExist;
            if (countProc != 0)
                LtlOk.Text = countProc.ToString();
            if (countErro != 0)
                LtlErro.Text = countErro.ToString();
            if (countExist != 0)
                LtlExis.Text = countExist.ToString();

            */

            using (var conn = new SqlConnection(connectionInfo))
            {
                string query = "";

                query = string.Format(@"INSERT INTO ALERTS (FECHA, TIPO, USUARIO, LOTE, TOT_DATOS, DESCRIPCION)
                                        VALUES('{0}', '{1}', '{2}', '{3}', {4}, '{5}')", DateTime.Now.ToString("dd/MM/yyyy"),
                                        "CARGA", (string)Session["user_logged"], (string)Session["loteSub"], dt.Rows.Count, "Carga de Ventas");
                /*
                if (countErro > 0)
                {
                    query = string.Format(@"INSERT INTO ALERTS (FECHA, TIPO, USUARIO, LOTE, TOT_DATOS, CANT_DATOS_ERRO, A_CORREGIR, DESCRIPCION, ESTADO)
                                        VALUES('{0}', '{1}', '{2}', '{3}', {4}, {5}, {6}, '{7}', '{8}')", DateTime.Now.ToString("dd/MM/yyyy"),
                                        "CORREGIR", (string)Session["user_logged"], (string)Session["lote"], dt.Rows.Count, countErro, countErro, "Corregir Errores e Inconsistencias", "PENDIENTE");
                }
                else
                {
                    query = string.Format(@"INSERT INTO ALERTS (FECHA, TIPO, USUARIO, LOTE, TOT_DATOS, CANT_DATOS_ERRO, A_CORREGIR, DESCRIPCION, ESTADO)
                                        VALUES('{0}', '{1}', '{2}', '{3}', {4}, {5}, {6}, '{7}', '{8}')", DateTime.Now.ToString("dd/MM/yyyy"),
                                        "CORREGIR", (string)Session["user_logged"], (string)Session["lote"], dt.Rows.Count, countErro, countErro, "Corregir Errores e Inconsistencias", "LISTO");
                }
                */
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            dt.Clear();
            data.Clear();
            //if (countErro > 0)
              //  sendMail(countErro);
        }

        protected void addExistente(DataRow row)
        {
            using (var conn = new SqlConnection(connectionInfo))
            {
                string query = string.Format(@"insert into EXISTENTES (NomLote, ProductorCod, VendedorCod, Producto, fechaVenta, Apellido, Nombre, TipoDNI, DNI, fechaNacimiento, Provincia,
                        Localidad, DireCalle, DireNum, DireDpto, DirePiso, cp, Mail, TarjetaTipo, TarjetaNum, TarjetaVence, IdContacto,
						Propuesta, Observaciones, ObservacionesBo, Telefono, estado_civil, iva, nacimiento_lugar, profesion, sexo, 
						TelefonoAlt, Banco, Vendedor, Estado)
                        values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}',
                                '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}',
                                '{32}', '{33}', '{34}')", (string)Session["loteSub"], row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(),
                                row[5].ToString(), row[6].ToString(), row[7].ToString(), row[8].ToString(), row[9].ToString(), row[10].ToString(), row[11].ToString(), row[12].ToString(),
                                row[13].ToString(), row[14].ToString(), row[15].ToString(), row[16].ToString(), row[17].ToString(), row[18].ToString(), row[19].ToString(), row[20].ToString(),
                                row[21].ToString(), row[22].ToString(), row[23].ToString(), row[24].ToString(), row[25].ToString(), row[26].ToString(), row[27].ToString(), row[28].ToString(),
                                row[29].ToString(), row[30].ToString(), row[31].ToString(), row[32].ToString(), "Existente");
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        protected void sendMail(int cant)
        {
            try
            {
                MailMessage msg = new MailMessage();
                //msg.Subject = "[" + (string)Session["lote"] + "] - " + cant.ToString() + " Errores e Inconsistencias";
                msg.Subject = "[" + (string)Session["lote"] + "] - " + cant.ToString() + " Errores e Inconsistencias";
                msg.From = new MailAddress("sales@touchedbpo.com");
                msg.To.Add("sistemas@touchedbpo.com");
                //msg.To.Add("mariano@touchedbpo.com");
                //msg.To.Add("jonathan@touchedbpo.com");
                //msg.Body = "Hay nuevos registros para corregir, para efectuar las correcciones por favor ingrese a: http://192.168.10.210";
                msg.Body = "Se cargaron las ventas del lote: "+ (string)Session["lote"];
                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                NetworkCredential netCre = new NetworkCredential("sales.tbpo@gmail.com", "123Cambiazo");
                smtp.Credentials = netCre;

                smtp.Send(msg);

                //string alert;
                //alert = "alert('Se solicitó la corrección de errores(" + cant.ToString() + ") por E-mail!')";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "WARNING", alert, true);
            }
            catch (Exception ex)
            {
                string alert = "alert('Error al enviar el mail: " + ex.Message + "')";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "WARNING", alert, true);
            }
        }

        protected DataTable loadData(DataTable data)
        {
            using (var conn = new SqlConnection(connectionInfo))
            {
                string q = "SELECT * FROM VAL_DATA WHERE NomLote = '" + (string)Session["lote"] + "'";
                SqlCommand cmd = new SqlCommand(q, conn);
                conn.Open();
                data.Load(cmd.ExecuteReader());
                conn.Close();
                return data;
            }
        }

        protected DataTable loadWrong()
        {
            DataTable data = new DataTable();
            string q = "";
            using (var conn = new SqlConnection(connectionInfo))
            {
                try
                {
                    q = "SELECT * FROM VAL_DATA WHERE NomLote = '" + (string)Session["lote"] + "' and Estado <> 'PROCESADO' and EstadoFinal <> 'RECHAZADO'";
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
        protected void RemoveNullColumnFromDataTable(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][1] == DBNull.Value)
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        protected void BtnModif_Click(object sender, EventArgs e)
        {
            string query = "";
            DataTable data = new DataTable();

            Button btn = (Button)sender;
            switch (btn.CommandName)
            {
                case "modif":
                    query = string.Format(@"UPDATE a 
                                            SET a.Apellido = '{0}', a.Nombre = '{1}', a.TipoDNI = '{2}', a.DNI = '{3}', a.fechaNacimiento = '{4}',
                                            a.Provincia = '{5}', a.Localidad = '{6}', a.DireCalle = '{7}', a.DireNum = '{8}',
                                            a.DireDpto = '{9}', a.DirePiso = '{10}', a.cp = '{11}', a.Mail = '{12}', a.TarjetaTipo = '{13}',
                                            a.TarjetaNum = '{14}', a.TarjetaVence = '{15}', a.Modif = '*', a.UserModif = '{16}', a.FecModif = '{17}'
                                            FROM VAL_DATA as a
                                            WHERE a.id = {18}",
                                            ape.Value.ToString(), nom.Value.ToString(), tDni.Value.ToString(), dni.Value.ToString(), nac.Value.ToString().Replace(" 12:00:00 a. m.", ""),
                                            prov.Value.ToString(), loc.Value.ToString(), dir.Value.ToString(), num.Value.ToString(), dto.Value.ToString(),
                                            piso.Value.ToString(), cp.Value.ToString(), mail.Value.ToString(), tipoTc.Value.ToString(), nroTc.Value.ToString(),
                                            venTc.Value.ToString().Replace(" 12:00:00 a. m.", ""), (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), Hidden1.Value.ToString());
                    break;
                case "rech":
                    query = string.Format(@"UPDATE a SET a.EstadoFinal = 'RECHAZADO', a.Modif = '*', a.UserModif = '{0}', a.FecModif = '{1}' FROM VAL_DATA as a WHERE ID = {2}", (string)Session["user_logged"], DateTime.Now.ToString("dd/MM/yyyy"), Hidden1.Value.ToString());
                    break;
            }

            using (var conn = new SqlConnection(connectionInfo))
            {
                //string q1;
                //q1 = string.Format(@"SELECT count(*) FROM VAL_DATA WHERE NomLote = '{0}' AND Modif is NULL AND Estado <> 'PROCESADO' AND EstadoFinal is null", (string)Session["lote"]);
                try
                {
                    SqlCommand cmmd = new SqlCommand(query, conn);
                    //SqlCommand cmmd2 = new SqlCommand(q1, conn);
                    conn.Open();
                    cmmd.ExecuteNonQuery();
                    /*var left = cmmd2.ExecuteScalar();
                    string q = "";
                    if ((int)left != 0)
                    {
                        q = string.Format(@"UPDATE a SET a.A_CORREGIR = {0}
                                            FROM ALERTS as a
                                            WHERE LOTE = '{1}'", (int)left, (string)Session["lote"]);

                    }
                    else
                    {
                        q = string.Format(@"UPDATE a SET a.A_CORREGIR = 0, a.ESTADO = 'LISTO'
                                            FROM ALERTS as a
                                            WHERE LOTE = '{0}'", (string)Session["lote"]);
                    }
                    SqlCommand cmmd3 = new SqlCommand(q, conn);
                    cmmd3.ExecuteNonQuery();
                    conn.Close();*/
                    data = loadWrong();
                    refreshWrong(data);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('Cambios guardados con éxito!');", true);
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblNomLotVal.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
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

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                if (dt.Rows[j]["Estado"].ToString() != "PROCESADO")
                {
                    count++;
                    flag = true;
                    if (dt.Rows[j]["Modif"].ToString() == "*")
                        html.Append("<tr style=\"color:green;\" data-toggle=\"modal\" data-id=\"");
                    else
                        html.Append("<tr style=\"color:red;\" data-toggle=\"modal\" data-id=\"");
                    html.Append(count.ToString());
                    html.Append("\" data-target=\"#modalRow\">");
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

        protected void BtnLogOut_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Response.Redirect("default.aspx");
        }

        protected void BtnCargaEnriq_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                int cant = 0; int res = 0; int i = 0;
                foreach (HttpPostedFile uploadedFile in FileUpload1.PostedFiles)
                {
                    i++;
                    res = FileUpload1.PostedFiles.Count;
                    cant = res - i;
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

                    uploadedFile.SaveAs(FilePath);
                    updateSugar(FilePath, FileName, cant, res);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Por Favor Seleccione un Archivo');", true);
                return;
            }
        }

        protected void updateSugar(string FilePath, string FileName, int cant, int res)
        {
            DataTable dt = new DataTable();
            DataTable data = new DataTable();
            string[] cols = null;

            var lines = File.ReadAllLines(FilePath);

            if (lines.Count() > 0)
            {
                if (FileName.Contains("ABM"))
                    cols = lines[0].Split(new char[] { ';' });
                else
                    cols = lines[0].Split(new char[] { '\t' });

                foreach (var col in cols)
                    dt.Columns.Add(col);
            }

            string[] values;
            for (int i = 1; i < lines.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                if (FileName.Contains("ABM"))
                    values = lines[i].Split(new char[] { ';' });
                else
                    values = lines[i].Split(new char[] { '\t' });

                for (int j = 0; j < values.Count() && j < cols.Count(); j++)
                    dr[j] = values[j];

                dt.Rows.Add(dr);
            }

            string query = "";
            using (var conn = new SqlConnection(connectionInfo))
            {
                conn.Open();
                foreach (DataRow row in dt.Rows)
                {
                    if (FileName.Contains("ABM"))
                        query = string.Format(@"UPDATE a set a.Sugar = '{0}', a.SugarDesc = '{1}', a.SugarLot = '{2}', a.FecEnriq = '{3}'
                                            from VAL_DATA as a WHERE Propuesta = '{4}'",
                                               row["FOLLOW_STATUS"].ToString(), row["OBSERVACION"].ToString().Replace("'", "") + "{" + row["FOLLOW_DESC"].ToString().Replace("'", "") + "}", FileName, DateTime.Now.ToString("dd/MM/yyyy"), row["PROPUESTA"].ToString());
                    if (FileName.Contains("Emisiones"))
                        query = string.Format(@"UPDATE a set a.Sugar = '{0}', a.SugarDesc = '{1}', a.SugarLot = '{2}', a.FecEnriq = '{3}', a.SugarFec = '{4}'
                                            from VAL_DATA as a WHERE Propuesta = '{5}'",
                                               row["Estado"].ToString(), row["Motivo"].ToString().Replace("'", ""), FileName, DateTime.Now.ToString("dd/MM/yyyy"), row["Fecha_Emision"].ToString(), row["Solicitud"].ToString());
                    if (FileName.Contains("Anulaciones"))
                        query = string.Format(@"UPDATE a set a.Sugar = '{0}', a.SugarDesc = '{1}', a.SugarLot = '{2}', a.FecEnriq = '{3}', a.SugarFec = '{4}'
                                            from VAL_DATA as a WHERE Propuesta = '{5}'",
                                               row["Status"].ToString(), row["motivo"].ToString().Replace("'", ""), FileName, DateTime.Now.ToString("dd/MM/yyyy"), row["fecha_anulacion"].ToString(), row["sevicio"].ToString());
                    SqlCommand cmmd = new SqlCommand(query, conn);
                    cmmd.ExecuteNonQuery();
                }
                conn.Close();
                /*if (!FileName.Contains("entas"))
                {
                    query = string.Format(@"select distinct Sugar as Estado, count(Propuesta) as Cant
                                        from VAL_DATA as a
                                        where Sugar is not null and SugarLot = '{0}'
                                        group by Sugar", FileName);
                    conn.Open();
                    SqlCommand cmmd2 = new SqlCommand(query, conn);
                    data.Load(cmmd2.ExecuteReader());
                    conn.Close();
                }*/
            }

            /*StringBuilder html = new StringBuilder();

            html.Append("<thead>");
            html.Append("<tr>");
            for (int i = 0; i < data.Columns.Count; i++)
            {
                html.Append("<th>");
                html.Append(data.Columns[i].ColumnName.ToString());
                html.Append("</th>");
            }
            html.Append("</tr>");
            html.Append("</thead>");
            thead.Text = html.ToString();
            //tfootN.Text = html.ToString();

            html.Clear();
            html.Append("<tbody>");

            for (int j = 0; j < data.Rows.Count; j++)
            {
                html.Append("<tr>");
                for (int k = 0; k < data.Columns.Count; k++)
                {
                    html.Append("<td>");
                    html.Append(data.Rows[j][k].ToString());
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }
            html.Append("</tbody>");
            tbody.Text = html.ToString();
            html.Clear();*/
        }

    }
}