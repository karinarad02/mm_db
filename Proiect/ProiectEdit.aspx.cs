using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proiect
{
    public partial class ProiectEdit : System.Web.UI.Page
    {
        OracleConnection con;
        private Dictionary<int, string> artistiDict = new Dictionary<int, string>();
        private List<Tablou> tablouriList= new List<Tablou>();

        // Variabile private pentru a stoca datele încărcate
        private string titluDefault;
        private int artistDefault;
        private byte[] imagineDefault;

        // Constructorul pentru inițializarea conexiunii Oracle
        private void InitializeConnection()
        {
            string cons = "User ID=STUD_RADITOIUK; Password=student; Data Source=(DESCRIPTION=" +
                          "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=37.120.249.41)(PORT=1521)))" +
                          "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcls)));";
            con = new OracleConnection(cons);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeConnection(); // Inițializare conexiune

            if (!IsPostBack)
            {
                LoadArtisti(); // Load artists into the dictionary and dropdown
                if (Request.QueryString["idTablou"] != null)
                {
                    int idTablou = Convert.ToInt32(Request.QueryString["idTablou"]);
                    LoadTablouData(idTablou);
                }
                else
                {
                    lbl_status.Text = "ID Tablou invalid!";
                    lbl_status.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                if (ViewState["imagineDefault"] != null)
                {
                    imagineDefault = (byte[])ViewState["imagineDefault"];
                }
            }
        }

        // Încărcarea artiștilor în dropdown și în dicționar
        private void LoadArtisti()
        {
            try
            {
                // Check if the artists are cached in Application (or Session)
                if (Application["ArtistiDict"] != null)
                {
                    artistiDict = (Dictionary<int, string>)Application["ArtistiDict"];
                    drop_artisti_i.Items.Clear();
                    drop_artisti_i.Items.Add(new ListItem("--Select Artist--", ""));

                    foreach (var artist in artistiDict)
                    {
                        drop_artisti_i.Items.Add(new ListItem(artist.Value, artist.Key.ToString()));
                    }
                }
                else
                {
                    // If not cached, fetch from the database
                    con.Open();

                    string query = "SELECT id_artist, nume FROM ARTISTI_PROIECT";
                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            drop_artisti_i.Items.Clear();
                            drop_artisti_i.Items.Add(new ListItem("--Select Artist--", ""));
                            artistiDict.Clear();

                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["id_artist"]);
                                string nume = reader["nume"].ToString();

                                artistiDict[id] = nume;
                                drop_artisti_i.Items.Add(new ListItem(nume, id.ToString()));
                            }

                            // Store the artists in the cache (Application or Session)
                            Application["ArtistiDict"] = artistiDict; // Cache the dictionary for the entire application
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                lbl_status.Text = "Eroare la încărcarea artiștilor: " + ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                con.Close();
            }
        }


        // Încărcarea datelor tabloului pe baza ID-ului
        private void LoadTablouData(int idTablou)
        {
            try
            {
                con.Open();

                string query = @"
                                SELECT 
                                    t.id_tablou,
                                    t.titlu,
                                    t.id_artist, 
                                    image_to_blob(t.img) AS imagine
                                FROM 
                                    TABLOURI_PROIECT t
                                WHERE 
                                    t.id_tablou = :idTablou";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add(new OracleParameter(":idTablou", idTablou));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            titluDefault = reader["titlu"].ToString(); // Set default value for title
                            artistDefault = Convert.ToInt32(reader["id_artist"]); // Set default value for artist
                            imagineDefault = reader["imagine"] as byte[]; // Set default value for image

                            ViewState["imagineDefault"] = imagineDefault;

                            tb_titlu.Text = titluDefault; // Set the text box with the default title
                            lbl_status.Text = "Datele au fost încărcate cu succes!";
                            lbl_status.ForeColor = System.Drawing.Color.Green;

                            if (artistiDict.ContainsKey(artistDefault))
                            {
                                drop_artisti_i.SelectedValue = artistDefault.ToString(); // Set the default artist
                            }

                            if (imagineDefault != null)
                            {
                                Image1.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(imagineDefault);
                            }

                        }
                        else
                        {
                            lbl_status.Text = "Tablou nu a fost găsit!";
                            lbl_status.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lbl_status.Text = "Eroare la conectarea la baza de date: " + ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                con.Close(); // Închidem conexiunea
            }
        }

        // Apelul pentru a actualiza datele tabloului
        protected void btn_update_Click(object sender, EventArgs e)
        {
            int idTablou = Convert.ToInt32(Request.QueryString["idTablou"]);
            string titlu = string.IsNullOrEmpty(tb_titlu.Text) ? titluDefault : tb_titlu.Text;
            int artist = string.IsNullOrEmpty(drop_artisti_i.SelectedValue) ? artistDefault : Convert.ToInt32(drop_artisti_i.SelectedValue);

            byte[] imgBytes = null;
            imagineDefault = (byte[])ViewState["imagineDefault"];
            // Check if a new image is uploaded
            if (FileUpload1.HasFile)
            {
                using (Stream fs = FileUpload1.PostedFile.InputStream)
                using (BinaryReader br = new BinaryReader(fs))
                {
                    imgBytes = br.ReadBytes((int)fs.Length); // Read the uploaded image as byte array
                }
            }
            else
            {
                if (imagineDefault != null)
                {
                    imgBytes = new byte[imagineDefault.Length];
                    Array.Copy(imagineDefault, imgBytes, imagineDefault.Length); // Manual copy
                }
                else
                {
                    imgBytes = new byte[0]; // Default to an empty byte array if no image is provided
                }
            }

            try
            {
                con.Open();

                // Call the stored procedure for UPDATE
                using (OracleCommand cmd = new OracleCommand("upd_tablou_proiect", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters for the UPDATE procedure
                    cmd.Parameters.Add(new OracleParameter("vid_tablou", idTablou)); // ID Tablou
                    cmd.Parameters.Add(new OracleParameter("vtitle", titlu));         // Titlu
                    cmd.Parameters.Add(new OracleParameter("vid_artist", artist));     // ID Artist

                    OracleParameter imgParam = new OracleParameter("fis", OracleDbType.Blob);
                    imgParam.Value = imgBytes;  // Set the image (either the uploaded one or the default)
                    cmd.Parameters.Add(imgParam);

                    // Execute the procedure
                    cmd.ExecuteNonQuery();
                }

                artistiDict = (Dictionary<int, string>)Application["ArtistiDict"];
                tablouriList = (List<Tablou>)Application["TablouriList"];
                if (tablouriList != null)
                {
                    var tableauToUpdate = tablouriList.Find(t => t.IdTablou == idTablou);
                    if (tableauToUpdate != null)
                    {
                        
                        tableauToUpdate.Titlu = titlu;
                        tableauToUpdate.Artist = artistiDict[artist];
                        tableauToUpdate.ImagineBase64 = Convert.ToBase64String(imgBytes);
                    }
                    Application["TablouriList"] = tablouriList;
                }
            }
            catch (Exception ex)
            {
                lbl_status.Text = "Eroare la actualizare: " + ex.Message;
                lbl_status.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                con.Close(); // Close the connection
            }

            Response.Redirect("Proiect.aspx",false);
        }
    }
}
