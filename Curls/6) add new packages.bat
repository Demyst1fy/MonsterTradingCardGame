@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 6) add new packages
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "{\"PackId\": \"afc3b2d2-1228-9790-9977-fc958234a70d\", \"Cards\": [{\"Id\":\"67f9048f-99b8-4ae4-b866-d8008d00c53d\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"Name\":\"RegularSpell\", \"Damage\": 50.0}, {\"Id\":\"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"Name\":\"Knight\", \"Damage\": 20.0}, {\"Id\":\"02a9c76e-b17d-427f-9240-2dd49b0d3bfd\", \"Name\":\"RegularSpell\", \"Damage\": 45.0}, {\"Id\":\"2508bf5c-20d7-43b4-8c77-bc677decadef\", \"Name\":\"FireElf\", \"Damage\": 25.0}]"
echo.
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "{\"PackId\": \"94789bde-849d-aeab-80f6-2a0ebbc6b2c6\", \"Cards\": [{\"Id\":\"70962948-2bf7-44a9-9ded-8c68eeac7793\", \"Name\":\"WaterGoblin\", \"Damage\":  9.0}, {\"Id\":\"74635fae-8ad3-4295-9139-320ab89c2844\", \"Name\":\"FireSpell\", \"Damage\": 55.0}, {\"Id\":\"ce6bcaee-47e1-4011-a49e-5a4d7d4245f3\", \"Name\":\"Knight\", \"Damage\": 21.0}, {\"Id\":\"a6fde738-c65a-4b10-b400-6fef0fdb28ba\", \"Name\":\"FireSpell\", \"Damage\": 55.0}, {\"Id\":\"a1618f1e-4f4c-4e09-9647-87e16f1edd2d\", \"Name\":\"FireElf\", \"Damage\": 23.0}]"
echo.
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "{\"PackId\": \"efd64b3c-1bc9-3da5-79cf-fd09ce0f00ef\", \"Cards\": [{\"Id\":\"2272ba48-6662-404d-a9a1-41a9bed316d9\", \"Name\":\"WaterGoblin\", \"Damage\": 11.0}, {\"Id\":\"3871d45b-b630-4a0d-8bc6-a5fc56b6a043\", \"Name\":\"Dragon\", \"Damage\": 70.0}, {\"Id\":\"166c1fd5-4dcb-41a8-91cb-f45dcd57cef3\", \"Name\":\"Knight\", \"Damage\": 22.0}, {\"Id\":\"237dbaef-49e3-4c23-b64b-abf5c087b276\", \"Name\":\"WaterSpell\", \"Damage\": 40.0}, {\"Id\":\"27051a20-8580-43ff-a473-e986b52f297a\", \"Name\":\"FireElf\", \"Damage\": 28.0}]"
echo.
echo.

REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 100 >NUL 2>NUL
@echo on
