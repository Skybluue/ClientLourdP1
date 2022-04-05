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
            Console.WriteLine("3. Générer un badge");
            Console.WriteLine("4. Quitter");
            Console.WriteLine("Votre Choix : ");
            try
            {
                choixUser = int.Parse(Console.ReadLine());
                if (choixUser > 0 | choixUser < 5)
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
                    break;
                case 2:
                    AjouterParticipant(DataBaseConnection);
                    break;
                case 3:
                    break;
                case 4:
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

        public static void FabriquerBadge(int TheparticipantID, String UnNom, String UnPrenom)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(TheparticipantID.ToString(), QRCodeGenerator.ECCLevel.Q);

            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20);

            StreamWriter monStreamWriter = new StreamWriter(@"BadgeSalon.html");

            String strImage = "<img src=\"data:image/png;base64," + qrCodeImageAsBase64 + "\">";
            monStreamWriter.WriteLine("<html>");
            monStreamWriter.WriteLine("<body>");
            string temptext = "<P>" + UnNom + "<P>";
            monStreamWriter.WriteLine(temptext);
            temptext = "<P>" + UnPrenom + "<P>";
            monStreamWriter.WriteLine(temptext);
            monStreamWriter.WriteLine(strImage);
            monStreamWriter.WriteLine("<body>");
            monStreamWriter.WriteLine("<html>");

            monStreamWriter.Close();
            Console.WriteLine("Le QrCode est généré. Appuyer sur une touche pour continuer");
            Console.ReadKey();
        }
    }
}
