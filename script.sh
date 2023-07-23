#!/bin/bash
while true; do

read -p "Inserte un comando: " C 
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
                 cd ..
                 cd ./presentacion
                 rm presentacion.aux
                 rm presentacion.log
                 rm presentacion.out
                 rm presentacion.nav
                 rm presentacion.snm
                 rm presentacion.toc
                 rm presentacion.pdf
                 cd ..
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