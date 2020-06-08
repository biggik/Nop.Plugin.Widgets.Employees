@echo off
set plugin=Widgets.Employees
echo Running custom build step for %plugin%
set nopOut="n:\nopCommerce 4.30\Presentation\Nop.Web\Plugins\%plugin%"
if not exist %nopOut% (
    mkdir %nopOut%
    echo Created %nopOut%
)
if not exist %nopOut%\Content (
    mkdir %nopOut%\Content
    echo Created %nopOut%Content
)
if not exist %nopOut%\Views (
    mkdir %nopOut%\Views
    echo Created %nopOut%Views
)
xcopy /d /y /Q "..\_build\debug\logo.jpg" %nopOut%\. > nul
xcopy /d /y /Q "..\_build\debug\plugin.json" %nopOut%\. > nul
xcopy /d /y /Q "..\_build\debug\Nop.Plugin.%plugin%*.*" %nopOut%\. > nul
xcopy /d /y /Q /S "..\_build\debug\Content\." %nopOut%\Content\. > nul
xcopy /d /y /Q /S "..\_build\debug\Views\." %nopOut%\Views\. > nul
echo %nopOut% was updated