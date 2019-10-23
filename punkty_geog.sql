--Tworzenie Tabeli do której wpisuję punkty
CREATE TABLE punkty_geom
(
    geom geometry(PointZ) Not null,
    id serial Primary key
);

--Tworzenie Bliźniaczej Tabeli punkty_geog
CREATE TABLE punkty_geog AS
SELECT
  Geography(ST_Transform(geom,4326)) AS geog,
  id
FROM punkty_geom;
                         
--Tworzenie Bliźniaczej Tabeli punkty_geog2
 CREATE TABLE punkty_geog2 AS
SELECT
  geog AS geog,
  id
FROM punkty_geog;                        
