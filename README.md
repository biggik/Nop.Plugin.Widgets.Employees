# Nop.Plugin.Widgets.Employees
This is a nopCommerce widget  plugin for managing employee info for displaying on public site, e.g. image, email, phone number, etc.

# Building
## Directory structure
The repository contains separate directories for each supported nopCommerce version, e.g. 4.20, 4.30, and so on.

## Common folder
The Common folder contains most of the common source code for each nopCommerce version of the plugin. Files are referenced (linked) from there.

## NopCommerce source
Each *Nop.Plugin.Widgets.Employees* project contains references to nopCommerce projects. For convenience the nopCommerce files are references via drive *n:*. It is recommended to map n: to the location of the nopCommerce source. For example, if nopCommerce source files are available in *c:\dev\nopCommerce public releases*, and each version in a subdirectory named by the release (i.e. *nopCommerce 4.20*) then the following command will map n: to that location

    net use n: "\\mycomputer\c$\dev\nopCommerce public releases" /persistent:yes

where mycomputer is the name of the development machine.
 
 ## Build the projects
 To build the projects, either open the preferred solution (.sln) file, or execute the build-all.cmd file in the root directory. That command builds all versions both in debug and release mode.

The output of the build, whether from Visual Studio, or from the build-all.cmd file, goes to the *_build* directory in the root directory
