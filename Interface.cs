using System;
using System.Collections.Generic;
using System.Text;
using TryDapper;
using QRCoder;
using MySql.Data.MySqlClient;
using System.IO;

namespace ClientLourdP1
{
    public static class Interface
    {
        public static int Afficher()
        {
            int choixUser;
            Console.WriteLine("1. Rechercher un Participant");
            Console.WriteLine("2. Ajouter un Participant");
            Console.WriteLine("3. Quitter");
            Console.WriteLine("Votre Choix : ");
            try
            {
                choixUser = int.Parse(Console.ReadLine());
                if (choixUser > 0 | choixUser < 4)
                    return choixUser;
                else
                    return 0;
            } catch
            {
                return 0;
            }
        }

        public static void TraiterChoix(int choixUser, DBConnection DataBaseConnection)
        {
            Console.Clear();
            switch (choixUser)
            {
                case 0:
                    Console.WriteLine("Vous ne pouvez entrer que 1, 2 ou 3.");
                    break;
                case 1:
                    RechercherParticipant(DataBaseConnection);
                    break;
                case 2:
                    AjouterParticipant(DataBaseConnection);
                    break;
                case 3:
                    Console.WriteLine("Au revoir !");
                    break;
            }
        }

        public static void AjouterParticipant(DBConnection DataBaseConnection)
        {
            Participant UnParticipant = new Participant();
            String NomParticipant, PrenomParticipant, EmailParticipant;
            Console.Clear();
            Console.WriteLine("Nom du participant : ");
            NomParticipant = Console.ReadLine();
            Console.WriteLine("Prénom du participant : ");
            PrenomParticipant = Console.ReadLine();
            Console.WriteLine("Email du participant : ");
            EmailParticipant = Console.ReadLine();
            UnParticipant.Nom = NomParticipant;
            UnParticipant.Prenom = PrenomParticipant;
            UnParticipant.Email = EmailParticipant;
            if (DataBaseConnection.IsConnect())
            {
                try
                {
                    String sqlString = "INSERT INTO participant(nom,prenom,email) VALUES(?nom,?prenom,?email)";
                    sqlString = Tools.PrepareLigne(sqlString, "?nom", Tools.PrepareChamp(UnParticipant.Nom, "Chaine"));
                    sqlString = Tools.PrepareLigne(sqlString, "?prenom", Tools.PrepareChamp(UnParticipant.Prenom, "Chaine"));
                    sqlString = Tools.PrepareLigne(sqlString, "?email", Tools.PrepareChamp(UnParticipant.Email, "Chaine"));
                    var cmd = new MySqlCommand(sqlString, DataBaseConnection.Connection);
                    cmd.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    Console.Write("Erreur N° " + ex.Number + " : " + ex.Message);
                }
                Console.WriteLine(UnParticipant.GetParticipant() + "\n");
            }
        }

        public static void FabriquerBadge(Participant UnParticipant)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(UnParticipant.ID.ToString(), QRCodeGenerator.ECCLevel.Q);

            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20);

            StreamWriter monStreamWriter = new StreamWriter(@"BadgeSalon.html");

            String strImage = "<img src=\"data:image/png;base64," + qrCodeImageAsBase64 + "\">";
            monStreamWriter.WriteLine("<html>");
            monStreamWriter.WriteLine("<body>");
            string temptext = "<P>" + UnParticipant.Nom + "<P>";
            monStreamWriter.WriteLine(temptext);
            temptext = "<P>" + UnParticipant.Prenom + "<P>";
            monStreamWriter.WriteLine(temptext);
            monStreamWriter.WriteLine(strImage);
            monStreamWriter.WriteLine("<body>");
            monStreamWriter.WriteLine("<html>");

            monStreamWriter.Close();
            Console.WriteLine("Le QrCode est généré. Appuyer sur une touche pour continuer");
            Console.ReadKey();
        }

        public static void RechercherParticipant(DBConnection DataBaseConnection)
        {
            String NomParticipant;
            Console.Clear();
            Console.WriteLine(".....................................................");
            Console.Write("...................Nom du participant recherché ?");
            NomParticipant = Console.ReadLine();

            string query = "select id,nom,prenom,email from participant where nom =?nom;";
            query = Tools.PrepareLigne(query, "?nom", Tools.PrepareChamp(NomParticipant, "Chaine"));

            if (DataBaseConnection.IsConnect())
            {
                var cmd = new MySqlCommand(query, DataBaseConnection.Connection);
                List<Participant> LesParticipantsTrouves = new List<Participant>();
                MySqlDataReader TheReader = cmd.ExecuteReader(); //On est arrivé à la fin, il faut recharger le reader
                while (TheReader.Read())
                {
                    Participant UnParticipant = new Participant
                    {
                        ID = (int)TheReader["id"],
                        Nom = (string)TheReader["nom"],
                        Prenom = (string)TheReader["prenom"],
                        Email = (string)TheReader["email"]

                    };
                    LesParticipantsTrouves.Add(UnParticipant);
                }
                if (LesParticipantsTrouves.Count > 0)
                {
                    Console.WriteLine("--------------------Participants Trouvés------------------");
                    foreach (Participant UnParticipant in LesParticipantsTrouves)
                        Console.WriteLine(UnParticipant.ID.ToString() + ", " + UnParticipant.Prenom + ", " + UnParticipant.Nom + ", " + UnParticipant.Email);
                    Console.WriteLine("Saisissez l'ID : ");
                    int IDtoImpr = int.Parse(Console.ReadLine());
                    int index = LesParticipantsTrouves.FindIndex(requete => requete.ID == IDtoImpr);
                    if (index > -1)
                    {
                        Participant LeParticipantAImprimer = LesParticipantsTrouves[index];
                        FabriquerBadge(LeParticipantAImprimer);
                    }
                }
                else
                    Console.WriteLine("Je n'ai trouvé personne.");
                Console.WriteLine("Appuyer sur une touche pour continuer");
                Console.ReadKey();
                TheReader.Close();
            }
        }
    }
}
