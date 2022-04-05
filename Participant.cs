using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using TryDapper;

namespace ClientLourdP1
{
    public class Participant
    {
        public Participant() {}
        public int ID { get; set; }
        public String Nom { get; set; }
        public String Prenom { get; set; }
        public String Email { get; set; }

        public String GetParticipant()
        {
            return "\n" + Nom.ToUpper() + "\n" + Prenom + "\n" + Email;
        }
    }
}
