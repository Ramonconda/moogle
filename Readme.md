# Moogle!

![](moogle.png)

> Proyecto de Programación I. 
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2023, 2024.

Moogle! es una aplicación cuyo propósito es buscar inteligentemente un texto en un conjunto de documentos.

Es una aplicación web, desarrollada con tecnología .NET Core 6.0, específicamente usando Blazor como *framework* web para la interfaz gráfica, y en el lenguaje C#.
La aplicación está dividida en dos componentes fundamentales:

- `MoogleServer` es un servidor web que renderiza la interfaz gráfica y sirve los resultados.
- `MoogleEngine` es una biblioteca de clases donde está implementada la lógica del algoritmo de búsqueda.

Para ejecutar el proyecto abrir una teminal y escribir el siguiente comando: dotnet watch run --project MoogleServer.
En caso de existir problemas al cargar los documentos modificar en la carpeta Moogle Server/ Clase Moogle / funcion CreateCorpus/ el valor direction que por defecto tiene : "../Content"


