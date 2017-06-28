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
using ClosedXML.Excel;

namespace sales
{
    public partial class dashboard : System.Web.UI.Page
    {
        public string connectionInfo = WebConfigurationManager.AppSettings["connString"];
        public string user = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Count < 1)
                Response.Redirect("default.aspx");
            user = "https://avatars.discourse.org/v2/letter/" + ((string)Session["user_logged"])[0] + "/f6c823/45.png";
            if ((string)Session["user_profile"] != "ADMIN")
            {
                val.Visible = false;
                BtnOk.Visible = false;
                BtnTotal.Visible = false;
                BtnDel.Visible = false;
            }
            LblErro.Visible = false;
            fillLotes();
            checkMissing();
        }

        protected void checkMissing()
        {
            string q = "select count(*) as CANT from VAL_DATA where Sugar is null";
            string q1 = "select count(*) as CANT from VAL_DATA where Estado is null";
            string q2 = "select count(*) as CANT from VAL_DATA where Estado is not null and Sugar is not null";
            string q3 = "select count(*) as CANT from VAL_DATA";
            string q4 = "select distinct Estado, count(*) as CANT from VAL_DATA where Modif is null and Estado is not null group by Estado order by Estado";
            string q5 = "select count(*) as CANT from TEMP_SCOR as a inner join VAL_DATA as b on a.Propuesta = b.Propuesta where FOLLOW_STATUS is not null and Retorno_Scor is null";
            string q6 = "select distinct RESOLUCION, count(*) as CANT from TEMP_SCOR where ModifScor is null and FOLLOW_STATUS is not null and Retorno_Scor is null  and PROPUESTA in (select a.Propuesta from VAL_DATA as a) group by RESOLUCION";
            string q7 = string.Format(@"select UserModif, count(*) as CANT
                                            from TEMP_SCOR 
                                            where UserModif is not null and month(convert(datetime, FecModif, 103)) = MONTH(GETDATE())
                                            group by UserModif");
            string q8 = string.Format(@"select UserModif, count(*) as CANT
                                            from VAL_DATA
                                            where UserModif is not null and month(convert(datetime, FecModif, 103)) = MONTH(GETDATE())
                                            group by UserModif");
            string q9 = string.Format(@"select a.ID as ID_NEG, a.DNI, a.NomLote, a.RESOLUCION, a.UserModif, b.ID as ID_POS, b.DNI, b.NomLote, b.RESOLUCION, c.Sugar
                                            from TEMP_SCOR as a
                                            inner join TEMP_SCOR as b on a.Propuesta = b.Propuesta
                                            inner join VAL_DATA as c on b.Propuesta = c.Propuesta
                                            where a.ID <> b.ID and a.RESOLUCION <> 'Positiva' and b.RESOLUCION = 'Positiva'
                                            and month(convert(datetime,a.FecModif,103)) = month(getdate())
                                            order by b.ID asc");
            using (var conn = new SqlConnection(connectionInfo))
            {
                SqlCommand cmmd = new SqlCommand(q, conn);
                SqlCommand cmmd1 = new SqlCommand(q1, conn);
                SqlCommand cmmd2 = new SqlCommand(q2, conn);
                SqlCommand cmmd3 = new SqlCommand(q3, conn);
                SqlCommand cmmd4 = new SqlCommand(q4, conn);
                SqlCommand cmmd5 = new SqlCommand(q5, conn);
                SqlCommand cmmd6 = new SqlCommand(q6, conn);
                SqlCommand cmmd7 = new SqlCommand(q7, conn);
                SqlCommand cmmd8 = new SqlCommand(q8, conn);
                SqlCommand cmmd9 = new SqlCommand(q9, conn);
                DataTable dt = new DataTable();
                conn.Open();
                SqlDataReader reg = null;
                SqlDataReader reg1 = null;
                SqlDataReader reg2 = null;
                SqlDataReader reg3 = null;
                SqlDataReader reg4 = null;
                reg = cmmd.ExecuteReader();
                if (reg.Read())
                {
                    LtlExis.Text = "";
                    string cant = reg["CANT"].ToString();
                    LtlExis.Text = cant;
                }
                else
                {
                    LtlErro.Text = " 0 ";
                }
                reg1 = cmmd1.ExecuteReader();
                if (reg1.Read())
                {
                    LtlErro.Text = "";
                    string cant = reg1["CANT"].ToString();
                    LtlErro.Text = cant;
                }
                else
                {
                    LtlExis.Text = " 0 ";
                }
                reg2 = cmmd2.ExecuteReader();
                if (reg2.Read())
                {
                    LtlOk.Text = "";
                    string cant = reg2["CANT"].ToString();
                    LtlOk.Text = cant;
                }
                else
                {
                    LtlOk.Text = " 0 ";
                }
                reg3 = cmmd3.ExecuteReader();
                if (reg3.Read())
                {
                    LtlVta.Text = "";
                    string cant = reg3["CANT"].ToString();
                    LtlVta.Text = cant;
                }
                else
                {
                    LtlVta.Text = " 0 ";
                }
                dt.Load(cmmd4.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Estado"].ToString() == "Error")
                            LtlErron.Text = dt.Rows[i]["CANT"].ToString();
                        if (dt.Rows[i]["Estado"].ToString() == "Error interno")
                        {
                            if (int.Parse(LtlErron.Text) > 0)
                                LtlErron.Text = (int.Parse(LtlErron.Text) + int.Parse(dt.Rows[i]["CANT"].ToString())).ToString();
                        }
                        if (dt.Rows[i]["Estado"].ToString() == "Existente")
                            LtlExisten.Text = dt.Rows[i]["CANT"].ToString();
                        if (dt.Rows[i]["Estado"].ToString() == "Inconsistencia")
                            LtlIncon.Text = dt.Rows[i]["CANT"].ToString();
                        if (dt.Rows[i]["Estado"].ToString() == "PROCESADO")
                            LtlProc.Text = dt.Rows[i]["CANT"].ToString();
                    }
                }
                else
                {
                    LtlErron.Text = 0.ToString();
                    LtlExisten.Text = 0.ToString();
                    LtlIncon.Text = 0.ToString();
                    LtlProc.Text = 0.ToString();
                }
                dt.Clear();
                reg4 = cmmd5.ExecuteReader();
                if (reg4.Read())
                {
                    LtlTotScor.Text = "";
                    string cant = reg4["CANT"].ToString();
                    LtlTotScor.Text = cant;
                }
                else
                {
                    LtlTotScor.Text = 0.ToString();
                }
                dt.Load(cmmd6.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    LtlSocrNeg.Text = 0.ToString();
                    LtlScorErr.Text = 0.ToString();
                    LtlScorPos.Text = 0.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["RESOLUCION"].ToString() == "Negativa")
                            LtlScorNeg.Text = dt.Rows[i]["CANT"].ToString();
                        if (dt.Rows[i]["RESOLUCION"].ToString() == "ERROR")
                            LtlScorErr.Text = dt.Rows[i]["CANT"].ToString();
                        if (dt.Rows[i]["RESOLUCION"].ToString() == "Positiva")
                            LtlScorPos.Text = dt.Rows[i]["CANT"].ToString();
                    }
                }
                else
                {
                    LtlScorNeg.Text = 0.ToString();
                    LtlScorErr.Text = 0.ToString();
                    LtlScorPos.Text = 0.ToString();
                }
                dt.Clear();
                dt.Load(cmmd7.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    LtlJpScor.Text = 0.ToString();
                    LtlJmScor.Text = 0.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["UserModif"].ToString() == "jpietrantonio")
                            LtlJpScor.Text = dt.Rows[i]["CANT"].ToString();
                        if (dt.Rows[i]["UserModif"].ToString() == "jmandianes")
                            LtlJmScor.Text = dt.Rows[i]["CANT"].ToString();
                    }
                }
                else
                {
                    LtlJpScor.Text = 0.ToString();
                    LtlJmScor.Text = 0.ToString();
                }
                dt.Clear();
                dt.Load(cmmd8.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    LtlJpVal.Text = 0.ToString();
                    LtlJmVal.Text = 0.ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["UserModif"].ToString() == "jpietrantonio")
                            LtlJpVal.Text = dt.Rows[i]["CANT"].ToString();
                        if (dt.Rows[i]["UserModif"].ToString() == "jmandianes")
                            LtlJmVal.Text = dt.Rows[i]["CANT"].ToString();
                    }
                }
                else
                {
                    LtlJpVal.Text = 0.ToString();
                    LtlJmVal.Text = 0.ToString();
                }
                dt.Clear();
                dt.Load(cmmd9.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    int countJm = 0; int countJp = 0; bool fst = true;
                    int emiJm = 0; int emiJp = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (fst)
                        {
                            if (dt.Rows[i]["UserModif"].ToString() == "jmandianes")
                            {
                                countJm++;
                                if (dt.Rows[i]["Sugar"].ToString() == "Emitida")
                                    emiJm++;
                            }
                            if (dt.Rows[i]["UserModif"].ToString() == "jpietrantonio")
                            {
                                countJp++;
                                if (dt.Rows[i]["Sugar"].ToString() == "Emitida")
                                    emiJp++;
                            }
                            fst = false;
                        }
                        else
                        {
                            if (dt.Rows[i]["ID_POS"].ToString() != dt.Rows[i - 1]["ID_POS"].ToString())
                            {
                                if (dt.Rows[i]["UserModif"].ToString() == "jmandianes")
                                {
                                    countJm++;
                                    if (dt.Rows[i]["Sugar"].ToString() == "Emitida")
                                        emiJm++;
                                }
                                if (dt.Rows[i]["UserModif"].ToString() == "jpietrantonio")
                                {
                                    countJp++;
                                    if (dt.Rows[i]["Sugar"].ToString() == "Emitida")
                                        emiJp++;
                                }
                            }
                        }
                    }
                    LtlJmOk.Text = countJm.ToString();
                    LtlJpOk.Text = countJp.ToString();
                    LtlJmEmi.Text = emiJm.ToString();
                    LtlJpEmi.Text = emiJp.ToString();
                }
                else
                {
                    LtlJmOk.Text = 0.ToString();
                    LtlJpOk.Text = 0.ToString();
                    LtlJmEmi.Text = 0.ToString();
                    LtlJpEmi.Text = 0.ToString();
                }
                conn.Close();
            }
        }

        protected void fillLotes()
        {
            string query = ""; string query2 = ""; string query3 = "";
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            using (var conn = new SqlConnection(connectionInfo))
            {
                try
                {
                    query = "SELECT DISTINCT NomLote from VAL_DATA where NomLote is not null";
                    query2 = "SELECT DISTINCT NomLote from TEMP_SCOR where NomLote is not null";
                    query3 = "SELECT DISTINCT Vendedor from VAL_DATA where Vendedor is not null order by Vendedor";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlCommand cmd2 = new SqlCommand(query2, conn);
                    SqlCommand cmd3 = new SqlCommand(query3, conn);
                    conn.Open();
                    dt.Load(cmd.ExecuteReader());
                    dt.Load(cmd2.ExecuteReader());
                    dt2.Load(cmd3.ExecuteReader());
                    conn.Close();
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblErro.Visible = true;
                    LblErro.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                }
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ListItem lst = dropLotes.Items.FindByText(dt.Rows[i]["NomLote"].ToString());
                        if (lst == null) //si no existe en el drop lo agrego
                        {
                            ListItem l = new ListItem(dt.Rows[i]["NomLote"].ToString(), dt.Rows[i]["NomLote"].ToString(), true);
                            dropLotes.Items.Add(l);
                        }
                    }
                }
                if (dt2.Rows.Count > 0)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        ListItem lst2 = listVend.Items.FindByText(dt2.Rows[i]["Vendedor"].ToString());
                        if (lst2 == null) //si no existe en el drop lo agrego
                        {
                            ListItem l = new ListItem(dt2.Rows[i]["Vendedor"].ToString(), dt2.Rows[i]["Vendedor"].ToString(), true);
                            listVend.Items.Add(l);
                        }
                    }
                    /*for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        ListItem lst = dropLotes.Items.FindByText(dt2.Rows[i]["NomLote"].ToString());
                        if (lst == null) //si no existe en el drop lo agrego
                        {
                            ListItem l = new ListItem(dt2.Rows[i]["NomLote"].ToString(), dt2.Rows[i]["NomLote"].ToString(), true);
                            dropLotes.Items.Add(l);
                        }
                    }*/
                }
            }
        }

        protected void BtnLogOut_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Response.Redirect("default.aspx");
        }

        protected void BtnSql_Click(object sender, EventArgs e)
        {
            if (queryCust.Value.Length < 5)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[WARNING] Por Favor Escriba una query de SQL');", true);
                return;
            }
            else
            {
                string query = "";
                DataTable data = new DataTable();
                using (var conn = new SqlConnection(connectionInfo))
                {
                    try
                    {
                        query = "select* from VAL_DATA where DNI = '" + queryCust.Value.Replace(" ", "") + "' or DNI_old = '" + queryCust.Value.Replace(" ", "") + "'";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        conn.Open();
                        data.Load(cmd.ExecuteReader());
                        conn.Close();
                    }
                    catch (SqlException sqlEx)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                        LblErro.Visible = true;
                        LblErro.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                    }
                }
                //muestro resultados
                StringBuilder html = new StringBuilder();

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
                html.Clear();
            }
            Label2.Visible = false;
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            string query = "";
            string query2 = "";
            string nombre = "";
            string prefijo = "";
            string fecDe = "";
            string fecA = "";
            LinkButton btn = (LinkButton)sender;
            if (dropLotes.SelectedValue.ToString() == "Seleccionar..." && btn.CommandName != "dwn")
            {
                if (btn.CommandName == "erro" || btn.CommandName == "tot" || btn.CommandName == "ok")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Por favor seleccione un Lote');", true);
                    return;
                }
            }
            switch (btn.CommandName)
            {
                case "erro":
                    if (dropLotes.SelectedValue.ToString().IndexOf("Scoring") >= 0)
                    {
                        prefijo = "NEGATIVAS_";
                        nombre = prefijo + dropLotes.SelectedValue.ToString();
                        query = "SELECT * FROM TEMP_SCOR as a INNER JOIN VAL_DATA as b on a.Propuesta = b.PROPUESTA WHERE a.NomLote = '" + dropLotes.SelectedValue.ToString() + "' AND FOLLOW_STATUS = 'Negativa' and Retorno is null and ModifScor is null and EstadoFinal is null";
                    }
                    else
                    {
                        prefijo = "ERRONEOS_";
                        nombre = prefijo + dropLotes.SelectedValue.ToString();
                        query = "SELECT * FROM VAL_DATA WHERE NomLote = '" + dropLotes.SelectedValue.ToString() + "' AND Estado <> 'PROCESADO' and Retorno is null and Modif is null and EstadoFinal is null";
                    }
                    break;
                case "tot":
                    if (dropLotes.SelectedValue.ToString().IndexOf("Scoring") >= 0)
                    {
                        prefijo = "TOTAL_";
                        nombre = prefijo + dropLotes.SelectedValue.ToString();
                        query = "SELECT * FROM TEMP_SCOR as a INNER JOIN VAL_DATA as b on a.Propuesta = b.Propuesta WHERE a.NomLote = '" + dropLotes.SelectedValue.ToString() + "' and Retorno is null and ModifScor is null";
                    }
                    else
                    {
                        prefijo = "TOTAL_";
                        nombre = prefijo + dropLotes.SelectedValue.ToString();
                        query = "SELECT * FROM VAL_DATA WHERE NomLote = '" + dropLotes.SelectedValue.ToString() + "' and Retorno is null";
                    }
                    break;
                case "ok":
                    if (dropLotes.SelectedValue.ToString().IndexOf("Scoring") >= 0)
                    {
                        prefijo = "POSITIVAS_";
                        nombre = prefijo + dropLotes.SelectedValue.ToString();
                        query = "SELECT * FROM TEMP_SCOR as a INNER JOIN VAL_DATA as b on a.Propuesta = b.Propuesta WHERE a.NomLote = '" + dropLotes.SelectedValue.ToString() + "' AND FOLLOW_STATUS = 'Positiva' and Retorno is null";
                    }
                    else
                    {
                        prefijo = "PROCESADO_";
                        nombre = prefijo + dropLotes.SelectedValue.ToString();
                        query = "SELECT * FROM VAL_DATA WHERE NomLote = '" + dropLotes.SelectedValue.ToString() + "' AND Estado = 'PROCESADO' and Retorno is null";
                    }
                    break;
                case "totScor":
                    fecDe = fecFrom.Value.ToString();
                    fecA = fecTo.Value.ToString();
                    prefijo = "UNIVERSAL_NEG_";
                    nombre = prefijo + "SCORING";
                    query = "SELECT * FROM TEMP_SCOR as a INNER JOIN VAL_DATA as b on a.Propuesta = b.Propuesta WHERE a.FOLLOW_STATUS <> 'Positiva' and convert(datetime, b.FechaVenta, 103) >= convert(datetime, '" + fecDe + "', 103) and convert(datetime, b.FechaVenta, 103) <= convert(datetime, '" + fecA + "', 103) and Retorno_Scor is null";
                    break;
                case "totVal":
                    fecDe = fecFrom.Value.ToString();
                    fecA = fecTo.Value.ToString();
                    prefijo = "UNIVERSAL_ERRO_";
                    nombre = prefijo + "RETORNO";
                    query = "SELECT * FROM VAL_DATA WHERE Estado <> 'PROCESADO' and Modif is null and EstadoFinal is null and Retorno is null and convert(datetime, FechaVenta, 103) >= convert(datetime, '" + fecDe + "', 103) and convert(datetime, FechaVenta, 103) <= convert(datetime, '" + fecA + "', 103)";
                    break;
                case "corrScor":
                    fecDe = fecFrom.Value.ToString();
                    fecA = fecTo.Value.ToString();
                    prefijo = "UNIVERSAL_CORREGIDAS_";
                    nombre = prefijo + "SCORING";
                    query = "SELECT * FROM TEMP_SCOR as a INNER JOIN VAL_DATA as b on a.Propuesta = b.Propuesta WHERE a.ModifScor is not null and b.EstadoFinal is null and convert(datetime, a.FecModif, 103) >= convert(datetime, '" + fecDe + "', 103) and convert(datetime, a.FecModif, 103) <= convert(datetime, '" + fecA + "', 103)";
                    LblErro.Text = query;
                    break;
                case "corrVal":
                    fecDe = fecFrom.Value.ToString();
                    fecA = fecTo.Value.ToString();
                    prefijo = "UNIVERSAL_CORREGIDAS_";
                    nombre = prefijo + "RETORNO";
                    query = "SELECT * FROM VAL_DATA WHERE Modif is not null and EstadoFinal is null and convert(datetime, FecModif, 103) >= convert(datetime, '" + fecDe + "', 103) and convert(datetime, FecModif, 103) <= convert(datetime, '" + fecA + "', 103)";
                    break;
                case "dwn":
                    fecDe = fecVendDe.Value.ToString();
                    fecA = fecVendA.Value.ToString();
                    nombre = "DETALLE_" + listVend.SelectedValue.ToString();
                    query = String.Format(@"select *
                                            from VAL_DATA
                                            where Vendedor = '{0}' and Retorno is null and convert(datetime, FechaVenta, 103) >= convert(datetime, '{1}', 103) and convert(datetime, FechaVenta, 103) <= convert(datetime, '{2}', 103)",
                                            listVend.SelectedValue.ToString(), fecDe, fecA);
                    query2 = String.Format(@"select *
                                            from EXISTENTES as a
                                            inner join VAL_DATA as b on a.DNI = b.DNI
                                            where a.Vendedor = '{0}' and a.Vendedor <> b.Vendedor and convert(datetime, a.FechaVenta, 103) >= convert(datetime, '{1}', 103) and convert(datetime, a.FechaVenta, 103) <= convert(datetime, '{2}', 103)",
                                            listVend.SelectedValue.ToString(), fecDe, fecA);
                    break;
                case "prodCal":
                    fecDe = fecCalDe.Value.ToString();
                    fecA = fecCalA.Value.ToString();
                    nombre = "DETALLE_PROD_CAL_" + fecDe + "-" + fecA;
                    query = string.Format(@"select a.FecModif,  a.ID as ID_NEG, a.DNI, a.NomLote, a.RESOLUCION, a.UserModif, b.ID as ID_POS, b.DNI, b.NomLote, b.RESOLUCION
                                            from TEMP_SCOR as a
                                            inner join TEMP_SCOR as b on a.Propuesta = b.Propuesta
                                            where a.ID <> b.ID and a.RESOLUCION <> 'Positiva' and b.RESOLUCION = 'Positiva'
                                            and convert(datetime,a.FecModif,103) >= convert(datetime,'{0}',103) and convert(datetime,a.FecModif,103) <= convert(datetime,'{1}',103)
                                            order by b.ID asc", fecDe, fecA);
                    break;
            }
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            using (var conn = new SqlConnection(connectionInfo))
            {
                try
                {
                    SqlCommand cmd2 = null;
                    SqlDataAdapter sda2 = null;
                    if (query2.Length > 5)
                    {
                        cmd2 = new SqlCommand(query2, conn);
                        sda2 = new SqlDataAdapter();
                    }
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter sda = new SqlDataAdapter();
                    conn.Open();
                    sda.SelectCommand = cmd;
                    sda.Fill(dt);
                    if (query2.Length > 5)
                    {
                        sda2.SelectCommand = cmd2;
                        sda2.Fill(dt2);
                    }
                    if (dt.Rows.Count > 0)
                    {
                        dt.DefaultView.ToTable(true);
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            if (dropLotes.SelectedValue.ToString() != "Seleccionar..." && dropLotes.SelectedValue.ToString().Length < 30)
                                wb.Worksheets.Add(dt, dropLotes.SelectedValue.ToString());
                            else
                                wb.Worksheets.Add(dt, "Hoja1");
                            if (query2.Length > 5)
                            {
                                if (dt2.Rows.Count > 0)
                                    wb.Worksheets.Add(dt2, "EXISTENTES EN BASE");
                            }
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename=" + nombre + ".xlsx");
                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                Response.Flush();
                                Response.End();
                            }
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('No hay registros para las fechas seleccionadas!');", true);
                    }
                    conn.Close();
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblErro.Visible = true;
                    LblErro.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                }
            }
        }

        protected void BtnDel_Click(object sender, EventArgs e)
        {
            if (dropLotes.SelectedValue.ToString() == "Seleccionar...")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Por favor seleccione un Lote');", true);
                return;
            }
            string bd = "";
            if (dropLotes.SelectedValue.ToString().IndexOf("Scoring") >= 0)
            {
                bd = "TEMP_SCOR";
            }
            else
            {
                bd = "VAL_DATA";
            }
            string query = "delete from " + bd + " where NomLote = '" + dropLotes.SelectedValue.ToString() + "'";
            string query2 = "delete from alerts where LOTE = '" + dropLotes.SelectedValue.ToString() + "'";

            using (var conn = new SqlConnection(connectionInfo))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlCommand cmd2 = new SqlCommand(query2, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                    conn.Close();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('El lote se eliminó correctamente!');", true);
                    dropLotes.Items.Remove(dropLotes.Items.FindByText(dropLotes.SelectedValue.ToString()));
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblErro.Visible = true;
                    LblErro.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                }
            }
        }

        protected void BtnVend_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            DwnData.Visible = false;
            if (listVend.SelectedValue.ToString() == "Seleccionar...")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Por favor seleccione un Vendedor');", true);
                return;
            }
            string query = "";
            string queryExist = "";
            bool moreTone = false;
            switch (btn.CommandName)
            {
                case "vtas":
                    if (listVend.GetSelectedIndices().Length > 1)
                    {
                        moreTone = true;
                        query = "select Vendedor, count(distinct DNI) as CANT from VAL_DATA where ";
                        bool first = true;
                        foreach (ListItem it in listVend.Items)
                        {
                            if (it.Selected)
                            {
                                if (first)
                                {
                                    query += "(Vendedor = '" + it.Value + "' ";
                                    first = false;
                                }
                                else
                                {
                                    query += " or Vendedor = '" + it.Value + "'";
                                }
                            }
                        }
                        query += ") and Retorno is null and convert(datetime, FechaVenta, 103) >= convert(datetime, '" + fecVendDe.Value.ToString() + "', 103) and convert(datetime, FechaVenta, 103) <= convert(datetime, '" + fecVendA.Value.ToString() + "', 103) group by Vendedor";
                    }
                    else
                    {
                        query = String.Format(@"select Vendedor, count(distinct DNI) as CANT
                                            from VAL_DATA
                                            where Vendedor = '{0}' and Retorno is null and convert(datetime, FechaVenta, 103) >= convert(datetime, '{1}', 103) and convert(datetime, FechaVenta, 103) <= convert(datetime, '{2}', 103)
                                            group by Vendedor", listVend.SelectedValue.ToString(), fecVendDe.Value.ToString(), fecVendA.Value.ToString());
                        queryExist = String.Format(@"select a.Vendedor, count(distinct a.DNI) as CANT
                                                    from EXISTENTES as a
                                                    inner join VAL_DATA as b on a.DNI = b.DNI
                                                    where a.Vendedor = '{0}' and a.Vendedor <> b.Vendedor and convert(datetime, a.FechaVenta, 103) >= convert(datetime, '{1}', 103) and convert(datetime, a.FechaVenta, 103) <= convert(datetime, '{2}', 103)
                                                    group by a.Vendedor", listVend.SelectedValue.ToString(), fecVendDe.Value.ToString(), fecVendA.Value.ToString());
                        DwnData.Visible = true;
                    }
                    break;
                case "sug":
                    if (listVend.GetSelectedIndices().Length > 1)
                    {
                        moreTone = true;
                        query = "select Vendedor, Sugar, Estado, count(distinct DNI) as CANT from val_data where ";
                        bool first = true;
                        foreach (ListItem it in listVend.Items)
                        {
                            if (it.Selected)
                            {
                                if (first)
                                {
                                    query += "(Vendedor = '" + it.Value + "' ";
                                    first = false;
                                }
                                else
                                {
                                    query += " or Vendedor = '" + it.Value + "'";
                                }
                            }
                        }
                        query += ") and convert(datetime, FechaVenta, 103) >= convert(datetime,'" + fecVendDe.Value.ToString() + "', 103) and convert(datetime, FechaVenta, 103) <= convert(datetime, '" + fecVendA.Value.ToString() + "', 103) and Estado is not null group by Vendedor, Sugar, Estado";
                    }
                    else
                    {
                        query = String.Format(@"select Vendedor, sugar, Estado, count(distinct DNI) as CANT
                                            from val_data
                                            where vendedor = '{0}' and Retorno is null
                                            and convert(datetime, FechaVenta, 103) >= convert(datetime, '{1}', 103) and convert(datetime, FechaVenta, 103) <= convert(datetime, '{2}', 103)
                                            group by Vendedor, sugar, Estado", listVend.SelectedValue.ToString(), fecVendDe.Value.ToString(), fecVendA.Value.ToString());
                        queryExist = String.Format(@"select a.Vendedor, count(distinct a.DNI) as CANT
                                                    from EXISTENTES as a
                                                    inner join VAL_DATA as b on a.DNI = b.DNI
                                                    where a.Vendedor = '{0}' and a.Vendedor <> b.Vendedor and convert(datetime, a.FechaVenta, 103) >= convert(datetime, '{1}', 103) and convert(datetime, a.FechaVenta, 103) <= convert(datetime, '{2}', 103)
                                                    group by a.Vendedor", listVend.SelectedValue.ToString(), fecVendDe.Value.ToString(), fecVendA.Value.ToString());
                        DwnData.Visible = true;
                    }
                    break;
                default:
                    break;
            }
            using (var conn = new SqlConnection(connectionInfo))
            {
                try
                {
                    SqlCommand cmmd = new SqlCommand(query, conn);
                    SqlCommand cmmd2 = null;
                    if (!moreTone)
                        cmmd2 = new SqlCommand(queryExist, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmmd);
                    DataTable dt = new DataTable();
                    DataTable dt2 = null;
                    if (!moreTone)
                        dt2 = new DataTable();
                    conn.Open();
                    da.Fill(dt);
                    GridViewVend.DataSource = dt;
                    GridViewVend.DataBind();
                    if (!moreTone)
                        dt2.Load(cmmd2.ExecuteReader());
                    int count = 0;
                    int contExis = 0;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        count = count + (int)dt.Rows[i]["CANT"];
                        if (dt.Rows[i]["Estado"].ToString() == "Existente")
                            contExis = contExis + (int)dt.Rows[i]["CANT"];
                    }
                    conn.Close();
                    if (contExis > 0)
                        LblTotExist.Text = "Existentes en Base: " + contExis.ToString();
                    LblTotVend.Text = "Total: " + count.ToString();
                    if (!moreTone)
                    {
                        if (dt2.Rows.Count > 0)
                            LblTotRepExist.Text = "Existentes Repetidos: " + dt2.Rows[0]["CANT"].ToString();
                    }
                    if (!moreTone)
                    {
                        LblTotExist.Visible = true;
                        LblTotRepExist.Visible = true;
                    }
                    LblTotVend.Visible = true;
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblErro.Visible = true;
                    LblErro.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                }
            }
            //muestro resultados
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
            html.Clear();
            Label2.Visible = false;*/
        }

        protected void BtnGenRep_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string query = ""; string query2 = "";
            string fecDe = fecGenDe.Value.ToString();
            string fecA = fecGenA.Value.ToString();
            using (var conn = new SqlConnection(connectionInfo))
            {
                switch (btn.CommandName)
                {
                    case "vtasTotEst":
                        query = string.Format(@"SELECT Estado, month(convert(datetime,fechaVenta,103)) as MES, count(distinct DNI) as CANT
                                            FROM VAL_DATA as a
                                            where Estado is not null and convert(datetime, fechaVenta, 103) >= convert(datetime, '{0}', 103) and convert(datetime, fechaVenta, 103) <= convert(datetime, '{1}', 103)
                                            group by Estado, month(convert(datetime, fechaVenta, 103))", fecDe, fecA);
                        if (chkGenRep.Checked == true)
                        {
                            query2 = string.Format(@"SELECT *
                                            FROM VAL_DATA as a
                                            where Estado is not null and convert(datetime, fechaVenta, 103) >= convert(datetime, '{0}', 103) and convert(datetime, fechaVenta, 103) <= convert(datetime, '{1}', 103)",
                                            fecDe, fecA);
                        }
                        break;
                    case "vtasTotSug":
                        query = string.Format(@"SELECT Sugar, month(convert(datetime,fechaVenta, 103)) as MES, count(distinct DNI) as CANT
                                            FROM VAL_DATA as a
                                            where Sugar is not null and convert(datetime, fechaVenta, 103) >= convert(datetime, '{0}', 103) and convert(datetime, fechaVenta, 103) <= convert(datetime, '{1}', 103)
                                            group by Sugar, month(convert(datetime, fechaVenta, 103))", fecDe, fecA);
                        if (chkGenRep.Checked == true)
                        {
                            query2 = string.Format(@"SELECT *
                                            FROM VAL_DATA as a
                                            where Sugar is not null and convert(datetime, fechaVenta, 103) >=convert(datetime, '{0}', 103) and convert(datetime, fechaVenta, 103) <= convert(datetime, '{1}', 103)",
                                            fecDe, fecA);
                        }
                        break;
                }
                try
                {
                    SqlCommand cmmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmmd);
                    SqlCommand cmmd2 = null;
                    SqlDataAdapter da2 = null;
                    if (chkGenRep.Checked == true)
                    {
                        cmmd2 = new SqlCommand(query2, conn);
                        da2 = new SqlDataAdapter();
                    }
                    DataTable dt = new DataTable();
                    DataTable dt2 = new DataTable();
                    conn.Open();
                    if (chkGenRep.Checked == true)
                    {
                        da2.SelectCommand = cmmd2;
                        da2.Fill(dt2);
                    }
                    da.Fill(dt);
                    GridViewRep.DataSource = dt;
                    GridViewRep.DataBind();
                    if (dt2.Rows.Count > 0)
                    {
                        dt2.DefaultView.ToTable(true);
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(dt2, "Hoja1");
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename=DETALLE_GRAL_" + fecDe + "_A_" + fecA + ".xlsx");
                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                Response.Flush();
                                Response.End();
                            }
                        }
                    }
                    int count = 0;
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        count = count + (int)dt.Rows[i]["CANT"];
                    }
                    conn.Close();
                    LblTotGenRep.Text = "Total: " + count.ToString();
                    LblTotGenRep.Visible = true;
                }
                catch (SqlException sqlEx)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "myalert", "alert('[ERROR] Ha ocurrido un error, Contactar a Soporte');", true);
                    LblErro.Visible = true;
                    if (chkGenRep.Checked == true)
                        LblErro.Text = "query2: " + query2 + ". ERROR: " + sqlEx.Message;
                    else
                        LblErro.Text = "query: " + query + ". ERROR: " + sqlEx.Message;
                }
            }
        }
    }
}