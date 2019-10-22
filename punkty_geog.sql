--Tworzenie Tabeli do której wpisuję punkty
CREATE TABLE punkty_geom2
(
    geom geometry(PointZ) Not null,
    id serial Primary key
);

--Tworzenie Bliźniaczej Tabeli punkty_geog2
CREATE TABLE punkty_geog2 AS
SELECT
  Geography(ST_Transform(geom,4326)) AS geog,
  id
FROM punkty_geom2;
                         
--Tworzenie Bliźniaczej Tabeli punkty_geog3
 CREATE TABLE punkty_geog3 AS
SELECT
  geog AS geog,
  id
FROM punkty_geog2;                        
