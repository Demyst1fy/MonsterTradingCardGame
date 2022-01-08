<h1 align="center">MonsterTradingCardGame</h1>

SWEN1 Semesterprojekt - MonsterTradingCardGame

## Design

Anfangs wurden 3 Projekte erstellt:
1. Game-Komponenten (Card, Match, Stats, User)
2. Server-Komponenten (HttpServer, Request, Response, HandleActions)
3. Unit-Tests (Game & Server)

Für dieses Projekt wurde lediglich kein Client, sondern nur der Server implementiert.

Grundsätzlich wird beim Aufruf des Programms die Klasse HttpServer instanziiert und mittels Start() der Server gestartet, der 
durch das Threading mehrere Client-Requests bekommt und entsprechend behandelt werden. 
Die Requests werden in der Funktion ParseRequest(data) in der ServiceHandler-Klasse analysiert und aufgeteilt in Methode (GET, POST, ...), 
Query (/users, /sessions, ...), Content (z.B. User-Daten) und gegebenenfalls Authentication-Token. 
Die Datenbank-Klasse wird bei jedem Request aufgerufen, die den PostGreSQL Server anspricht und dementsprechend 
die SQL-Commands ausführt.

Speziell zu den Battles:

Die Battles werden im HttpServer in einem ConcurrentQueue gespeichert und als Referenz an den ServiceHandler weitergeschickt. Die Users
rufen "POST" und "/battle" auf, werden in die Queue gesteckt und warten solange, bis sich sein Gegner verbindet. Das Match befindet sich
nur im HandleBattle() und gegen Ende wird ein Gamelog als Response zurückgeschickt und das Match dann von der Queue entfernt.
Als Unique-Feature wurde eine Crit-Damage Mechanic eingebaut. Erst werden die Monster-Abhängigkeiten und die Element-Wechselwirkungen
durchlaufen und anschließend besteht eine 10% Chance, dass der Schaden um das Doppelte erhöht wird.

Beispiel:

Wenn zum Beispiel der Request (Methode: "POST"; Query: "/users"; Content: "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}")
gesendet wird, wird die Funktion HandleRegistration(request) aufgerufen, der das JSON deserialisiert in einer User-Klasse 
mit den Fields Username und Password. Die Userdaten werden dann in die User-Tabelle eingefügt. Bei erfolgreicher Ausführung wird dann
schließlich eine Response mit Code: 201 und Message: "You are now registered!" zurückgeschickt.

Architektur:
* Presentation layer: Response
* Business layer: HttpServer
* Persistence layer: ServiceHandler
* Database layer: HandleActions & Database

## Implementierte Funktionen

Grundfunktionen:
* Registrierung & Login (mit Authentifizierungsschlüssel)
* Packs erstellen und kaufen
* Stack & Deck erstellen/konfigurieren/anzeigen
* Kampfsystem (mit Crit als Unique Feature)
* Statistiken & Scoreboard
* Profilbearbeitung
* Tauschhandel (Karte vs Karte)

Optimale Features:
* Mehrere Elemente & Monstertypen:
  * Elemente: Dark & Light
  * Monstertypen: Vampire & Fairy
* Alle 10 gespielte Matches -> +2 Coins
* Verlauf der Transaktionen
* Winrate in Statistiken

## Lessons learned

Anfangs habe ich bei meiner Solution für jede Klasse ein Projekt erstellt, worauf ich dann später gemerkt habe, dass Konflikte
zwischen Abhängigkeiten aufgetreten sind und dann die Solution nochmal umstrukturieren musste. Da ich letztes Semester im 
Informatik-Projekt schon ein Server auf JavaScript mit meiner Gruppe implementiert habe, konnte ich auch etwas Vorwissen von der
Socket-Programmierung benutzen. 

Die größte Herausforderung stellte sich jedoch das Threading für das Battle gegenüber. Die Herangehensweise gegen Ende war, dass 
User sich in der HandleBattle() befinden, solange sie nach einem Gegner suchen.
Beim Curl-Script habe ich ein Delay zwischen den Spielerrequests beim Queue der Battle eingebaut, weil es in unserem Fall 
unwahrscheinlich ist, dass die beiden Spieler zeitgleich den gleichen Request schicken, 
was zum "Aufhängen" der Spielerqueue geführt hat.

Im Großen und Ganzen habe ich am meisten gelernt, dass man sich schon im Vorhinein die Klassen- und Ordnerstruktur
des Projekt überlegen und wie man was implementieren muss. Vorallem Kommentare zum Code sollte man eigentlich
während der Implementierung machen und nicht erst gegen Ende.

## Unit Tests

Game:

Für die Unit Tests wurden großteils Assert-Funktionen verwendet, um sehen zu können, ob die Objekte (User, Card, ...) richtig konstruiert werden.
Für die Tests im Match werden Mockings verwendet, um checken zu können, ob die entsprechenden Funktionen (CompareElement() und CheckEffect())
wirklich aufgerufen werden.

Server:

Das Parsing der Requests (ParseRequest) und das Zurückgeben der Response-Message wurden getestet, indem sie in Assert überprüft werden.
Im HandleAction werden wieder Mockings verwendet, um zu überprüfen, ob die entsprechenden Funktionen aufgerufen werden.
(z.B. "GET" & "/deck" & "Authorization: Basic kienboec-mtcgToken" => HandleShowDeck()).

Großteils wurden viele Methoden durch Integration Tests (Curl-Scripts) abgedeckt.

## Zeitaufwand

* Card-Class: 5h
* Battle: 6h
* HttpServer/Threading: 5h
* Parse Request: 4h
* Database connection & methods: 15h
* Servicehandler: 12h
* Unit-Tests: 8h

## [Link zum GitHub-Repository](https://github.com/Demyst1fy/MonsterTradingCardGame)
