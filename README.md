W tej gałęzi sukcesywnie będę dodawał moje pliki
  -- Opis dotychczasowego Działania:
      - Zapytanie SQL tworzy Tabelę punkty_geom do której można wpisywać punkty o trzech współrzędnych w wariancie geometrycznym
      - program.cs dodaje w C# losowe współrzędne do punktów w 3D
  -- TO DO:
      - znalezienie bazy wojewówdztw
      - zwrócenie w konsoli w C# czy punkt znajduje się w obrębie województwa za pomocą ST_Within(punkty_geom,województwa)
      - spawdzenie dystansu za pomocą ST_Distance czy jest mniejszy  niz 30 000m
      - Ewentualne przypisanie danych geometrycznych do typu geograficznego przez stworzenie tabeli 
          CREATE TABLE punkty_geog AS
          SELECT
            Geography(ST_Transform(geom,4326)) AS geog,
            id
          FROM punkty_geom;
