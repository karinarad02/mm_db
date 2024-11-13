<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProiectEdit.aspx.cs" Inherits="Proiect.ProiectEdit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Editare Tablou</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            color: #333;
            display: flex;
            justify-content: center;
            padding: 20px;
        }

        form {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
        }

        #Panel1 {
            background-color: #000066;
            color: #fff;
            padding: 20px;
            text-align: center;
            border-radius: 8px;
            margin-bottom: 20px;
        }

            #Panel1 .title {
                font-size: 36px;
                font-weight: bold;
                margin: 0;
            }

        #Panel2 {
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 8px;
        }

            #Panel2 label {
                display: block;
                margin-top: 15px;
                font-weight: bold;
            }

            #Panel2 input, #Panel2 select {
                width: -webkit-fill-available;
                padding: 10px;
                margin-top: 5px;
                font-size: 14px;
                border: 1px solid #ccc;
                border-radius: 4px;
            }

                #Panel2 input[type="file"] {
                    padding: 2;
                }

            #Panel2 button, #Panel2 .btn {
                background-color: #FF9900;
                color: #fff;
                border: none;
                padding: 15px 30px;
                font-size: 16px;
                font-weight: bold;
                cursor: pointer;
                margin-top: 20px;
                border-radius: 8px;
                transition: background-color 0.3s;
            }

                #Panel2 button:hover, #Panel2 .btn:hover {
                    background-color: #e08800;
                }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="Panel1" runat="server">
            <h1 class="title">Editeaza opera</h1>
        </asp:Panel>

        <asp:Panel ID="Panel2" runat="server">

            <asp:Label ID="Label2" runat="server" Text="Status:"></asp:Label>
            <asp:Label ID="lbl_status" runat="server" Text="......."></asp:Label>

            <br />
            <asp:Label ID="Label7" runat="server" Text="Opera curenta:"></asp:Label>
            <br />
            <asp:Image ID="Image1" runat="server" Height="206px" Width="235px" />

            <br />

            <asp:Label ID="Label3" runat="server" Text="Editati datele despre opera de arta:"></asp:Label>

            <br />

            <asp:Label ID="Label4" runat="server" Text="Titlu:"></asp:Label>
            <asp:TextBox ID="tb_titlu" runat="server"></asp:TextBox>

            <br />

            <asp:Label ID="Label5" runat="server" Text="Artist:"></asp:Label>
            <asp:DropDownList ID="drop_artisti_i" runat="server"></asp:DropDownList>

            <br />

            <asp:Label ID="Label6" runat="server" Text="Imagine:"></asp:Label>
            <asp:FileUpload ID="FileUpload1" runat="server" />

            <br />

            <asp:Button ID="btn_update" runat="server" Text="Actualizează opera" CssClass="btn" OnClick="btn_update_Click" />

            <br />
        </asp:Panel>
    </form>
</body>
</html>
