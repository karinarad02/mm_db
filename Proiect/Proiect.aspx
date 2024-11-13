<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Proiect.aspx.cs" Inherits="Proiect.Proiect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Galerie de Arta</title>
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

        #Panel2, #Panel3, #Panel4, #Panel5, #Panel6 {
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 8px;
        }

            #Panel2 label, #Panel3 label, #Panel5 label, #Panel6 label {
                display: block;
                margin-top: 15px;
                font-weight: bold;
            }

            #Panel2 input, #Panel2 select, #Panel3 input, #Panel3 select, #Panel5 input, #Panel5 select, #Panel6 input, #Panel6 select {
                width: -webkit-fill-available;
                padding: 10px;
                margin-top: 5px;
                font-size: 14px;
                border: 1px solid #ccc;
                border-radius: 4px;
            }

                #Panel2 input[type="file"], #Panel5 input[type="file"], #Panel6 input[type="file"] {
                    padding: 2;
                }

            #Panel2 button, #Panel2 .btn, #Panel3 button, #Panel3 .btn, #Panel5 button, #Panel5 .btn, #Panel6 button, #Panel6 .btn {
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

                #Panel2 button:hover, #Panel2 .btn:hover, #Panel3 button:hover, #Panel3 .btn:hover, #Panel4 button:hover,
                #Panel4 .btn:hover, #Panel5 button:hover, #Panel5 .btn:hover, #Panel6 button:hover, #Panel6 .btn:hover {
                    background-color: #e08800;
                }

            #Panel4 button, #Panel4 .btn {
                background-color: #FF9900;
                color: #fff;
                border: none;
                padding: 0px 30px;
                font-size: 16px;
                font-weight: bold;
                cursor: pointer;
                margin-top: 20px;
                border-radius: 8px;
                transition: background-color 0.3s;
                height: 50px;
            }

        .repeater-wrapper {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 20px;
            justify-content: center;
        }

        .repeater-wrapper-list {
            display: flex;
            flex-direction: column;
            gap: 10px;
            margin-top: 20px;
            justify-content: center;
            max-height: 400px;
            overflow-y: auto;
            border: 1px solid #ddd;
            padding: 10px;
        }

        .repeater-item {
            width: 200px;
            height: 200px;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        }

            .repeater-item img, #Panel5 img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }

        .repeater-item-list {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

            .repeater-item-list img {
                height 100px;
                width: 100px;
                object-fit: cover;
            }

            .repeater-item-list div {
                display: flex;
                flex-direction: column;
                justify-content: center;
            }

        .display-img-wrapper {
            justify-items: center;
        }

        .img-wrapper {
            width: 400px;
            height: 400px;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        }

        #lb_titlu {
            font-size: 26px;
            font-weight: bold;
            margin: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="Panel1" runat="server">
            <h1 class="title">Galerie de Arta</h1>
        </asp:Panel>

        <asp:Panel ID="Panel2" runat="server">
            <asp:Label ID="Label2" runat="server" Text="Status:"></asp:Label>
            <asp:Label ID="lbl_status" runat="server" Text="......."></asp:Label>

            <br />

            <asp:Label ID="Label3" runat="server" Text="Introduceti date despre operele de arta:"></asp:Label>

            <br />
            <asp:Label ID="Label4" runat="server" Text="Titlu:"></asp:Label>
            <asp:TextBox ID="tb_titlu" runat="server"></asp:TextBox>

            <br />

            <asp:Label ID="Label5" runat="server" Text="Artist:"></asp:Label>
            <asp:DropDownList ID="drop_artisti_i" runat="server" OnLoad="drop_artisti_i_Load"></asp:DropDownList>

            <br />

            <asp:Label ID="Label6" runat="server" Text="Imagine:"></asp:Label>
            <asp:FileUpload ID="FileUpload1" runat="server" />

            <br />

            <asp:Button ID="btn_inserare" runat="server" Text="Insereaza opera" CssClass="btn" OnClick="btn_inserare_Click" />

            <br />
        </asp:Panel>

        <br />

        <br />

        <asp:Panel ID="Panel3" runat="server">
            <asp:Label ID="Label7" runat="server" Text="Afisare opere de arta:"></asp:Label>

            <br />

            <asp:Label ID="Label8" runat="server" Text="In functie de artist:"></asp:Label>
            <asp:DropDownList ID="drop_artisti_a" runat="server" OnLoad="drop_artisti_a_Load"></asp:DropDownList>

            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <div class="repeater-wrapper">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="repeater-item">
                        <img src='data:image/jpeg;base64,<%# Container.DataItem %>' alt='Image' />
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <asp:Button ID="btn_afisare" runat="server" Text="Afiseaza opere" CssClass="btn" OnClick="btn_afisare_Click" />

        </asp:Panel>
        <br />
        <asp:Panel ID="Panel4" runat="server" CssClass="panel-with-scrollbar">
            <asp:Label ID="Label14" runat="server" Text="Lista opere de arta:"></asp:Label>
            <br />

            <asp:Repeater ID="Repeater2" runat="server" OnLoad="Repeater2_Load">
                <HeaderTemplate>
                    <div class="repeater-wrapper-list">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="repeater-item-list">
                        <img src="data:image/jpeg;base64,<%# Eval("ImagineBase64") %>" alt="<%# Eval("Titlu") %>" />
                        <div>
                            <p><strong>Titlu:</strong> <%# Eval("Titlu") %></p>
                            <p><strong>Artist:</strong> <%# Eval("Artist") %></p>
                        </div>
                        <asp:Button ID="btnEdit" runat="server" Text="Editeaza" CssClass="btn" CommandArgument='<%# Eval("IdTablou") %>' OnClick="btnEdit_Click" />
                        <asp:Button ID="btnDelete" runat="server" Text="Sterge" CssClass="btn" CommandArgument='<%# Eval("IdTablou") %>' OnClick="btnDelete_Click" />
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

        </asp:Panel>

        <br />
        <asp:Panel ID="Panel5" runat="server">
            <asp:Button ID="btn_gen_semn" runat="server" Text="Genereaza semnaturi" CssClass="btn" OnClick="btn_gen_semn_Click" />
            <br />
            <asp:Label ID="Label9" runat="server" Text="Regasire opera:"></asp:Label>
            <br />
            <asp:FileUpload ID="FileUpload2" runat="server" />
            <br />
            <asp:Label ID="Label10" runat="server" Text="Indice culoare:"></asp:Label>
            <asp:TextBox ID="tb_culoare" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label11" runat="server" Text="Indice textura:"></asp:Label>
            <asp:TextBox ID="tb_textura" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label12" runat="server" Text="Indice forma:"></asp:Label>
            <asp:TextBox ID="tb_forma" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label13" runat="server" Text="Indice locatie:"></asp:Label>
            <asp:TextBox ID="tb_locatie" runat="server"></asp:TextBox>
            <br />

            <br />
            <div class="display-img-wrapper">
                <div class="img-wrapper">
                    <asp:Image ID="img_regasita" runat="server" />
                </div>
                <br />
                <asp:Label ID="lb_titlu" runat="server" Text="Titlul operei"></asp:Label>
            </div>
            <br />
            <asp:Button ID="btn_regasire" runat="server" Text="Gaseste opera" CssClass="btn" OnClick="btn_regasire_Click" />
        </asp:Panel>

        <br />

        <asp:Panel ID="Panel6" runat="server">
            
            <asp:Label ID="Label15" runat="server" Text="Introduceti videoclipuri despre artisti sau evenimente:"></asp:Label>

            <br />
            <asp:Label ID="Label16" runat="server" Text="Titlu:"></asp:Label>
            <asp:TextBox ID="tb_titlu_video" runat="server"></asp:TextBox>

            <br />


            <asp:Label ID="Label17" runat="server" Text="Video:"></asp:Label>
            <asp:FileUpload ID="FileUpload3" runat="server" />

            <br />

            <asp:Button ID="btn_inserare_video" runat="server" Text="Insereaza video" CssClass="btn" OnClick="btn_inserare_video_Click" />

            <br />
            <br />
            <asp:Label ID="Label18" runat="server" Text="Videoclipul inserat:"></asp:Label>

            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
            <br />
        </asp:Panel>
    </form>
</body>
</html>
