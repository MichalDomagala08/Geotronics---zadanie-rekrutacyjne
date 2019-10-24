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



             //     ************************************************************************************************************************
            //      *                                          ******UWAGA!******                                                          *
            //      * Zanim będzie się korzystać z funkcji sprawdzających odległość trzeba stworzyć za pomocą funkcji                      *
            //      * Create_geog_Tables(conn) tabele geometryczne                                                                         *
            //      * Jesli funkcja mimo to nie działa trzeba sprawdzić czy w Tabeli punkty_geom są rzeczywiście zapisane jakieś rekordy   *
            //      ************************************************************************************************************************





            conn.Open();
           AddRandomSpatialPoints(conn);
            isWithinAllWojewodztwas(conn);
           // Is30kmFrom(conn);
           // Console.WriteLine(IsAll30kmFrom(conn));
            //Delete_records_from_table(conn);
            //Create_geog_Tables(conn);
            //Drop_geog_tables(conn);
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
                var cmd = new NpgsqlCommand("insert into punkty_geom(geom) " +
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
        static void IsWithinWojewodztwo(NpgsqlConnection connection)
        {
            //Reading Wojewodztwo from keyboard
                Console.WriteLine("Proszę Wpisać nazwę Województwa bez Polskich znaków");
                string d = Console.ReadLine();
                Console.WriteLine();

            //Querry with already red Wojewodztwo
                NpgsqlCommand cmd2 = new NpgsqlCommand("SELECT ST_X(punkty_geom.geom),ST_Y(punkty_geom.geom) ,\"Województwa\".jpt_nazwa_ as woj " +
                "FROM \"Województwa\" join punkty_geom on ST_Within(punkty_geom.geom, ST_SetSRID(\"Województwa\".geom, 4326)) " +
                "Where \"Województwa\".jpt_nazwa_ = @W", connection);
                cmd2.Parameters.AddWithValue("W", d);

            //Writing all Points in Selected Wojewodztwo
            NpgsqlDataReader dr = cmd2.ExecuteReader();
            Console.WriteLine("ID:      X             Y            WOJ    ");
            while (dr.Read())
                Console.WriteLine("{0}     {1}    {2}    {3}    ", dr[0], dr[1], dr[2], dr[3]);

        }


            //      ************************************************************************************************************************
            //      *                                          ******UWAGA!******                                                          *
            //      * Zanim będzie się korzystać z funkcji sprawdzających odległość trzeba stworzyć za pomocą funkcji                      *
            //      * Create_geog_Tables(conn) tabele geometryczne                                                                         *
            //      * Jesli funkcja mimo to nie działa trzeba sprawdzić czy w Tabeli punkty_geom są rzeczywiście zapisane jakieś rekordy   *
            //      ************************************************************************************************************************




        //Czy Dany punkt jest 30km od wszystkich
        static void Is30kmFrom(NpgsqlConnection connection/*,int ID*/)
        {
            //Uzyskanie ID Punktu 
                Console.WriteLine("Podaj ID punktu do sprawdzenia");
                var z = Console.ReadLine();
                int x = Int32.Parse(z);

            //Zapytanie oraz wczytanie ID
                NpgsqlCommand cmd3 = new NpgsqlCommand("SELECT  punkty_geog2.id as \" point id\", (ST_Distance(punkty_geog.geog, punkty_geog2.geog) <= 30000) " +
                "as \"Is Within 30 km\" FROM punkty_geog JOIN punkty_geog2 ON ST_Area(punkty_geog2.geog) <= 0 where punkty_geog.id = @D ", connection);
                cmd3.Parameters.AddWithValue("D",x);

            // Wypisanie Punktów
                NpgsqlDataReader dr = cmd3.ExecuteReader();
                Console.WriteLine("PointId     Is it within 30 km?    ");
                while (dr.Read())
                    Console.WriteLine("{0}             {1}    ", dr[0], dr[1]);

        }
        // Czy wszystkie punkty są 30km od Siebie? UWAGA! Mało prawdopodobne że przy niecelowo wpisanych punktach kiedykolwiek zwróci True
        static string IsAll30kmFrom(NpgsqlConnection connection/*,int ID*/)
        {
            //licznik punktów
                var cmd4 = new NpgsqlCommand("Select COUNT(*) FROM punkty_geog",connection);
            //Zwracanie Czy dany punkt jest w odl. 30 km od sebie
                var cmd5 = new NpgsqlCommand("SELECT  punkty_geog2.id as \" point id\", (ST_Distance(punkty_geog.geog, punkty_geog2.geog) <= 30000)" +
                " as \"Is Within 30 km\" FROM punkty_geog JOIN punkty_geog2 ON ST_Area(punkty_geog2.geog) <= 0 where punkty_geog.id = @D ", connection);

            //Odzczytanie Licznika oraz przypisanie go do int i
                NpgsqlDataReader dr2 = cmd4.ExecuteReader();
                dr2.Read();
                int i = Convert.ToInt32(dr2[0]);
                dr2.Close();
                dr2.Dispose();

            //Pętla iterująca przez wszystkie punkty, zwracająca TRUE - tylko jeśli któryś punkt nie jest FALSE
                cmd5.Parameters.AddWithValue("D", 1);
                NpgsqlDataReader dr = cmd5.ExecuteReader();
            
                for(int n = 0; n <i+1;n++)
                {
                    while (dr.Read())
                    {
                        cmd5.Parameters.AddWithValue("D",n+1);
                        if (dr[1].ToString() == "True")
                        {
                            
                        }
                        else
                        {
                            return false.ToString();
                        }
                    }
                };
                return true.ToString();
        }
        static void Create_geog_Tables(NpgsqlConnection connection)
        {
            //tworzenie dwóch Tabel, gdzie przechowywane będą identyczne wartości jak w przypadku punkty_geom, ale w formacie Geograficznym
                var cmd6 = new NpgsqlCommand("CREATE TABLE punkty_geog AS SELECT Geography(ST_Transform(geom,4326)) AS geog, id FROM punkty_geom", connection);
                cmd6.ExecuteNonQuery();
                var cmd7 = new NpgsqlCommand("CREATE TABLE punkty_geog2 AS SELECT geog AS geog, id FROM punkty_geog", connection);
                cmd7.ExecuteNonQuery();
        }
        static void Drop_geog_tables(NpgsqlConnection connection)
        {
            //Usuwanie Tabel Geograficznych
                var cmd8 = new NpgsqlCommand("DROP TABLE punkty_geog", connection);
                cmd8.ExecuteNonQuery();
                var cmd9 = new NpgsqlCommand("DROP TABLE punkty_geog2", connection);
                cmd9.ExecuteNonQuery();
        }
        static void Delete_records_from_table(NpgsqlConnection connection)
        {
            //Usuwanie Rekordów z wybranej tabeli
                Console.WriteLine("Wpisz nazwę Tabeli którą chcesz wyczyścić (punkty_geom,punkty_geog,punkty_geog2)");
                string x = Console.ReadLine();
                var cmd10 = new NpgsqlCommand("DELETE FROM @Tabela", connection);
                cmd10.Parameters.AddWithValue("Tabela", x);
                cmd10.ExecuteNonQuery();
        }
        static void manually_insert(NpgsqlConnection connection)
        {
            //Dodawanie manualne rekordów do Tabel Geograficznych (prawdopodobnie bedzie działało też dla tabeli Geometrycznej
                Console.WriteLine("Wpisz dziesiętne współrzędne E z dokładnością 5 miejsc po kropce");
                string x = Console.ReadLine();
                Console.WriteLine("Wpisz dziesiętne współrzędne N  z dokładnością 5 miejsc po kropce");
                string y = Console.ReadLine();
                Console.WriteLine("Wpisz Wysokość w Metrach");
                string z = Console.ReadLine();
                Console.WriteLine("Wpisz nazwę Tabeli(punkty_geog,punkty_geog2, punkty_geom)");
                string w = Console.ReadLine();

                var cmd11 = new NpgsqlCommand("insert into @Tabela VALUES (\'POINTZ(cast(@X as float) cast(@Y as float) @Z)\')", connection);
                cmd11.Parameters.AddWithValue("Tabela", w);
                cmd11.Parameters.AddWithValue("X", x);
                cmd11.Parameters.AddWithValue("Y", y);
                cmd11.Parameters.AddWithValue("Z", z);
                cmd11.ExecuteNonQuery();
        }
        static void isWithinAllWojewodztwas(NpgsqlConnection connection)
        {
            //Zapytanie do wszytskich województw
                NpgsqlCommand cmd12 = new NpgsqlCommand("SELECT punkty_geom.id, ST_X(punkty_geom.geom),ST_Y(punkty_geom.geom) ,\"Województwa\".jpt_nazwa_ as woj " +
                 "FROM \"Województwa\" join punkty_geom on ST_Within(punkty_geom.geom, ST_SetSRID(\"Województwa\".geom, 4326)) ORDER BY \"Województwa\".geom", connection);

            //Wypisanie wszystkich województw
                NpgsqlDataReader dr = cmd12.ExecuteReader();
                Console.WriteLine("ID:      X             Y            WOJ    ");
                while (dr.Read())
                    Console.WriteLine("{0}     {1}    {2}    {3}    ",dr[0], dr[1], dr[2], dr[3]);

        }
    }

}

