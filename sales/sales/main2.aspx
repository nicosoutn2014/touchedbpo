<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main2.aspx.cs" Inherits="sales.main2" %>

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
            <i class="fa fa-pencil"></i> <span>Correcciones</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
        </li>
        <li class="treeview active" id="scoring" runat="server">
          <a href="#">
            <i class="fa fa-random"></i> <span>Scoring</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
          <ul class="treeview-menu">
            <li><a href="#modalCargaScor" data-toggle="modal" data-target="#modalCargaScor"><i class="fa fa-circle-o"></i>Carga de Datos</a></li>
          </ul>
        </li>
        <li class="active treeview" id="tablero" runat="server">
          <a href="#">
            <i class="fa fa-dashboard"></i> <span>Tablero</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
          <ul class="treeview-menu">
            <li><a href="dashboard.aspx"><i class="fa fa-circle-o"></i>Consultar Bases</a></li>
          </ul>
        </li>
          <li class="active treeview" id="retorno" runat="server">
          <a href="#">
            <i class="fa fa-check"></i> <span>Retornos</span>
            <span class="pull-right-container">
              <i class="fa fa-angle-right pull-right"></i>
            </span>
          </a>
          <ul class="treeview-menu">
            <li><a href="main.aspx"><i class="fa fa-circle-o"></i>Cargar Retornos</a></li>
          </ul>
        </li>
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
    <section class="content-header">
      <h1>
        sales<span style="color:darkgray;">.io</span>
        <small>Panel de Control</small>
      </h1>
        <!--<div><span style="font-family:Effra;">Lote Seleccionado:&nbsp;&nbsp;</span><asp:Label ID="LblLote" runat="server" Text="--Ninguno--" Font-Bold="true"></asp:Label></div> -->
      <ol class="breadcrumb">
        <li><a href="#"><i class="fa fa-dashboard"></i> Home</a></li>
        <li class="active">Tablero</li>
      </ol>
    </section>

    <!-- Main content -->
    <section class="content">

      <!-- Small boxes (Stat box) -->
      <div class="row">
        <div class="col-lg-4 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-aqua">
            <div class="inner">
              <h3><asp:Literal ID="LtlVta" runat="server"></asp:Literal></h3>

              <p>Ventas</p>
            </div>
            <div class="icon">
              <i class="ion ion-bag"></i>
            </div>
            <a href="#" class="small-box-footer">Más info<i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-4 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-green">
            <div class="inner">
              <h3><asp:Literal ID="LtlOk" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Positivas</p>
            </div>
            <div class="icon">
              <i class="ion ion-stats-bars"></i>
            </div>
            <a href="#" class="small-box-footer">Más info <i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-4 col-xs-6">
          <!-- small box -->
          <div class="small-box bg-red">
            <div class="inner">
              <h3><asp:Literal ID="LtlNeg" runat="server"></asp:Literal><!-- <sup style="font-size: 20px">%</sup>--></h3>

              <p>Negativas</p>
            </div>
            <div class="icon">
              <i class="ion ion-pie-graph"></i>
            </div>
            <a href="#" class="small-box-footer">Más info <i class="fa fa-arrow-circle-right"></i></a>
          </div>
        </div>
      </div>
      <!-- /.row -->
      
      <div class="row">
        <div class="col-xs-12">
          <div class="box">
            <div class="box-header">
              <h3 class="box-title">Correcciones Pendientes:</h3>
                <p style="font-style:italic;">(Click en la fila para editar los datos)</p>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
              <asp:PlaceHolder ID="Lotes" runat="server"></asp:PlaceHolder> 
                <br />
                <br />
              <table id="dtCorr" class="table table-bordered table-striped" style="overflow:auto;">
                    <asp:Literal ID="theadN" runat="server"></asp:Literal>
                    <asp:Literal ID="tbodyN" runat="server"></asp:Literal>
              </table>
              <br />
                <h3 class="box-title" style="font-size:18px;">Datos Cargados:</h3>
                <asp:Literal ID="LtlDataLoaded" runat="server"></asp:Literal>
                <asp:Label ID="LblNomLotVal" runat="server" Text="" Font-Bold="true"></asp:Label>
                <br />
                <br />
              <table id="dtScor" class="table table-bordered table-striped" style="overflow:auto;">
                <asp:Literal ID="thead" runat="server"></asp:Literal>
                <asp:Literal ID="tbody" runat="server"></asp:Literal>
              </table>
                <br />
                <asp:Label ID="Label" runat="server" Text="Negativas:" Font-Bold="true"></asp:Label>
                <asp:Literal ID="LtlScorWrong" runat="server"></asp:Literal>
              <table id="dtScorWrong" class="table table-bordered table-striped" style="overflow:auto;">
                <asp:Literal ID="theadW" runat="server"></asp:Literal>
                <asp:Literal ID="tbodyW" runat="server"></asp:Literal>
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

    <div class="modal-example" >
        <div id="modalCargaScor" class="modal modal-primary" role="dialog">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                  <span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title">Carga de Datos - Scoring</h4>
              </div>
              <div class="modal-body">
                <p>Seleccione el archivo:</p>
                  <asp:Label ID="LblLastLote" runat="server" Text="Ultimo Lote: " BackColor="Black" Font-Bold="true"></asp:Label><br /><br />
                <asp:FileUpload ID="FileUpload" AllowMultiple="true" CssClass="btn btn-info" runat="server" Width="300"></asp:FileUpload>
                <small style="color:black;">Formatos aceptados: .txt;</small>
                 <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
              </div>
              <div class="modal-footer" style="margin:0;">
                <button type="button" class="btn btn-outline pull-left" data-dismiss="modal">Cerrar</button>
                <asp:Button runat="server" ID="BtnCargaScor" CssClass="btn btn-outline" Text="Cargar Data" OnClick="BtnCargaScor_Click"></asp:Button>
              </div>
            </div>
            <!-- /.modal-content -->
          </div>
          <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
      </div>

        
      <div class="modal-example">
        <div id="modalRow" class="modal modal-primary fade-in" role="dialog">
          <div class="modal-dialog" style="width:60%">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                  <span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title">Modificar Datos de Cliente ID: &nbsp;<asp:Label ID="iden" runat="server" Text=""></asp:Label></h4>
                <asp:HiddenField ID="Hidden1" runat="server" />
                <asp:HiddenField ID="Hidden2" runat="server" /> 
              </div>
              <div class="modal-body">
                <div class="form-group row">
                    <label for="example-text-input" class="col-xs-2 col-form-label">Fecha Venta</label>
                    <div class="col-xs-4">
                        <input id="fecVta" runat="server" class="form-control" type="text" readonly/>
                    </div>
                    <label for="example-text-input" class="col-xs-1 col-form-label">Obs</label>
                    <div class="col-xs-5">
                        <textarea id="obser" rows="3" runat="server" class="form-control" type="text" readonly/>
                    </div>
                </div>
                <div class="form-group row">
                  <label for="example-text-input" class="col-xs-2 col-form-label">Apellido</label>
                  <div class="col-xs-10">
                    <input id="ape" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-search-input" class="col-xs-2 col-form-label">Nombre</label>
                  <div class="col-xs-10">
                    <input id="nom" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-email-input" class="col-xs-2 col-form-label">Tipo Dni</label>
                  <div class="col-xs-10">
                    <input id="tDni" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-url-input" class="col-xs-2 col-form-label">DNI</label>
                  <div class="col-xs-10">
                    <input id="dni" runat="server" class="form-control disabled" type="text" readonly/>
                    <a href="#modalDNI" style="color:white;" data-toggle="modal" data-target="#modalDNI"><i class="fa fa-pencil"></i>&nbsp;&nbsp;Modificar DNI</a>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-url-input" class="col-xs-2 col-form-label">Fecha Nacimiento</label>
                  <div class="col-xs-10">
                    <input id="nac" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-tel-input" class="col-xs-2 col-form-label">Provincia</label>
                  <div class="col-xs-10">
                    <input id="prov" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-password-input" class="col-xs-2 col-form-label">Localidad</label>
                  <div class="col-xs-10">
                    <input id="loc" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-number-input" class="col-xs-2 col-form-label">Calle</label>
                  <div class="col-xs-10">
                    <input id="dir" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-datetime-local-input" class="col-xs-2 col-form-label">Nro Calle</label>
                  <div class="col-xs-10">
                    <input id="num" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-date-input" class="col-xs-2 col-form-label">Depto</label>
                  <div class="col-xs-10">
                    <input id="dto" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-month-input" class="col-xs-2 col-form-label">Piso</label>
                  <div class="col-xs-10">
                    <input id="piso" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-week-input" class="col-xs-2 col-form-label">CP</label>
                  <div class="col-xs-10">
                    <input id="cp" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-week-input" class="col-xs-2 col-form-label">Mail</label>
                  <div class="col-xs-10">
                    <input id="mail" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-time-input" class="col-xs-2 col-form-label">Tipo TC</label>
                  <div class="col-xs-10">
                    <input id="tipoTc" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-color-input" class="col-xs-2 col-form-label">Nro TC</label>
                  <div class="col-xs-10">
                    <input id="nroTc" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-color-input" class="col-xs-2 col-form-label">Vencimiento TC</label>
                  <div class="col-xs-10">
                    <input id="venTc" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
                <div class="form-group row">
                  <label for="example-color-input" class="col-xs-2 col-form-label">Telefono</label>
                  <div class="col-xs-10">
                    <input id="tel" runat="server" class="form-control" style="text-transform:uppercase;" type="text"/>
                  </div>
                </div>
              </div>
              <div class="modal-footer" style="margin:0;">
                <button type="button" class="btn btn-outline pull-left" data-dismiss="modal">Cerrar</button>
                <!--<asp:Button runat="server" ID="BtnPend" CssClass="btn btn-warning" Text="Marcar Pendiente" CommandName="pend" CommandArgument="pend" OnClick="BtnModif_Click"></asp:Button> -->
                <asp:Button runat="server" ID="BtnRech" CssClass="btn btn-danger" Text="Rechazar Cliente" CommandName="rech" CommandArgument="rech" OnClick="BtnModif_Click"></asp:Button>
                <asp:Button runat="server" ID="BtnModif" CssClass="btn btn-success" Text="Guardar Cambios" CommandName="modif" CommandArgument="modif" OnClick="BtnModif_Click"></asp:Button>
              </div>
            </div>
            <!-- /.modal-content -->
          </div>
          <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
      </div>

    <div class="modal-example">
        <asp:UpdatePanel runat="server" ID="UpDNI">
            <ContentTemplate>
                <div id="modalDNI" class="modal modal-default" role="dialog">
          <div class="modal-dialog">
            <div class="modal-content" style="margin-top:25%;">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                  <span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title">Modificar DNI</h4>
              </div>
              <div class="modal-body">
                <div class="form-group row">
                  <label for="example-url-input" class="col-xs-2 col-form-label">DNI</label>
                  <div class="col-xs-10">
                    <input id="dni2" runat="server" class="form-control" type="text"/>
                    <br />
                    <br />
                  </div>
                </div>
              </div>
              <div class="modal-footer" style="margin:0;">
                <button type="button" class="btn btn-outline btn-primary pull-left" data-dismiss="modal">Cerrar</button>
                <asp:Button ID="BtnMdfDNI" runat="server" CssClass="btn btn-default" Text="Modificar" OnClick="BtnMdfDNI_Click" />
              </div>
            </div>
            <!-- /.modal-content -->
          </div>
          <!-- /.modal-dialog -->
        </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <!-- /.modal -->
      </div>

  <!-- /.content-wrapper -->

  <footer class="main-footer">
    <div class="pull-right hidden-xs">
      <b>Version</b> 2.7.0
    </div>
    <strong>Copyright &copy; 2016 <a href="http://mitec.io">mitec.io</a>.</strong> All rights
    reserved.
  </footer>

  <!-- Control Sidebar -->
  <aside class="control-sidebar control-sidebar-dark">
    <!-- Create the tabs -->
    <ul class="nav nav-tabs nav-justified control-sidebar-tabs">
      <li><a href="#control-sidebar-home-tab" data-toggle="tab"><i class="fa fa-home"></i></a></li>
    </ul>
    <!-- Tab panes -->
    <div class="tab-content">
      <!-- Home tab content -->
      <div class="tab-pane" id="control-sidebar-home-tab">
        <h3 class="control-sidebar-heading">Actividad Reciente</h3>
        <ul class="control-sidebar-menu">
          <li>
            <a href="javascript:void(0)">
              <i class="menu-icon fa fa-minus-circle bg-red"></i>

              <div class="menu-info">
                <h4 class="control-sidebar-subheading">2 ventas rechazadas</h4>

                <p></p>
              </div>
            </a>
          </li>
          <li>
            <a href="javascript:void(0)">
              <i class="menu-icon fa fa-check-circle bg-green"></i>

              <div class="menu-info">
                <h4 class="control-sidebar-subheading">9 ventas confirmadas</h4>

                <p></p>
              </div>
            </a>
          </li>
        </ul>
        <!-- /.control-sidebar-menu -->

        <h3 class="control-sidebar-heading">Progreso Objetivos</h3>
        <ul class="control-sidebar-menu">
          <li>
            <a href="javascript:void(0)">
              <h4 class="control-sidebar-subheading">
                Equipo A
                <span class="label label-info pull-right">60%</span>
              </h4>

              <div class="progress progress-xxs">
                <div class="progress-bar progress-bar-danger" style="width: 70%"></div>
              </div>
            </a>
          </li>
          <li>
            <a href="javascript:void(0)">
              <h4 class="control-sidebar-subheading">
                Equipo B
                <span class="label label-success pull-right">90%</span>
              </h4>
              <div class="progress progress-xxs">
                <div class="progress-bar progress-bar-danger" style="width: 90%"></div>
              </div>
            </a>
          </li>
          <li>
            <a href="javascript:void(0)">
              <h4 class="control-sidebar-subheading">
                Equipo C
                <span class="label label-warning pull-right">33%</span>
              </h4>

              <div class="progress progress-xxs">
                <div class="progress-bar progress-bar-warning" style="width: 33%"></div>
              </div>
            </a>
          </li>
        </ul>
        <!-- /.control-sidebar-menu -->

      </div>
      <!-- /.tab-pane -->
    </div>
  </aside>
  <!-- /.control-sidebar -->
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

    function changeDNI() {
        document.getElementById('dni').value = document.getElementById('dni2').value;
    }
</script>
<script>
    function pageLoad() {
        $("#dtScor").DataTable({
            "sScrollX": true 
        });
        var table2 = $("#dtScorWrong").DataTable({
            "sScrollX": true
        });

        $('#dtScorWrong tbody').on('click', 'tr', function () {
            var i = 19;
            document.getElementById('Hidden1').value = table2.row(this).data()[i + 0];
            document.getElementById('Hidden2').value = table2.row(this).data()[i - 1];
            document.getElementById('iden').innerHTML = table2.row(this).data()[i+0];
            document.getElementById('ape').value = table2.row(this).data()[i+6];
            document.getElementById('nom').value = table2.row(this).data()[i+7];
            document.getElementById('tDni').value = table2.row(this).data()[i+8];
            document.getElementById('dni').value = table2.row(this).data()[i + 9];
            document.getElementById('dni2').value = table2.row(this).data()[i + 9];
            document.getElementById('nac').value = table2.row(this).data()[i+10];
            document.getElementById('prov').value = table2.row(this).data()[i+11];
            document.getElementById('loc').value = table2.row(this).data()[i+12];
            document.getElementById('dir').value = table2.row(this).data()[i+13];
            document.getElementById('num').value = table2.row(this).data()[i+14];
            document.getElementById('dto').value = table2.row(this).data()[i+15];
            document.getElementById('piso').value = table2.row(this).data()[i+16];
            document.getElementById('cp').value = table2.row(this).data()[i+17];
            document.getElementById('mail').value = table2.row(this).data()[i+18];
            document.getElementById('tipoTc').value = table2.row(this).data()[i+19];
            document.getElementById('nroTc').value = table2.row(this).data()[i+20];
            document.getElementById('venTc').value = table2.row(this).data()[i+21];
            document.getElementById('tel').value = table2.row(this).data()[i + 26];
            document.getElementById('fecVta').value = table2.row(this).data()[i + 5];
            document.getElementById('obser').value = table2.row(this).data()[i - 6];
        });
    }
</script>
<script>
    function pageLoad() {
        var table = $("#dtCorr").DataTable({
            "sScrollX": true
        });

        $('#dtCorr tbody').on('click', 'tr', function () {
            var lote = "<%=(string)Session["loteSub"]%>";
            var i = 0;
            if (lote.indexOf("Scoring") >= 0)
                i = 19;
            document.getElementById('Hidden1').value = table.row(this).data()[i];
            document.getElementById('Hidden2').value = table.row(this).data()[i - 1];
            document.getElementById('iden').innerHTML = table.row(this).data()[i];
            document.getElementById('ape').value = table.row(this).data()[i + 6];
            document.getElementById('nom').value = table.row(this).data()[i + 7];
            document.getElementById('tDni').value = table.row(this).data()[i + 8];
            document.getElementById('dni').value = table.row(this).data()[i + 9];
            document.getElementById('dni2').value = table.row(this).data()[i + 9];
            document.getElementById('nac').value = table.row(this).data()[i + 10];
            document.getElementById('prov').value = table.row(this).data()[i + 11];
            document.getElementById('loc').value = table.row(this).data()[i + 12];
            document.getElementById('dir').value = table.row(this).data()[i + 13];
            document.getElementById('num').value = table.row(this).data()[i + 14];
            document.getElementById('dto').value = table.row(this).data()[i + 15];
            document.getElementById('piso').value = table.row(this).data()[i + 16];
            document.getElementById('cp').value = table.row(this).data()[i + 17];
            document.getElementById('mail').value = table.row(this).data()[i + 18];
            document.getElementById('tipoTc').value = table.row(this).data()[i + 19];
            document.getElementById('nroTc').value = table.row(this).data()[i + 20];
            document.getElementById('venTc').value = table.row(this).data()[i + 21];
            document.getElementById('tel').value = table.row(this).data()[i + 26];
            document.getElementById('fecVta').value = table.row(this).data()[i + 5];
            if (lote.indexOf("Scoring") >= 0)
                document.getElementById('obser').value = table.row(this).data()[i - 6];
            else
                document.getElementById('obser').value = table.row(this).data()[i + 35];
        });
    }
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
