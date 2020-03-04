DROP DATABASE if exists BEST
GO

Create Database BEST
go

USE BEST

/* Budowanie Tabel */
create table Klienci(
ID_Klienta int IDENTITY(1,1) PRIMARY KEY,
Imie NVARCHAR(100) not null,
Nazwisko nvarchar(100) not null,
Data_Dolaczenia date null,
Plec NVARCHAR(30) not null,
Miasto NVARCHAR(100) not null,
Ulica NVARCHAR(100) not null,
Numer_Lokalu NVARCHAR(20) not null,
Numer_Mieszkania smallint,
Email NVARCHAR(80) not null,
Numer_Telefonu VARCHAR(15) not null
)

create table Pracownicy(
ID_Pracownika int IDENTITY(1,1) PRIMARY KEY,
Imie NVARCHAR(100) not null,
Nazwisko nvarchar(100) not null,
Plec NVARCHAR(30) not null,
Email NVARCHAR(80) not null,
Numer_Telefonu VARCHAR(15) not null,
Rola NVARCHAR(70) not null,
Login NVARCHAR(70) not null,
Haslo NVARCHAR(70) not null,
Uprawnienia_Systemowe smallint not null
)

create table Karnety(
ID_Karnetu int IDENTITY(1,1) PRIMARY KEY,
Nazwa NVARCHAR(100) not null,
Cena DECIMAL(10,2) not null,
Uprawnienia smallint not null,
Ilosc_Miesiecy int not null,
Opis NVARCHAR(100)
)

/* U¿ywam wzoru na identyfikator tabel lacznych ID_XY(Gdzie X to pierwsza litera pierwszego a Y to pierwsza litera drugiego) */
create table Karnet_Klient(
ID_KK int IDENTITY(1,1) PRIMARY KEY,
ID_Karnetu int FOREIGN KEY REFERENCES Karnety(ID_Karnetu)
ON DELETE CASCADE,
ID_Klienta int FOREIGN KEY REFERENCES Klienci(ID_Klienta)
ON DELETE CASCADE,
Data_Zakupu Date null,
Data_Konca Date null,
)

create table Atrakcje(
ID_Atrakcji int IDENTITY(1,1) PRIMARY KEY,
Nazwa Nvarchar(100) not null,
Uprawnienia smallint not null
)

create table Trenerzy_Certyfikaty(
ID_TC int IDENTITY(1,1) PRIMARY KEY,
ID_Pracownika int FOREIGN KEY REFERENCES Pracownicy(ID_Pracownika)
ON DELETE CASCADE,
Nazwa_Certyfikatu NVARCHAR(100) not null
)

create table Trenerzy_Atrakcji(
ID_TA int IDENTITY(1,1) PRIMARY KEY,
ID_Pracownika int FOREIGN KEY REFERENCES Pracownicy(ID_Pracownika)
ON DELETE CASCADE,
ID_Atrakcji int FOREIGN KEY REFERENCES Atrakcje(ID_Atrakcji)
ON DELETE CASCADE
)

/* Koniec Sekcji z Budowaniem Tabel */

/* Sekcja z budowaniem procedur sk³adowanych */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE CzyKlientIstnieje
@ID_KLienta as INT,
@result as INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM Klienci WHERE ID_Klienta = @ID_KLienta)
	BEGIN
		SET @result = -1;
		return
	END

	SET @result = 1;
END
GO




Create PROCEDURE CzyKlientAtrakcja
	@numer_klienta as INT ,
	@atrakcja as INT,
	@result as INT OUTPUT
AS
BEGIN
	DECLARE @NumerUprawnienie as INT

	SET NOCOUNT ON;
	DECLARE @date as Date;
	SET @date = GETDATE();

	IF NOT EXISTS(SELECT * FROM Klienci WHERE ID_Klienta = @numer_klienta)
		BEGIN
			SET @result = -1;
		END
	ELSE
	BEGIN
		IF EXISTS(SELECT * FROM Karnet_Klient WHERE Data_Zakupu <= @date and Data_Konca >= @date and ID_Klienta = @numer_klienta)
			BEGIN
				SET @NumerUprawnienie  = (SELECT Uprawnienia FROM Atrakcje WHERE ID_Atrakcji = @atrakcja)
				IF EXISTS(SELECT k.Uprawnienia FROM Karnet_Klient kk join Karnety k on k.ID_Karnetu=kk.ID_Karnetu 
				 WHERE kk.Data_Zakupu <= @date and kk.Data_Konca >= @date and kk.ID_Klienta = @numer_klienta and k.Uprawnienia = @NumerUprawnienie)
					SET @result = 1
				 ELSE
					SET @result = 0
			END
		ELSE
			SET @result = 0;

	END

END
GO

Create PROCEDURE CzyKlientMaAktywnyKarnet
	@numer_klienta as INT ,
	@result as INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @date as Date;
	SET @date = GETDATE();

	IF NOT EXISTS(SELECT * FROM Klienci WHERE ID_Klienta = @numer_klienta)
		BEGIN
			SET @result = -1;
		END
	ELSE
	BEGIN
		IF EXISTS(SELECT * FROM Karnet_Klient WHERE Data_Zakupu <= @date and Data_Konca >= @date and ID_Klienta = @numer_klienta)
			BEGIN
				SET @result = 1;
			END
		ELSE
			SET @result = 0;

	END

END
GO



CREATE PROCEDURE CzyPracownikIstnieje
@ID_Pracownika as INT,
@result as INT OUTPUT,
@dane as NVARCHAR(100) OUTPUT
AS
BEGIN

	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM Pracownicy WHERE ID_Pracownika = @ID_Pracownika)
	BEGIN
		SET @result = -1;
		return
	END
	ELSE IF NOT EXISTS(SELECT * FROM Pracownicy WHERE ID_Pracownika = @ID_Pracownika and Rola like 'Trener')
	BEGIN
	SET @result = 0;
		return
	END
	SELECT @dane = CONCAT(Imie,' ',Nazwisko) FROM Pracownicy WHERE ID_Pracownika = @ID_Pracownika
	SET @result = 1;
END
GO


Create PROCEDURE DodawanieAtrakcji
@Nazwa as NVARCHAR(100),
@Uprawnienia as SMALLINT,
@result as INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM Atrakcje WHERE Nazwa like @Nazwa)
		SET @result= 0
	ELSE
	BEGIN 
		INSERT INTO Atrakcje values(@Nazwa,@Uprawnienia)
		SET @result= 1
	END
END
GO


Create PROCEDURE DodajCertyfikatDoTrenera
@ID_Trenera as INT,
@Nazwa as NVARCHAR(100),
@result as INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM Trenerzy_Certyfikaty WHERE ID_Pracownika = @ID_Trenera and Nazwa_Certyfikatu like @Nazwa)
		SET @result = 0 
	ELSE
	BEGIN 
		INSERT INTO Trenerzy_Certyfikaty values(@ID_Trenera,@Nazwa)
		SET @result= 1
	END
END
GO


Create PROCEDURE DodajKarnet
@Nazwa as NVARCHAR(100),
@Cena as DECIMAL(10,2),
@Uprawnienia as SMALLINT,
@Ilosc_Miesiecy as INT,
@Opis as NVARCHAR(100) = 'Brak',
@result as INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM Karnety WHERE Nazwa like @Nazwa)
		SET @result =  0
	ELSE
	BEGIN 
		INSERT INTO Karnety values(@Nazwa,@Cena,@Uprawnienia,@Ilosc_Miesiecy,@Opis)
		SEt @result =  1
	END
END
GO


Create PROCEDURE DodawanieKlienta
@Imie as NVARCHAR(70) ,
@Nazwisko as NVARCHAR(70) ,
@Plec as NVARCHAR(30) ,
@Miasto as NVARCHAR(100) ,
@Ulica as NVARCHAR(100) ,
@Numer_Lokalu as NVARCHAR(20) ,
@Numer_Mieszkania as SMALLINT ,
@Email as NVARCHAR(80),
@Numer_Telefonu as VARCHAR(15),
@result as INT OUTPUT
AS
BEGIN
	DECLARE @Data_Dolaczenia DATE
	SET @Data_Dolaczenia = CAST(GETDATE() as DATE)
	SET NOCOUNT ON;

	IF EXISTS(SELECT * FROM Klienci WHERE Imie like @Imie and Nazwisko = @Nazwisko  and Numer_Telefonu like @Numer_Telefonu)
		SET @result = 0;
	ELSE
	BEGIN 
		INSERT INTO Klienci values(@Imie,@Nazwisko,@Data_Dolaczenia,@Plec,@Miasto,@Ulica,@Numer_Lokalu,@Numer_Mieszkania,@Email,@Numer_Telefonu)
		SET @result = 1;
	END
END
GO


Create PROCEDURE DodawaniePracownika
@Imie as NVARCHAR(70) ,
@Nazwisko as NVARCHAR(70) ,
@Plec as NVARCHAR(30) ,
@Email as NVARCHAR(80),
@Numer_Telefonu as VARCHAR(15),
@Rola as NVARCHAR(70),
@Login as NVARCHAR(70),
@Haslo as NVARCHAR(70),
@Uprawnienia_systemowe as SMALLINT,
@result as INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
IF EXISTS(SELECT * FROM Pracownicy WHERE Login like @Login and Haslo like @Haslo)
		SET @result =  0
	ELSE
	BEGIN 
		INSERT INTO Pracownicy values(@Imie,@Nazwisko,@Plec,@Email,@Numer_Telefonu,@Rola,@Login,@Haslo,@Uprawnienia_systemowe)
		SET @result = 1
	END
END
GO


Create PROCEDURE HistoriaTransakcji
@ID_Klienta as INT
AS
BEGIN

	SET NOCOUNT ON;

	SELECT k.Nazwa as 'Nazwa',k.Cena as 'Cena',k.Opis as 'Opis',kk.Data_Zakupu as 'Data_Zakupu_Karnetu',kk.Data_Konca as 'Data_Zakonczenia_Karnetu' FROM Karnety k join Karnet_Klient kk on kk.ID_Karnetu =k.ID_Karnetu WHERE kk.ID_Klienta = @ID_Klienta
END
GO


Create PROCEDURE JakieAtrakcje
@ID_Klienta as INT
AS
BEGIN
	DECLARE @date as Date;
	SET @date = GETDATE();

	DECLARE @pom as INT
	SET NOCOUNT ON;

	SET @pom = (SELECT MAX(ka.Uprawnienia) FROM Klienci k join Karnet_Klient kk on k.ID_Klienta=kk.ID_Klienta join Karnety ka on ka.ID_Karnetu=kk.ID_Karnetu
	 where kk.Data_Zakupu <= @date and kk.Data_Konca >= @date and kk.ID_Klienta = @ID_Klienta)
	 
	SELECT * FROM Atrakcje WHERE Uprawnienia <= @pom

END
GO


Create PROCEDURE [dbo].[ProceduraLogowania]
@login AS NVARCHAR(70),
@haslo AS NVARCHAR(70),
@uprawnienie  AS int output

AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS(SELECT Uprawnienia_Systemowe FROM Pracownicy  where Login like @login and haslo like @haslo)
	BEGIN
		SET @uprawnienie = -1
		return
	END
	SET @uprawnienie = 1;
	IF EXISTS(SELECT * from Pracownicy where Login like @login and haslo like @haslo) 
	BEGIN
		SELECT @uprawnienie = Uprawnienia_Systemowe FROM Pracownicy  where Login like @login and haslo like @haslo
	END
END
GO


CREATE PROCEDURE PrzypiszTrenerAtrakcja
@ID_Trenera as INT,
@ID_Atrakcji as INT,
@result as INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT * FROM Trenerzy_Atrakcji WHERE ID_Atrakcji = @ID_Atrakcji and ID_Pracownika = @ID_Trenera)
		SEt @result = 0
	ELSE
	BEGIN 
		INSERT INTO Trenerzy_Atrakcji values(@ID_Trenera,@ID_Atrakcji)
		SET @result = 1
	END
END
GO

CREATE PROCEDURE PrzypiszKarnetKlient
@ID_Karnetu as INT,
@ID_Klienta as INT,
@result as INT OUTPUT
AS
BEGIN
	DECLARE @Data_Zakupu DATE
	SET @Data_Zakupu = CAST(GETDATE() as DATE)

	SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM Karnet_Klient WHERE ID_Karnetu = @ID_Karnetu and ID_Klienta = @ID_Klienta and Data_zakupu = @Data_Zakupu )
		SET @result =  0
	ELSE
	BEGIN 
		DECLARE @Data_Konca DATE
		DECLARE @ilosc AS INT = (SELECT Ilosc_Miesiecy FROM Karnety WHERE ID_Karnetu = @ID_Karnetu)
		SET @Data_Konca = CAST(DATEADD(month,@ilosc,@Data_Zakupu) as DATE)
		INSERT INTO Karnet_Klient values(@ID_Karnetu,@ID_Klienta,@Data_Zakupu,@Data_Konca)
		SET @result = 1
	END
END
GO

Create PROCEDURE WyswietlAtrakcjeOUprawnieniach
@Uprawnienie as INT = null,
@result as INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	IF(@Uprawnienie = null)
		BEGIN
			SELECT Nazwa FROM Atrakcje 
			SET @result = 1
		END
	ELSE IF(@Uprawnienie > 3 or @Uprawnienie < 0)
		SET @result =  0
	ELSE
		BEGIN
			SELECT Nazwa FROM Atrakcje WHERE Uprawnienia = @Uprawnienie
			SET @result =  1
		END
END
GO


CREATE PROCEDURE WyswietlCertyfikatyTrenera
@ID_Trenera as INT,
@result as INT OUTPUT
AS
BEGIN
 --Jesli nie ma takiego trenera (0.1% prawdopodobne) to wtedy ma wyskakiwac return 0 czyli blad
	SET NOCOUNT ON;
	IF NOT EXISTS(SELECT * FROM Pracownicy WHERE ID_Pracownika = @ID_Trenera)
		SET @result =  0
	ELSE IF NOT EXISTS(SELECT * FROM Trenerzy_Certyfikaty WHERE ID_Pracownika = @ID_Trenera)
		BEGIN
			SELECT 'Brak' as Nazwa_Certyfikatu
			SET @result =  1
		END
	ELSE
		BEGIN
			SELECT Nazwa_Certyfikatu from Trenerzy_Certyfikaty WHERE ID_Pracownika = @ID_Trenera
			SET @result =  1
		END
END
GO


CREATE PROCEDURE WyswietlKarnetyKlienta
@Imie as NVARCHAR(100),
@Nazwisko as NVARCHAR(100),
@result as INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT * FROM Klienci WHERE Imie like @Imie and Nazwisko like @Nazwisko)
		SET @result =  0
	ELSE
		BEGIN
			SELECT ka.Nazwa,ka.Cena,kk.Data_Zakupu,kk.Data_Konca FROM Karnet_Klient kk join Karnety ka on ka.ID_Karnetu=kk.ID_Karnetu 
			SET @result =  1
		END
END
GO

CREATE PROCEDURE WyswietlKarnetyOUprawnieniach
@Uprawnienie as INT = null,
@result as INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	IF(@Uprawnienie = null)
		BEGIN
			SELECT Nazwa,Cena,Opis FROM Karnety
			SET @result =  1
		END
	ELSE IF(@Uprawnienie > 3 or @Uprawnienie < 0)
		SET @result =  0
	ELSE
		BEGIN
			SELECT Nazwa,Cena,Opis FROM Karnety WHERE Uprawnienia = @Uprawnienie
			SET @result =  1
		END
END
GO

CREATE PROCEDURE WyswietlPracownikow
@Typ_Pracownika as NVARCHAR(70)
AS
BEGIN
-- Jesli da Wszyscy maja byæ wszyscy
	SET NOCOUNT ON;

	IF(@Typ_Pracownika = 'Wszyscy')
		SELECT Imie,Nazwisko,Plec,Email,Numer_Telefonu from Pracownicy
	ELSE
		SELECT Imie,Nazwisko,Plec,Email,Numer_Telefonu from Pracownicy WHERE Rola like @Typ_Pracownika

END
GO
CREATE PROCEDURE JakieCertyfikaty
	@ID_Pracownika as INT
AS
BEGIN
	SET NOCOUNT ON;
	
	Select * From Trenerzy_Certyfikaty Where ID_Pracownika = @ID_Pracownika
END
GO


/* Koniec Sekcji Procedur Sk³adowanych */

/* Sekcja Triggerów */
CREATE TRIGGER DajDateDolaczenia
   ON  Klienci
   AFTER INSERT
AS 
BEGIN
	DECLARE @Data_Dolaczenia DATE
	SET @Data_Dolaczenia = CAST(GETDATE() as DATE)
	DECLARE @pom as INT 

	SET NOCOUNT ON;

	DECLARE kursor CURSOR for SELECT ID_KLienta FROM inserted WHERE Data_Dolaczenia is null
	OPEN kursor
	FETCH NEXT FROM kursor into @pom
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE Klienci SET Data_Dolaczenia = @Data_Dolaczenia WHERE ID_Klienta = @pom
	FETCH NEXT FROM kursor into @pom
	END
	CLOSE kursor
	DEALLOCATE kursor
END
GO

Create TRIGGER Usuwanie_Polaczen_Atrakcje
   ON  Atrakcje
   AFTER DELETE
AS 
BEGIN
	DECLARE @pomoc as INT;
	SET NOCOUNT ON;
	DECLARE kursor CURSOR for SELECT ID_Atrakcji from deleted
	OPEN kursor
	FETCH NEXT FROM kursor INTO @pomoc

	while	@@FETCH_STATUS = 0
	BEGIN
		DELETE FROM Trenerzy_Atrakcji WHERE ID_Atrakcji = @pomoc
		FETCH NEXT FROM kursor INTO @pomoc
	END
	
	CLOSE kursor
	DEALLOCATE kursor
END
GO

CREATE TRIGGER Usuwanie_Polaczen
   ON  Karnety
   AFTER DELETE
AS 
BEGIN
	DECLARE @pomoc as INT;
	SET NOCOUNT ON;
	DECLARE kursor CURSOR for SELECT ID_Karnetu from deleted
	OPEN kursor
	FETCH NEXT FROM kursor INTO @pomoc

	while	@@FETCH_STATUS = 0
	BEGIN
		DELETE FROM Karnet_Klient WHERE ID_Karnetu = @pomoc
		FETCH NEXT FROM kursor INTO @pomoc
	END
	CLOSE kursor
	DEALLOCATE kursor
END
GO


/* Koniec Sekcji Triggerów */


/* Sekcja Danych */

DECLARE @result as INT
exec DodawaniePracownika 'Admin','Admin','Mezczyzna','admin@gmail.com','+48 2173372121','Biuro','admin','admin',3,@result
exec DodawaniePracownika 'Trener','Trener','Mezczyzna','Trener@gmail.com','+48 2173372121','Trener','trener','trener',1,@result
exec DodawaniePracownika 'Agata','Nowak','Kobieta','agus12@gmail.com','+48 997997121','Biuro','prac','prac',2,@result
exec DodawanieKlienta 'Klient','Klient','Mezczyzna','Miasto','Andrzeja','2a',2,'email@gmail.com','123123123',@result

