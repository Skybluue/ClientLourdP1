using MySql.Data.MySqlClient;
using System;
using TryDapper;

namespace ClientLourdP1
{
    class Program
    {
        static void Main()
        {
            DBConnection dbCon = new DBConnection
            {
                Server = "127.0.0.1",
                DatabaseName = "Sucrerie",
                UserName = "root",
                Password = ""
            };

                int choixUser;
                do
                {
                    choixUser = Interface.Afficher();
                    Interface.TraiterChoix(choixUser, dbCon);
                } while (choixUser != 3);
            
        }
    }
}
