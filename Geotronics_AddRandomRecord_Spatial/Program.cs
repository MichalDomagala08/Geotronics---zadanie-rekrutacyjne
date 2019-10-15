using System;
using Npgsql;


namespace Geotronics_AddRandomRecord_Spatial
{
    class Program
    {
        static void Main(string[] args)
        {



            // ***********Specify Connection Options And Open An Connection*********
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=postgres;" +
                                    "Password=Qetchua08!;Database=Geotronics_Domagala;");


            conn.Open();

            try
            {

                double RandomDouble(double min, double max, Random rands)
                {

                    return rands.NextDouble() * (max - min) - min;
                }

                NpgsqlCommand cmd2 = new NpgsqlCommand("INSERT INTO punkty_geog(points) VALUES ( ST_SetSRID(ST_MakePoint(@X, @Y), 4326))", conn);
                Random rand2 = new Random();

                cmd2.Parameters.AddWithValue("X", (RandomDouble(0.0, 20.0, rand2)));
                cmd2.Parameters.AddWithValue("Y", (RandomDouble(0.0, 20.0, rand2)));

                cmd2.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex);
            }

            conn.Close();






        }
    }
}

