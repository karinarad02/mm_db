using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proiect
{
    public partial class Proiect : System.Web.UI.Page
    {
        OracleConnection con;
        private Dictionary<int, string> artistiDict;
        private List<string> imageBase64List;
        private List<Tablou> tablouriList= new List<Tablou>();

        //partea de loading

        protected void LoadList()
        {
            if (Application["TablouriList"] == null)
            {
                try
                {
                    con.Open();

                    OracleCommand cmd = new OracleCommand("lista_tablouri", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("flux", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        byte[] imageBytes = (byte[])reader["imagine"];
                        string imageBase64 = Convert.ToBase64String(imageBytes);

                        tablouriList.Add(new Tablou
                        {
                            IdTablou = reader.GetInt32(reader.GetOrdinal("id_tablou")),
                            Titlu = reader.GetString(reader.GetOrdinal("titlu")),
                            Artist = reader.GetString(reader.GetOrdinal("artist")),
                            ImagineBase64 = imageBase64
                        });
                    }

                    // Stocheaza tablourile in cache
                    Application["TablouriList"] = tablouriList;
                }
                catch (OracleException ex)
                {
                    lbl_status.Text = "EROARE!!! " + ex.Message;
                    lbl_status.ForeColor = System.Drawing.Color.Red;
                }
                catch (Exception ex)
                {
                    lbl_status.Text = "EROARE!!! " + ex.Message;
                    lbl_status.ForeColor = System.Drawing.Color.Red;
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                tablouriList = (List<Tablou>)Application["TablouriList"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string cons = "User ID=STUD_RADITOIUK; Password=student; Data Source=(DESCRIPTION=" +
            "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=37.120.249.41)(PORT=1521)))" +
            "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcls)));";

            con = new OracleConnection(cons);

            // IsPostBack <=> Reload al paginii
            if (!IsPostBack)
            {
                // Verifica daca datele pentru artisti sunt deja in cache
                if (Application["ArtistiDict"] == null)
                {
                    try
                    {
                        con.Open();

                        OracleCommand cmd = new OracleCommand("SELECT ID_ARTIST, NUME FROM ARTISTI_PROIECT", con);
                        artistiDict = new Dictionary<int, string>();

                        OracleDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            artistiDict.Add(reader.GetInt32(0), reader.GetString(1));
                        }

                        Application["ArtistiDict"] = artistiDict;
                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        lbl_status.Text = ex.Message;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
                else
                {
                    artistiDict = (Dictionary<int, string>)Application["ArtistiDict"];
                }

            }
            LoadList();


        }

        protected void drop_artisti_i_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (drop_artisti_i.Items.Count == 0)
                {
                    drop_artisti_i.Items.Add(new ListItem("Selectați un artist", "0"));

                    foreach (var artist in artistiDict)
                    {
                        drop_artisti_i.Items.Add(new ListItem(artist.Value, artist.Key.ToString()));
                    }
                }
            }
        }

        protected void drop_artisti_a_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (drop_artisti_a.Items.Count == 0)
                {
                    drop_artisti_a.Items.Add(new ListItem("Selectați un artist", "0"));

                    foreach (var artist in artistiDict)
                    {
                        drop_artisti_a.Items.Add(new ListItem(artist.Value, artist.Key.ToString()));
                    }
                }
            }
        }

        protected void Repeater2_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_status.Text = "";

                if (Repeater2.Items.Count == 0)
                {
                    Repeater2.DataSource = tablouriList;
                    Repeater2.DataBind();

                    lbl_status.Text = $"{tablouriList.Count} imagini găsite!";
                    lbl_status.ForeColor = System.Drawing.Color.Green;
                }
            }
        }

        //Incepere actiuni propriu zise
        protected void btn_inserare_Click(object sender, EventArgs e)
        {
            lbl_status.Text = "";
            lbl_status.ForeColor = System.Drawing.Color.Empty;


            int artistId = Convert.ToInt32(drop_artisti_i.SelectedValue);
            if (artistId == 0)
            {
                lbl_status.Text = "Selectați un artist valid!!!";
                lbl_status.ForeColor = System.Drawing.Color.Red;
                return;
            }
            else if (FileUpload1.HasFile)
            {
                FileUpload1.SaveAs(@"C:\Users\radit\source\repos\Proiect\imagini\" + FileUpload1.FileName);
                lbl_status.Text = "Fisier incarcat " + FileUpload1.FileName;
                using (var img = System.IO.File.OpenRead(@"C:\Users\radit\source\repos\Proiect\imagini\" + FileUpload1.FileName))
                {
                    var imageBytes = new byte[img.Length];
                    img.Read(imageBytes, 0, (int)img.Length);
                    lbl_status.Text = "Fisierul are dimensiunea " + img.Length.ToString();
                    lbl_status.ForeColor = System.Drawing.Color.Green;

                    try
                    {
                        con.Open();
                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = ex.Message;
                        lbl_status.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    OracleCommand cmd = new OracleCommand("ins_tabou_proiect", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("vtitle", OracleDbType.Varchar2);
                    cmd.Parameters.Add("vid_artist", OracleDbType.Int32);
                    cmd.Parameters.Add("fis", OracleDbType.Blob);
                    cmd.Parameters[0].Value = tb_titlu.Text;
                    cmd.Parameters[1].Value = Convert.ToInt32(drop_artisti_i.SelectedValue);
                    cmd.Parameters[2].Value = imageBytes;


                    try
                    {
                        cmd.ExecuteNonQuery();

                        tb_titlu.Text = "";
                        drop_artisti_i.SelectedIndex = 0;
                        FileUpload1.Attributes.Clear();
                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = ex.Message;
                        lbl_status.ForeColor = System.Drawing.Color.Red;
                    }
                    catch (Exception ex)
                    {
                        lbl_status.Text = ex.Message;
                        lbl_status.ForeColor = System.Drawing.Color.Red;
                    }
                    finally
                    {
                        con.Close();
                    }


                }
            }
            else
            {
                lbl_status.Text = "Imagine inexistenta!!!";
                lbl_status.ForeColor = System.Drawing.Color.Red;
                return;
            }
        }

        protected void btn_afisare_Click(object sender, EventArgs e)
        {
            lbl_status.Text = "";
            imageBase64List = new List<string>();
            imageBase64List.Clear();
            int artistId = Convert.ToInt32(drop_artisti_a.SelectedValue);

            if (artistId == 0)
            {
                lbl_status.Text = "Selectați un artist valid!!!";
                lbl_status.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                con.Open();
            }
            catch (OracleException ex)
            {
                lbl_status.Text = ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
                return;
            }

            var cmd = new OracleCommand("read_tablouri", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("artist_id", OracleDbType.Int32);
            cmd.Parameters.Add("flux", OracleDbType.RefCursor);
            cmd.Parameters[0].Direction = ParameterDirection.Input;
            cmd.Parameters[1].Direction = ParameterDirection.Output;
            cmd.Parameters[0].Value = artistId;

            try
            {

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        byte[] blobData = (byte[])reader[0];

                        if (blobData != null)
                        {
                            string base64String = Convert.ToBase64String(blobData);
                            imageBase64List.Add(base64String);
                        }
                    }
                }

                if (imageBase64List.Count <= 0)
                {
                    lbl_status.Text = $"Nu au fost gasite imagini pentru acest artist!!!";
                    lbl_status.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    Repeater1.DataSource = imageBase64List;
                    Repeater1.DataBind();

                    lbl_status.Text = $"{imageBase64List.Count} imagini gasite cu succes!";
                    lbl_status.ForeColor = System.Drawing.Color.Green;
                }

            }
            catch (OracleException ex)
            {
                lbl_status.Text = ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                lbl_status.Text = ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                con.Close();
            }

        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btnEdit = (Button)sender;
            int idTablou = Convert.ToInt32(btnEdit.CommandArgument);
            Response.Redirect($"ProiectEdit.aspx?idTablou={idTablou}", false);
        }

        //stergere
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btnDelete = (Button)sender;

            int idTablou = Convert.ToInt32(btnDelete.CommandArgument);
            if (idTablou == 0)
            {
                lbl_status.Text = "ID-ul tabloului nu este valid!";
                lbl_status.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                con.Open();

                OracleCommand cmd = new OracleCommand("DELETE FROM TABLOURI_PROIECT WHERE id_tablou = :idTablou", con);
                cmd.Parameters.Add(new OracleParameter(":idTablou", idTablou));

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    lbl_status.Text = "Tabloul a fost șters cu succes!";
                    lbl_status.ForeColor = System.Drawing.Color.Green;
                    if (tablouriList != null && tablouriList.Any())
                    {
                        tablouriList = tablouriList.Where(t => t.IdTablou != idTablou).ToList();

                        Application["TablouriList"] = tablouriList;

                        Repeater2.DataSource = tablouriList;
                        Repeater2.DataBind();
                    }
                }
                else
                {
                    lbl_status.Text = "Tabloul nu a fost găsit.";
                    lbl_status.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (OracleException ex)
            {
                lbl_status.Text = ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                lbl_status.Text = "Eroare la ștergerea tabloului: " + ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                con.Close();
            }
        }

        //semnaturi
        protected void btn_gen_semn_Click(object sender, EventArgs e)
        {
            lbl_status.Text = "";
            try
            {
                con.Open();
                OracleCommand cmd = new OracleCommand("gensemn_tablouri", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                lbl_status.Text = "Semnaturi generate!";
            }
            catch (OracleException ex)
            {
                lbl_status.Text = "EROARE!!! " + ex.Message;
            }
            con.Close();
        }

        //regasire
        protected void btn_regasire_Click(object sender, EventArgs e)
        {
            lbl_status.Text = "";
            if (FileUpload2.HasFile)
            {
                FileUpload2.SaveAs(@"C:\Users\radit\source\repos\Proiect\imagini_regasire\" + FileUpload2.FileName);
                using (var img = System.IO.File.OpenRead(@"C:\Users\radit\source\repos\Proiect\imagini_regasire\" + FileUpload2.FileName))
                {
                    var imageBytes = new byte[img.Length];
                    img.Read(imageBytes, 0, imageBytes.Length);

                    try
                    {
                        con.Open();

                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = "EROARE!!! " + ex.Message;
                    }

                    OracleCommand cmd1 = new OracleCommand("regasire_tablou", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("fis", OracleDbType.Blob);
                    cmd1.Parameters.Add("cculoare", OracleDbType.Decimal);
                    cmd1.Parameters.Add("ctextura", OracleDbType.Decimal);
                    cmd1.Parameters.Add("cforma", OracleDbType.Decimal);
                    cmd1.Parameters.Add("clocatie", OracleDbType.Decimal);
                    cmd1.Parameters.Add("idrez", OracleDbType.Int32);
                    cmd1.Parameters.Add("titlurez", OracleDbType.Varchar2, 255);
                    for (int i = 0; i < 5; i++)
                    {
                        cmd1.Parameters[i].Direction = ParameterDirection.Input;
                    }
                    cmd1.Parameters[0].Value = imageBytes;
                    cmd1.Parameters[5].Direction = ParameterDirection.Output;
                    cmd1.Parameters[6].Direction = ParameterDirection.Output;
                    cmd1.Parameters[1].Value = Convert.ToDecimal(tb_culoare.Text);
                    cmd1.Parameters[2].Value = Convert.ToDecimal(tb_textura.Text);
                    cmd1.Parameters[3].Value = Convert.ToDecimal(tb_forma.Text);
                    cmd1.Parameters[4].Value = Convert.ToDecimal(tb_locatie.Text);

                    try
                    {
                        cmd1.ExecuteScalar();

                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = "EROARE!!! " + ex.Message;
                    }
                    finally
                    {
                        con.Close();
                    }

                    lb_titlu.Text = cmd1.Parameters[6].Value.ToString();
                    int id_regasit = ((OracleDecimal)cmd1.Parameters[5].Value).ToInt32();

                    //Afisare imagine
                    img_regasita.ImageUrl = "";
                    lbl_status.Text = "";
                    try
                    {
                        con.Open();
                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = "EROARE!!! " + ex.Message;
                    }

                    OracleCommand cmd2 = new OracleCommand("afisare_tablou", con);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.Add("vid", OracleDbType.Int32);
                    cmd2.Parameters.Add("flux", OracleDbType.Blob);
                    cmd2.Parameters[0].Direction = ParameterDirection.Input;
                    cmd2.Parameters[1].Direction = ParameterDirection.Output;
                    cmd2.Parameters[0].Value = id_regasit;

                    try
                    {
                        cmd2.ExecuteScalar();
                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = "EROARE!!! " + ex.Message;
                    }

                    Byte[] blob = new Byte[((OracleBlob)cmd2.Parameters[1].Value).Length];

                    try
                    {
                        ((OracleBlob)cmd2.Parameters[1].Value).Read(blob, 0, blob.Length);

                        con.Close();

                        string myimg = Convert.ToBase64String(blob);
                        img_regasita.ImageUrl = "data:image/gif;base64," + myimg;
                    }
                    catch (Exception ex)
                    {
                        lbl_status.Text = "EROARE!!! " + ex.Message;
                    }


                }

            }
        }

        //video
        protected void btn_inserare_video_Click(object sender, EventArgs e)
        {
            lbl_status.Text = "";
            lbl_status.ForeColor = System.Drawing.Color.Empty;

            
            if (FileUpload3.HasFile)
            {
                string videoPath = @"C:\Users\radit\source\repos\Proiect\Proiect\video\" + FileUpload3.FileName;
                FileUpload3.SaveAs(videoPath);
                lbl_status.Text = "Fisier video incarcat " + FileUpload3.FileName;

                using (var videoStream = System.IO.File.OpenRead(videoPath))
                {
                    var videoBytes = new byte[videoStream.Length];
                    videoStream.Read(videoBytes, 0, (int)videoStream.Length);
                    lbl_status.Text = "Fisierul video are dimensiunea " + videoStream.Length.ToString();
                    lbl_status.ForeColor = System.Drawing.Color.Green;

                    try
                    {
                        con.Open();
                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = ex.Message;
                        lbl_status.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    OracleCommand cmd = new OracleCommand("ins_informatii_proiect", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("vtitle", OracleDbType.Varchar2);
                    cmd.Parameters.Add("fis", OracleDbType.Blob); 
                    cmd.Parameters[0].Value = tb_titlu_video.Text;
                    cmd.Parameters[1].Value = videoBytes; 

                    try
                    {
                        cmd.ExecuteNonQuery();

                        tb_titlu_video.Text = "";
                        FileUpload3.Attributes.Clear();
                        lbl_status.Text = "Video inserat cu succes!";
                        lbl_status.ForeColor = System.Drawing.Color.Green;

                        string videoUrl = "/video/" + FileUpload3.FileName; 
                        string videoPlayer = "<video width='640' height='480' controls><source src='" + videoUrl + "' type='video/mp4'>Your browser does not support the video tag.</video>";
                        Literal1.Text = videoPlayer;
                    }
                    catch (OracleException ex)
                    {
                        lbl_status.Text = ex.Message;
                        lbl_status.ForeColor = System.Drawing.Color.Red;
                    }
                    catch (Exception ex)
                    {
                        lbl_status.Text = ex.Message;
                        lbl_status.ForeColor = System.Drawing.Color.Red;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                lbl_status.Text = "Video inexistent!!!";
                lbl_status.ForeColor = System.Drawing.Color.Red;
                return;
            }
        }


    }
}