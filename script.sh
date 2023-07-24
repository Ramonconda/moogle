#!/bin/bash
while true; do


read -p "
LISTA DE COMANDOS:

run: ejectutar el proyecto

report: compilar y generar el pdf del proyecto latex relativo al informe

slides: compilar y generar el pdf del proyecto latex relativo a la presentación

show_report: ejecutar un programa que permita visualizar el informe (normal lo ejecuta por defecto custom permite elegir comando de ejecucion)

show_slides: ejecutar un programa que permita visualizar la presentación (normal lo ejecuta por defecto custom permite elegir comando de ejecucion)

clean: eliminar todos los ficheros auxiliares que no forman parte del contenido del repositorio

exit: salir del script

Inserte un comando: " C 
echo $C

case $C in 
          "slides")
                  cd ./presentacion
                  pdflatex presentacion.tex
                  cd ..

          ;;
          "report")
                  cd ./informe
                  pdflatex Latex.tex
                  cd ..
          ;;
          "run")
              dotnet watch run --project MoogleServer
          ;;
          "clean")
                 cd ./informe
                 rm Latex.aux
                 rm Latex.log
                 rm Latex.pdf
                 rm Latex.fdb_latexmk
                 rm Latex.fls
                 rm Latex.synctex.gz
                 cd ..
                 cd ./presentacion
                 rm e.log
                 rm x.log
                 rm presentacion.aux
                 rm presentacion.fdb_latexmk
                 rm presentacion.fls
                 rm presentacion.synctex.gz
                 rm presentacion.log
                 rm presentacion.out
                 rm presentacion.nav
                 rm presentacion.snm
                 rm presentacion.toc
                 rm presentacion.pdf
                 cd ..
          ;;
          "show_slides")
                read -p "normal or custom: " S
                echo $S

                case $S in
                          "normal")
                                  cd ./presentacion
                                  if [ -e presentacion.pdf ]; then
                                       start presentacion.pdf
                                  else
                                      pdflatex presentacion.tex
                                      start presentacion.pdf
                                  fi
                           ;;
                           "custom")
                                   cd ./presentacion
                                   if [ -e presentacion.pdf ]; then
                                   read -p "ingrese su comando de ejecucion de pdf " F
                                   echo $F
                                     $F presentacion.pdf
                                  else
                                      pdflatex presentacion.tex
                                      read -p "ingrese su comando de ejecucion de pdf " F
                                   echo $F
                                     $F presentacion.pdf
                                  fi
                           ;;
                           *)
                           echo "Comando invalido"
                 ;;
        esac
        ;;
        "show_report")
                read -p "normal or custom: " S
                echo $S

                case $S in
                          "normal")
                                  cd ./informe
                                  if [ -e Latex.pdf ]; then
                                       start Latex.pdf
                                  else
                                      pdflatex Latex.tex
                                      start Latex.pdf
                                  fi
                           ;;
                           "custom")
                                   cd ./informe
                                   if [ -e Latex.pdf ]; then
                                   read -p "ingrese su comando de ejecucion de pdf " F
                                   echo $F
                                     $F Latex.pdf
                                  else
                                      pdflatex Latex.tex
                                      read -p "ingrese su comando de ejecucion de pdf " F
                                   echo $F
                                     $F Latex.pdf
                                  fi
                           ;;
                           *)
                           echo "Comando invalido"
                 ;;
        esac
          ;;
          "exit")
                echo "Saliendo ... "
                break
             
          ;; 
          *)
                  echo "Comando invalido"
          ;;
        
esac
done