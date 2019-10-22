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
            Is30kmFrom(conn/*,1*/);
            // IsWithinWojewodztwo(conn, "swietokrzyskie");
            // AddRandomSpatialPoints(conn);
            conn.Close();

        }
        // DODAWANIE LOSOWYCH PUNKTÓW 3D W OBRĘBIE GRANIC POLSKI
        static void AddRandomSpatialPoints(NpgsqlConnection connection)
        {
            try
            {
             //***********PIERWSZE ROZWIĄZANIE - GENEROWANIE PUNKTÓW W C#*************
             //W razie potrzeby są inne mozliwośi osiągnięcia podobnych resultatów jednak ta wydawała się póki co najbardziej wydajna pod wzgledem długości kodu
             //oraz złożoności.

                //Adding Point SQL command and random
                var cmd = new NpgsqlCommand("insert into punkty_geom2(geom) " +
                    "SELECT ST_SetSRID(ST_MakePoint(cast(@X as float)/100000, cast(@Y as float)/100000,@Z), 4326) " +
                    "where (ST_Contains((SELECT ST_SetSRID(geom,4326) FROM \"Państwo\"), " +
                    "ST_SetSRID(ST_Point((CAST(@X as NUMERIC)/CAST(100000 as NUMERIC)), " +
                    "(CAST(@Y as numeric)/CAST(100000 as NUMERIC))),4326)) = TRUE)", connection);

                //preparing random numbers
                    Random rand2 = new Random();
                    int EX = rand2.Next(1412298, 2414585);
                    double EXD = Convert.ToDouble(EX);
                    int NY = rand2.Next(4900238, 5483250);
                    double NYD = Convert.ToDouble(NY);

                //Adding Parameters with random Values
                    cmd.Parameters.AddWithValue("X", (EX) );
                    cmd.Parameters.AddWithValue("Y", (NY) );
                    cmd.Parameters.AddWithValue("Z", (rand2.Next(0, 3000)));
                    cmd.ExecuteNonQuery();

                //Checking witch point"aspire" to be added
                    Console.WriteLine("Inserted Point: ");
                    Console.WriteLine("N: {0} E: {1} H: {2}", (NYD / 100000), (EXD / 100000), (rand2.Next(0, 3000)));
                    Console.ReadLine();

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Czy w pdanym województwe (jeśli w ogóle) jest  punkt, oraz który to punkt
        static void IsWithinWojewodztwo(NpgsqlConnection connection, string wojewodztwo)
        {
            
            Console.WriteLine("Proszę Wpisać nazwę Województwa bez Polskich znaków");
            string d = Console.ReadLine();
            Console.WriteLine();
            NpgsqlCommand cmd3 = new NpgsqlCommand("SELECT ST_X(punkty_geom2.geom),ST_Y(punkty_geom2.geom) ,\"Województwa\".jpt_nazwa_ as woj FROM \"Województwa\" join punkty_geom2 on ST_Within(punkty_geom2.geom, ST_SetSRID(\"Województwa\".geom, 4326)) Where \"Województwa\".jpt_nazwa_ = @W", connection);
            cmd3.Parameters.AddWithValue("W", d);
            NpgsqlDataReader dr = cmd3.ExecuteReader();
            Console.WriteLine(" X             Y            WOJ    ");
            while (dr.Read())
                Console.WriteLine("{0}    {1}    {2}    ", dr[0], dr[1], dr[2]);
        }
        //Czy Punkt o podanym ID punkt jest 30km od wszystkich
        static void Is30kmFrom(NpgsqlConnection connection/*,int ID*/)
        {

            Console.WriteLine("Podaj ID punktu do sprawdzenia");
            var z = Console.ReadLine();

            int x = Int32.Parse(z);


            NpgsqlCommand cmd5 = new NpgsqlCommand("SELECT  punkty_geog3.id as \" point id\", (ST_Distance(punkty_geog2.geog, punkty_geog3.geog) <= 30000) as \"Is Within 30 km\" FROM punkty_geog2 JOIN punkty_geog3 ON ST_Area(punkty_geog3.geog) <= 0 where punkty_geog2.id = @D ", connection);
            cmd5.Parameters.AddWithValue("D",x);
            NpgsqlDataReader dr = cmd5.ExecuteReader();
            Console.WriteLine("PointId     Is it within 30 km?    ");
            while (dr.Read())
                Console.WriteLine("{0}             {1}    ", dr[0], dr[1]);

        }
    }

}

