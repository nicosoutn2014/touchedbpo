<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="sales.dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta charset="utf-8" />
  <meta http-equiv="X-UA-Compatible" content="IE=edge" />
  <title>Validacion | sales.io</title>
  <!-- Tell the browser to be responsive to screen width -->
  <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport" />
  <!-- Bootstrap 3.3.6 -->
  <link rel="stylesheet" href="Content/bootstrap.min.css" />
  <!-- Font Awesome -->
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.5.0/css/font-awesome.min.css" />
  <!-- Ionicons -->
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ionicons/2.0.1/css/ionicons.min.css" />
  <!-- Data Tables -->
  <link rel="stylesheet" href="plugins/datatables/dataTables.bootstrap.css" />
  <!-- Theme style -->
  <link rel="stylesheet" href="Content/AdminLTE.min.css" />
  <!-- AdminLTE Skins. Choose a skin from the css/skins
       folder instead of downloading all of them to reduce the load. -->
  <link rel="stylesheet" href="Content/skins/_all-skins.min.css" />
  <!-- iCheck -->
  <link rel="stylesheet" href="plugins/iCheck/flat/blue.css" />
  <!-- Morris chart -->
  <link rel="stylesheet" href="plugins/morris/morris.css" />
  <!-- jvectormap -->
  <link rel="stylesheet" href="plugins/jvectormap/jquery-jvectormap-1.2.2.css" />
  <!-- Date Picker -->
  <link rel="stylesheet" href="plugins/datepicker/datepicker3.css" />
  
  <link rel="stylesheet" type="text/css" href="Content/bootstrap-select.css" />

  <!-- Daterange picker -->
  <link rel="stylesheet" href="plugins/daterangepicker/daterangepicker.css" />
  <!-- bootstrap wysihtml5 - text editor -->
  <link rel="stylesheet" href="plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css" />

  <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
  <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
  <!--[if lt IE 9]>
  <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
  <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
  <![endif]-->
</head>
<body class="hold-transition skin-blue sidebar-mini">
<form runat="server" id="form1">
<asp:ScriptManager runat="server" ID="mainScript"></asp:ScriptManager>
<div class="wrapper">

  <header class="main-header">
    <!-- Logo -->
    <a href="#" class="logo">
      <!-- mini logo for sidebar mini 50x50 pixels -->
      <span class="logo-mini">T</span>
      <!-- logo for regular state and mobile devices -->
      <span class="logo-lg"><b>TOUCHED</b>&nbsp;BPO</span>
    </a>
    <!-- Header Navbar: style can be found in header.less -->
    <nav class="navbar navbar-static-top">
      <!-- Sidebar toggle button-->
      <a href="#" class="sidebar-toggle" data-toggle="offcanvas" role="button">
        <span class="sr-only">Toggle navigation</span>
      </a>

      <div class="navbar-custom-menu">
        <ul class="nav navbar-nav">
          <!-- Notifications: style can be found in dropdown.less -->
          <li class="dropdown notifications-menu">
            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
              <i class="fa fa-bell-o"></i>
              <span class="label label-warning">1</span>
            </a>
            <ul class="dropdown-menu">
              <li class="header">Tienes 1 notificacion</li>
              <li>
                <!-- inner menu: contains the actual data -->
                <ul class="menu">
                  <li>
                    <a href="#">
                      <i class="fa fa-warning text-yellow"></i> Se corrigieron los Negativos de Scoring
                    </a>
                  </li>
                </ul>
              </li>
              <li class="footer"><a href="#">Ver Todas</a></li>
            </ul>
          </li>
          <li class="dropdown user user-menu">
            <a href="#" class="dropdown-toggle" data-toggle="dropdown">
              <img src="<%=user %>" class="user-image" />
              <span class="hidden-xs"><%=(string)Session["user_logged"] %></span>
            </a>
            <ul class="dropdown-menu">
              <!-- User image -->
              <li class="user-header">
                <img src="<%=user %>" class="img-circle" />
                <p>
                  <span><%=(string)Session["name"] %></span> - <span><%=(string)Session["cargo"] %></span>
                  <!--<small>Member since Nov. 2012</small> -->
                </p>
              </li>
              <!-- Menu Body -->
              <!-- Menu Footer-->
              <li class="user-footer">
                <div class="pull-right">
                  <asp:LinkButton ID="BtnLogOut" runat="server" CssClass="btn btn-default btn-flat" OnClick="BtnLogOut_Click"><span style="font-family:Effra; color:black;">Salir</span>&nbsp;&nbsp;<span class="fa fa-power-off" style="color:indianred"></span></asp:LinkButton>
                </div>
              </li>
            </ul>
          </li>
        </ul>
      </div>
    </nav>
  </header>
  <!-- Left side column. contains the logo and sidebar -->
  <aside class="main-sidebar">
    <!-- sidebar: style can be found in sidebar.less -->
    <section class="sidebar">
      <!-- Sidebar user panel -->
      <!-- sidebar menu: : style can be found in sidebar.less -->
      <ul class="sidebar-menu">
        <li class="header">MENU PRINCIPAL</li>
        <li class="treeview active" runat="server">
          <a href="#">
            <i class="fa fa-search"></i> <span>Consultas</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
        </li>
        <li id="val" class="active treeview" runat="server">
          <a href="#">
            <i class="fa fa-check"></i> <span>Validacion</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
          <ul class="treeview-menu">
            <li><a href="main.aspx"><i class="fa fa-circle-o"></i>Ir a Carga de Datos</a></li>
          </ul>
        </li>
        <li class="active treeview" runat="server">
          <a href="#">
            <i class="fa fa-pencil"></i> <span>Correcciones y Scoring</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
          <ul class="treeview-menu">
            <li><a href="main2.aspx"><i class="fa fa-circle-o"></i>Ir a Cargar o Corregir</a></li>
          </ul>
        </li>
        <!-- <li class="treeview active" runat="server">
          <a href="#">
            <i class="fa fa-random"></i> <span>Estadísticas</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
          <ul class="treeview-menu">
            <li><a href="#"><i class="fa fa-circle-o"></i>Ver Gráficos</a></li>
          </ul>
        </li> --> 
                 <!--<li>
          <a href="calendar.aspx">
            <i class="fa fa-calendar"></i> <span>Calendario</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
        </li> -->
        <!-- <li class="header">LABELS</li>
        <li><a href="#"><i class="fa fa-circle-o text-red"></i> <span>Important</span></a></li>
        <li><a href="#"><i class="fa fa-circle-o text-yellow"></i> <span>Warning</span></a></li>
        <li><a href="#"><i class="fa fa-circle-o text-aqua"></i> <span>Information</span></a></li> -->
      </ul>
    </section>
    <!-- /.sidebar -->
  </aside>

  <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
    <!-- Content Header (Page header) -->
    <section class="content-header" style="border-bottom: 1px gray solid; padding: 15px 15px 15px 15px;">
      <h1>
        sales<span style="color:darkgray;">.io</span>
        <small>Panel de Control</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="#"><i class="fa fa-dashboard"></i> Home</a></li>
        <li class="active">Tablero</li>
      </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <!-- Small boxes (Stat box) -->
      <div class="row">
          <h4>&nbsp;&nbsp;&nbsp;<strong>Estado General y Retorno</strong></h4><br />
        <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-aqua">
            <div class="inner">
              <h3><asp:Literal ID="LtlVta" runat="server"></asp:Literal></h3>
              <p>Ventas Cargadas</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green">
            <div class="inner">
              <h3><asp:Literal ID="LtlOk" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Ventas Enriq OK</p>
            </div>
            <div class="icon">
              <i class="ion ion-stats-bars"></i>
            </div>
            <a href="#" class="small-box-footer">Más info <i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-red">
            <div class="inner">
              <h3><asp:Literal ID="LtlErro" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Ventas Sin Estado</p>
            </div>
            <div class="icon">
              <i class="ion ion-pie-graph"></i>
            </div>
            <a href="#" class="small-box-footer">Más info <i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-yellow">
            <div class="inner">
              <h3><asp:Literal ID="LtlExis" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Ventas Sin Sugar</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green-gradient">
            <div class="inner">
              <h3><asp:Literal ID="LtlProc" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Procesadas</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-purple">
            <div class="inner">
              <h3><asp:Literal ID="LtlExisten" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Existentes</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-red-active">
            <div class="inner">
              <h3><asp:Literal ID="LtlErron" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Errores</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-maroon">
            <div class="inner">
              <h3><asp:Literal ID="LtlIncon" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Inconsistencias</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
          <br />
          <br />
          <h4>&nbsp;&nbsp;&nbsp;<strong>Estado Scoring</strong></h4><br />
          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-blue">
            <div class="inner">
              <h3><asp:Literal ID="LtlTotScor" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Scoring Cargado</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green">
            <div class="inner">
              <h3><asp:Literal ID="LtlScorPos" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Scoring Positivas</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-red-active">
            <div class="inner">
              <h3><asp:Literal ID="LtlScorNeg" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Scoring Negativas</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-3 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-orange">
            <div class="inner">
              <h3><asp:Literal ID="LtlScorErr" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Scoring Erroneo</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-stalker"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
          <br />
          <br />
          <h4>&nbsp;&nbsp;&nbsp;<strong>Estado Calidad</strong></h4><br />
          <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-blue">
            <div class="inner">
              <h3><asp:Literal ID="LtlJpVal" runat="server"></asp:Literal></h3>

              <p>Juan Pablo Retorno</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-orange">
            <div class="inner">
              <h3><asp:Literal ID="LtlJmVal" runat="server"></asp:Literal></h3>

              <p>Juan Manuel Retorno</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-blue">
            <div class="inner">
              <h3><asp:Literal ID="LtlJpScor" runat="server"></asp:Literal></h3>

              <p>Juan Pablo Scoring</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-orange">
            <div class="inner">
              <h3><asp:Literal ID="LtlJmScor" runat="server"></asp:Literal></h3>

              <p>Juan Manuel Scoring</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col -->
          <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green">
            <div class="inner">
              <h3><asp:Literal ID="LtlJpOk" runat="server"></asp:Literal></h3>

              <p>Juan Pablo OK</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

          <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green">
            <div class="inner">
              <h3><asp:Literal ID="LtlJmOk" runat="server"></asp:Literal></h3>

              <p>Juan Manuel OK</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
      </div>

        <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green-gradient">
            <div class="inner">
              <h3><asp:Literal ID="LtlJpEmi" runat="server"></asp:Literal></h3>

              <p>Juan Pablo Emitidas</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>

        <div class="col-lg-6 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green-gradient">
            <div class="inner">
              <h3><asp:Literal ID="LtlJmEmi" runat="server"></asp:Literal></h3>

              <p>Juan Manuel Emitidas</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
      <!-- /.row -->
    </section>

      <section class="content">
      <!-- Small boxes (Stat box) 
      <div class="row">
        <div class="col-lg-3 col-xs-6">
          <!-- small box 
          <div class="small-box bg-aqua">
            <div class="inner">
              <h3>32</h3>

              <p>Ventas</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col 
        <div class="col-lg-3 col-xs-6">
          <!-- small box 
          <div class="small-box bg-green">
            <div class="inner">
              <h3>83<sup style="font-size: 20px">%</sup></h3>

              <p>Correctas</p>
            </div>
            <div class="icon">
              <i class="ion ion-stats-bars"></i>
            </div>
            <a href="#" class="small-box-footer">Más info <i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col 
        <div class="col-lg-3 col-xs-6">
          <!-- small box 
          <div class="small-box bg-red">
            <div class="inner">
              <h3>17<sup style="font-size: 20px">%</sup></h3>

              <p>Erróneas</p>
            </div>
            <div class="icon">
              <i class="ion ion-pie-graph"></i>
            </div>
            <a href="#" class="small-box-footer">Más info <i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col 
        <div class="col-lg-3 col-xs-6">
          <!-- small box 
          <div class="small-box bg-yellow">
            <div class="inner">
              <h3>15</h3>

              <p>Operadores</p>
            </div>
            <div class="icon">
              <i class="ion ion-person-add"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
      </div>
      <!-- /.row -->
      
      <div class="row">
        <div class="col-xs-12">
          <div class="box">
            <div class="box-header">
              <h3 class="box-title">Consultar Bases:</h3>
                <asp:Label ID="LblErro" runat="server" Text=""></asp:Label>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
              <div id="sql" runat="server" class="col-md-12" style="border-bottom: 1px solid #2f2d2d;">
                  <div class="col-md-6">
                      <asp:Label ID="Label4" runat="server" Text="Reportes Generales:" Font-Bold="true"></asp:Label><br />
                      <p>Fecha Desde:</p>
                      <input runat="server" id="fecGenDe" class="datepicker" data-date-format="dd/mm/yyyy"/>
                      <p>Fecha Hasta:</p>
                      <input runat="server" id="fecGenA" class="datepicker" data-date-format="dd/mm/yyyy"/>
                      <br /><br />
                      <asp:CheckBox ID="chkGenRep" runat="server" Checked="false" Text="Descargar Reporte" /><br />
                      <asp:LinkButton ID="BtnGenRep" runat="server" CssClass="btn btn-default" CommandArgument="vtasTotEst" CommandName="vtasTotEst" OnClick="BtnGenRep_Click"><i class="fa fa-line-chart"></i>&nbsp;&nbsp;Ventas Totales x Estado</asp:LinkButton>&nbsp;
                      <asp:LinkButton ID="BtnGenRep2" runat="server" CssClass="btn btn-default" CommandArgument="vtasTotSug" CommandName="vtasTotSug" OnClick="BtnGenRep_Click"><i class="fa fa-line-chart"></i>&nbsp;&nbsp;Ventas Totales x Sugar</asp:LinkButton>&nbsp;
                      <br /><br />
                  </div>
                  <div class="col-md-6">
                      <asp:GridView ID="GridViewRep" runat="server" CssClass="table table-striped" CellPadding="4" ForeColor="#333333" GridLines="None">
                          <AlternatingRowStyle BackColor="White" />
                          <EditRowStyle BackColor="#2461BF" />
                          <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                          <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                          <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                          <RowStyle BackColor="#EFF3FB" />
                          <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                          <SortedAscendingCellStyle BackColor="#F5F7FB" />
                          <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                          <SortedDescendingCellStyle BackColor="#E9EBEF" />
                          <SortedDescendingHeaderStyle BackColor="#4870BE" />
                      </asp:GridView>
                      <br /><br />
                      <asp:Label ID="LblTotGenRep" runat="server" Font-Bold="true" Visible="false" Text="Total: 0"></asp:Label>
                      <br /><br />
                  </div>
                  <br /><br />
                </div>
                  <br /><br />
                  <div class="col-md-12" style="border-bottom: 1px solid #2f2d2d;">
                      <div class="col-md-6">
                          <asp:Label ID="Label3" runat="server" Text="Reportes de Vendedores:" Font-Bold="true"></asp:Label>
                          <br /><br />
                          <asp:ListBox ID="listVend" runat="server" CssClass="form-control" SelectionMode="Multiple" Width="300" Height="200" style="display:inline;">
                          </asp:ListBox>
                          <br />
                          <br />
                          <p>Fecha Desde:</p>
                          <input runat="server" id="fecVendDe" class="datepicker" data-date-format="dd/mm/yyyy"/>
                          <p>Fecha Hasta:</p>
                          <input runat="server" id="fecVendA" class="datepicker" data-date-format="dd/mm/yyyy"/>
                          <br />
                          <br />
                          <asp:LinkButton ID="BtnVend" runat="server" CssClass="btn btn-default" CommandArgument="vtas" CommandName="vtas" OnClick="BtnVend_Click"><i class="fa fa-line-chart"></i>&nbsp;&nbsp;Ventas x Vendedor</asp:LinkButton>&nbsp;
                          <asp:LinkButton ID="BtnSug" runat="server" CssClass="btn btn-default" CommandArgument="sug" CommandName="sug" OnClick="BtnVend_Click"><i class="fa fa-line-chart"></i>&nbsp;&nbsp;Sugar x Vendedor</asp:LinkButton>&nbsp;
                          <asp:LinkButton ID="DwnData" Visible="false" runat="server" CssClass="btn btn-primary" CommandArgument="dwn" CommandName="dwn" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Descargar Detalle</asp:LinkButton>&nbsp;
                          <br />
                          <br />
                      </div>
                      <div class="col-md-6">
                          <asp:GridView ID="GridViewVend" runat="server" CssClass="table table-striped" CellPadding="4" ForeColor="#333333" GridLines="None">
                              <AlternatingRowStyle BackColor="White" />
                              <EditRowStyle BackColor="#2461BF" />
                              <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                              <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                              <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                              <RowStyle BackColor="#EFF3FB" />
                              <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                              <SortedAscendingCellStyle BackColor="#F5F7FB" />
                              <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                              <SortedDescendingCellStyle BackColor="#E9EBEF" />
                              <SortedDescendingHeaderStyle BackColor="#4870BE" />
                        </asp:GridView>
                        <br /><br />
                        <asp:Label ID="LblTotVend" runat="server" Font-Bold="true" Visible="false" Text="Total: 0"></asp:Label>&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="LblTotExist" runat="server" Font-Bold="true" Visible="false" Text="Existentes en Base: 0" ForeColor="IndianRed"></asp:Label>&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="LblTotRepExist" runat="server" Font-Bold="true" Visible="false" Text="Existentes Repetidos: 0" ForeColor="IndianRed"></asp:Label>&nbsp;&nbsp;&nbsp;
                        <br /><br />
                      </div>
                  </div>
                  <br />
                  <br />
                  <div class="col-md-12" style="border-bottom: 1px solid #2f2d2d;">
                      <asp:Label ID="LblExport" runat="server" Text="Exportación Rápida por Lotes:" Font-Bold="true"></asp:Label>
                      <br /><br />
                      <asp:DropDownList ID="dropLotes" runat="server" CssClass="form-control" Width="300" style="display:inline;">
                      <asp:ListItem>Seleccionar...</asp:ListItem>
                      </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                      <asp:LinkButton ID="BtnDel" runat="server" CssClass="btn btn-default" CommandArgument="del" CommandName="del" OnClick="BtnDel_Click"><i class="fa fa-trash"></i>&nbsp;&nbsp;Borrar Lote</asp:LinkButton>&nbsp;
                      <br /><br />
                      <asp:LinkButton ID="BtnErron" runat="server" CssClass="btn btn-danger" CommandArgument="erro" CommandName="erro" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Erroneos</asp:LinkButton>&nbsp;
                      <asp:LinkButton ID="BtnOk" runat="server" CssClass="btn btn-success" CommandArgument="ok" CommandName="ok" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Procesadas</asp:LinkButton>&nbsp;
                      <asp:LinkButton ID="BtnTotal" runat="server" CssClass="btn btn-primary" CommandArgument="tot" CommandName="tot" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Todo</asp:LinkButton>&nbsp;
                      <br />
                  <br />
                  <asp:Label ID="Label1" runat="server" Text="Exportación Masiva:" Font-Bold="true"></asp:Label>
                  <br /> 
                  <div class="col-md-6" style="width:100%; padding: 0; padding-top: 10px; padding-bottom: 10px;">
                      <div class="col-md-3" style="padding:0; width:20%;">
                          <p>Fecha Desde:</p>
                          <input runat="server" id="fecFrom" class="datepicker" data-date-format="dd/mm/yyyy"/>
                      </div>
                      <div class="col-md-3">
                          <p>Fecha Hasta:</p>
                          <input runat="server" id="fecTo" class="datepicker" data-date-format="dd/mm/yyyy"/>
                      </div>
                      <br />
                  </div>
                  <br />
                  <br />
                  <asp:LinkButton ID="BtnCorrVal" runat="server" CssClass="btn btn-success" CommandArgument="corrVal" CommandName="corrVal" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Retorno Corregidas</asp:LinkButton>
                  <asp:LinkButton ID="BtnCorrScor" runat="server" CssClass="btn btn-success" CommandArgument="corrScor" CommandName="corrScor" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Scoring Corregidas</asp:LinkButton><br />
                  <asp:LinkButton ID="BtnValNeg" runat="server" CssClass="btn btn-default" CommandArgument="totVal" CommandName="totVal" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Todo Retorno Erroneo</asp:LinkButton>&nbsp;
                  <asp:LinkButton ID="BtnScorNeg" runat="server" CssClass="btn btn-default" CommandArgument="totScor" CommandName="totScor" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Todo Scoring Negativo</asp:LinkButton>&nbsp;
                      <br /><br />
                  </div>
                <div class="col-md-12" style="border-bottom: 1px solid #2f2d2d;">
                    <asp:Label ID="Label5" runat="server" Text="Reportes Calidad:" Font-Bold="true"></asp:Label>
                    <br />
                      <div class="col-md-6" style="width:100%; padding: 0; padding-top: 10px; padding-bottom: 10px;">
                          <div class="col-md-3" style="padding:0; width:20%;">
                              <p>Fecha Desde:</p>
                              <input runat="server" id="fecCalDe" class="datepicker" data-date-format="dd/mm/yyyy"/>
                          </div>
                          <div class="col-md-3">
                              <p>Fecha Hasta:</p>
                              <input runat="server" id="fecCalA" class="datepicker" data-date-format="dd/mm/yyyy"/>
                          </div>
                          <br />
                      </div>
                    <br /><br />
                    <asp:LinkButton ID="btnProdCal" runat="server" CssClass="btn btn-primary" CommandArgument="prodCal" CommandName="prodCal" OnClick="BtnExport_Click"><i class="fa fa-download"></i>&nbsp;&nbsp;Exportar Productividad</asp:LinkButton>&nbsp;
                    <br /><br />
                  </div>
                <br />
                <br />
                <div class="col-md-12" style="border-bottom: 1px solid #2f2d2d;">
                      <input id="queryCust" runat="server" placeholder="Ingrese DNI..." style="width:20%; margin-top:20px;"/>
                      <br />
                      <br />
                      <asp:LinkButton ID="BtnSql" runat="server" CssClass="btn btn-primary" OnClick="BtnSql_Click"><i class="fa fa-search"></i>&nbsp;Consultar</asp:LinkButton>&nbsp;
                      <br /><br />
                  </div>
                <br />
              <h3 class="box-title" style="font-size:18px;">Resultados:</h3>
                <br />
                <asp:Label ID="Label2" runat="server" Text="No se cargaron datos" Font-Italic="true"></asp:Label>
              <table id="mainTable" class="table table-bordered table-striped">
                    <asp:Literal ID="thead" runat="server"></asp:Literal>
                    <asp:Literal ID="tbody" runat="server"></asp:Literal>
                    <!-- <asp:Literal ID="tfootN" runat="server"></asp:Literal> -->
              </table>
            </div>
            <!-- /.box-body -->
          </div>
          <!-- /.box -->
        </div>
        <!-- /.col -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
  </div>

  <!-- /.content-wrapper -->

  <footer class="main-footer">
    <div class="pull-right hidden-xs">
      <b>Version</b> 2.7.0
    </div>
    <strong>Copyright &copy; 2016 <a href="http://mitec.io">mitec.io</a>.</strong> All rights
    reserved.
  </footer>

  <!-- Add the sidebar's background. This div must be placed
       immediately after the control sidebar -->
  <div class="control-sidebar-bg"></div>
</div>
<!-- ./wrapper -->
<!-- jQuery 2.2.3 -->
<script src="Scripts/jquery-2.2.3.min.js"></script>
<!-- jQuery UI 1.11.4 -->
<script src="https://code.jquery.com/ui/1.11.4/jquery-ui.min.js"></script>
<!-- Resolve conflict in jQuery UI tooltip with Bootstrap tooltip -->
<script>
    $.widget.bridge('uibutton', $.ui.button);
</script>
<script>
    $(function () {
        $.fn.datepicker.defaults.format = "dd/mm/yyyy";
        $('#fecFrom').datepicker();
        $('#fecTo').datepicker();
        $('#fecVendDe').datepicker();
        $('#fecVendA').datepicker();
        $('#fecGenDe').datepicker();
        $('#fecGenA').datepicker();
        $('#fecCalDe').datepicker();
        $('#fecCalA').datepicker();
      var table = $("#mainTable").DataTable({
          "sScrollX": true
      });

      /*$('#mainTable tbody').on('click', 'tr', function () {
          document.getElementById('Hidden1').value = table.row(this).data()[0];
          document.getElementById('iden').innerHTML = table.row(this).data()[0];
          document.getElementById('ape').value = table.row(this).data()[6];
          document.getElementById('nom').value = table.row(this).data()[7];
          document.getElementById('tDni').value = table.row(this).data()[8];
          document.getElementById('dni').value = table.row(this).data()[9];
          document.getElementById('nac').value = table.row(this).data()[10];
          document.getElementById('prov').value = table.row(this).data()[11];
          document.getElementById('loc').value = table.row(this).data()[12];
          document.getElementById('dir').value = table.row(this).data()[13];
          document.getElementById('num').value = table.row(this).data()[14];
          document.getElementById('dto').value = table.row(this).data()[15];
          document.getElementById('piso').value = table.row(this).data()[16];
          document.getElementById('cp').value = table.row(this).data()[17];
          document.getElementById('mail').value = table.row(this).data()[18];
          document.getElementById('tipoTc').value = table.row(this).data()[19];
          document.getElementById('nroTc').value = table.row(this).data()[20];
          document.getElementById('venTc').value = table.row(this).data()[21];
          document.getElementById('tel').value = table.row(this).data()[26];
      });*/
  });
</script>
<!-- Bootstrap 3.3.6 -->
<script src="Scripts/bootstrap.min.js"></script>
<!-- Morris.js charts -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/raphael/2.1.0/raphael-min.js"></script>
<script src="plugins/morris/morris.min.js"></script>
<!-- Sparkline -->
<script src="plugins/sparkline/jquery.sparkline.min.js"></script>
<!-- jvectormap -->
<script src="plugins/jvectormap/jquery-jvectormap-1.2.2.min.js"></script>
<script src="plugins/jvectormap/jquery-jvectormap-world-mill-en.js"></script>
<!-- jQuery Knob Chart -->
<script src="plugins/knob/jquery.knob.js"></script>
<!-- daterangepicker -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.11.2/moment.min.js"></script>
<script src="plugins/daterangepicker/daterangepicker.js"></script>
<!-- datepicker -->
<script src="plugins/datepicker/bootstrap-datepicker.js"></script>
<!-- Bootstrap WYSIHTML5 -->
<script src="plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js"></script>
<!-- DataTables -->
<script src="plugins/datatables/jquery.dataTables.min.js"></script>
<script src="plugins/datatables/dataTables.bootstrap.min.js"></script>
<!-- SelectPicker -->
<script src="Scripts/bootstrap-select.js"></script>
<script src="Scripts/bootstrap-select.min.js"></script>
<!-- Slimscroll -->
<script src="plugins/slimScroll/jquery.slimscroll.min.js"></script>
<!-- FastClick -->
<script src="plugins/fastclick/fastclick.js"></script>
<!-- AdminLTE App -->
<script src="Scripts/app.min.js"></script>
<!-- AdminLTE dashboard demo (This is only for demo purposes) -->
<script src="Scripts/dashboard.js"></script>
<!-- AdminLTE for demo purposes -->
<script src="Scripts/demo.js"></script>
</form>
</body>

</html>
