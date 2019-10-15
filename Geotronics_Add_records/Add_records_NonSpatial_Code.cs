using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Geotronics_Add_records
{
    class Program
    {
        static void Main(string[] args)
        {

            // ***********Specify Connection Options And Open An Connection*********
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=postgres;" +
                                    "Password=Qetchua08!;Database=Geotronics_Domagala;");
            conn.Open();

            //*******ALTERING TABLE********
            /*
            try
            {
                NpgsqlCommand cmd3 = new NpgsqlCommand("ALTER TABLE twopoints ADD ID SERIAL", conn);
                cmd3.ExecuteNonQuery();
            }
            catch(NpgsqlException ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
                */
            //**********ADDING RANDOM RECORDS********

            double RandomDouble(double min, double max, Random rands)
            {

                return rands.NextDouble() * (max - min) - min;
            }

            NpgsqlCommand cmd2 = new NpgsqlCommand("INSERT INTO punkty(x,y,z) VALUES ( @X, @Y,@Z)", conn);
            Random rand2 = new Random();

            cmd2.Parameters.AddWithValue("X", RandomDouble(0.0, 20.0, rand2));
            cmd2.Parameters.AddWithValue("Y", RandomDouble(0.0, 20.0, rand2));
            cmd2.Parameters.AddWithValue("Z", RandomDouble(0.0, 20.0, rand2));

            cmd2.ExecuteNonQuery();

            //*******DELETING RECORDS********
            /*   
                   try
                   {
                       NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " +
                           "twopoints", conn);
                       int recordAffected = command.ExecuteNonQuery();
                       if (Convert.ToBoolean(recordAffected))
                       {
                           Console.WriteLine("Data successfully deleted!");
                           Console.ReadLine();
                       }
                   }
                   catch (NpgsqlException ex)
                   {
                       Console.WriteLine(ex);
                   }

       */
            // ******READING TABLE*********
            NpgsqlCommand cmd = new NpgsqlCommand("select* from punkty", conn);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
                Console.Write("{0}  {1}   {2}    ID:{3}\n", dr[0], dr[1], dr[2], dr[3]);
            Console.ReadLine();


            // ******Close connection********
            conn.Close();

        }
    }
}
