@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 17) battle
start /b "kienboec battle" curl -X POST http://localhost:10001/battles --header "Authorization: Basic kienboec-mtcgToken"
ping localhost -n 3 >NUL 2>NUL
start /b "altenhof battle" curl -X POST http://localhost:10001/battles --header "Authorization: Basic altenhof-mtcgToken"
ping localhost -n 10 >NUL 2>NUL
echo.
echo.

REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 100 >NUL 2>NUL
@echo on
