<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="sales._default1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta charset="utf-8" />
  <meta http-equiv="X-UA-Compatible" content="IE=edge" />
  <title>Login | sales.io</title>
  <!-- Tell the browser to be responsive to screen width -->
  <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport" />
  <!-- Bootstrap 3.3.6 -->
  <link rel="stylesheet" href="Content/bootstrap.min.css" />
  <!-- Font Awesome -->
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.5.0/css/font-awesome.min.css" />
  <!-- Ionicons -->
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ionicons/2.0.1/css/ionicons.min.css" />s
  <!-- Theme style -->
  <link rel="stylesheet" href="Content/AdminLTE.min.css" />

  <link rel="stylesheet" href="plugins/iCheck/flat/blue.css" />
</head>
<body class="hold-transition login-page">
    <form id="form1" runat="server">
    <div class="login-box">
        <div class="login-logo">
            <a><b>TOUCHED</b>BPO</a>&nbsp; | &nbsp;<small>sales.<span style="color:#428bca;">io</span></small>
        </div>
        <div class="login-box-body">
    <p class="login-box-msg">Ingrese usuario y contraseña</p>
      <div class="inner-addon right-addon">
        <i class="glyphicon glyphicon-user"></i>
        <input id="TextName" runat="server" type="text" class="form-control" placeholder="Usuario" />
      </div>
            <br />
      <div class="inner-addon right-addon">
        <i class="glyphicon glyphicon-lock"></i>
        <input id="TextPass" runat="server" type="password" class="form-control" placeholder="Calve" />
      </div>
            <br />
      <div class="row">
        <div class="col-xs-4">
            <asp:Button ID="BtnLogin" runat="server" CssClass="btn btn-primary btn-block btn-flat" Text="Ingresar" OnClick="Login_Click"></asp:Button>
        </div>
        <!-- /.col -->
      </div>
  </div>
        <style>
            .inner-addon { 
                position: relative; 
            }

            /* style icon */
            .inner-addon .glyphicon {
              position: absolute;
              padding: 10px;
              pointer-events: none;
            }

            /* align icon */
            .left-addon .glyphicon  { left:  0px;}
            .right-addon .glyphicon { right: 0px;}

            /* add padding  */
            .left-addon input  { padding-left:  30px; }
            .right-addon input { padding-right: 30px; }
        </style>
    </div>
    </form>
</body>

</html>
