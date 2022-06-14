using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PPE_Salons
{
    public partial class AdminMenu : Form
    {
        public AdminMenu()
        {
            InitializeComponent();
            loadListeParticipants();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUser adminPage = new AddUser();
            adminPage.ShowDialog();
        }

        private void loadListeParticipants()
        {
            DBConnection dbCon = new DBConnection();
            dbCon.Server = "127.0.0.1";
            dbCon.DatabaseName = "ppe_client_lourd";
            dbCon.UserName = "root";
            dbCon.Password = "";//Crypto.Decrypt("MGgAtv/61oXwMgJN47ilHg==");//Pour éviter d'afficher le mot de passe en clair dans le code
            if (dbCon.IsConnect())
            {
                string query = "SELECT Id, Nom, Prenom, Email FROM contact ORDER BY Nom";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = cmd.ExecuteReader();//Remplissage du curseur
                List<Contact> contacts = new List<Contact>();
                while (reader.Read())
                {
                    Contact contact = new Contact
                    {
                        Id = (int)reader["Id"],
                        Nom = (string)reader["Nom"],
                        Prenom = (string)reader["Prenom"],
                        Email = (string)reader["Email"],

                    };
                    contacts.Add(contact);
                }

                MaGrid.DataSource = null;
                MaGrid.DataSource = contacts;
                FormaterListe();
                reader.Close();
                dbCon.Close();
                MaGrid.Visible = true;

            }
        }

        private void FormaterListe()
        {
            MaGrid.Columns["Id"].Visible = false;
            MaGrid.Columns["Nom"].HeaderText = "Nom du participant";
            MaGrid.Columns["Nom"].Width = 150;
            MaGrid.MultiSelect = false;
            MaGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MaGrid.ReadOnly = true;
        }
    }
}
