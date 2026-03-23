<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebUI.WebForm1" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="border:1px solid Gray; border-collapse:collapse">
        <tr>
            <th>Hola</th>
            <th>Chao</th>
        </tr>
        <tr>
            <td>
            JAJAJAJA
            </td>
            <td>
            kjdlkasdja
            </td>
        </tr>
        </table>
        <asp:TextBox ID="txtmail" runat="server"></asp:TextBox>
        
        <asp:Button ID="Button1" runat="server" Text="Send" onclick="Button1_Click" />
        <br />
    
    <asp:TextBox ID="TextBox1" runat="server" Height="489px" TextMode="MultiLine" 
            Width="633px"></asp:TextBox>

        <asp:Label ID="lblmensaje" runat="server" Text="Label"></asp:Label>
    </div>
    </form>
</body>
</html>
