namespace MoogleEngine;
using System.IO;

using System.Text.RegularExpressions;
public class Document
{//Informacion general

    //Posicion original de ese documento en el corpus
    public int Originalpos { get; private set; }

    // Titulo del documento
    public string Title { get; private set; }

    //Texto completo del documento  
    public string Text { get; private set; }

    // Texto separado por palabras 
    public string[] Words { get; private set; }



    // Relacionado Con el Tf idf

    // Guarda la relevancia de este documento 
    public double Relevance;

    // Cantidad de palabras en este documento 
    public int NumberofWords { get; private set; }

    // Diccionario que guarda cada palabra y la cantidad de veces que aparece
    public Dictionary<string, int> TimesforWord { get; private set; }

    // Constructor de la clase Document
    public Document(string TTL, string texto, int pos)
    {
        this.Originalpos = pos;
        this.Title = TTL;
        this.Text = texto;
        this.Relevance = 0;
        this.Words = SplitWords(texto);
        this.NumberofWords = Words.Length;
        this.TimesforWord = CountOccurrences(this.Words);

    }


    //Funcion que recibe el texto y elimina signos de puntuacion y espacios
    public static string[] SplitWords(string inputString)
    {
        inputString = inputString.ToLower();
        char[] delimiters = { ' ', ',', '.', ':', ';', '!', '?', '-', '_', '"', '(', ')', '[', ']', '¿', '¡', '»', '«', '^', '*' }; // Add any other delimiters you want to ignore
        string[] words = inputString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i++)
        {
            if (Char.IsPunctuation(words[i][0]))
            {
                words[i] = words[i].Substring(1);
            }
        }

        return words;
    }

    // Funcion que recibe las palabras separadas y devuelve un diciionario con las palabras y la cantidad de veces que aparece cada una
    public static Dictionary<string, int> CountOccurrences(string[] arr)
    {
        //Crear un diccionio vacio
        Dictionary<string, int> counts = new Dictionary<string, int>();


        foreach (string element in arr)
        {
            // Si el elemento ya estaba en el diccionario incrementa en uno el valor de esta palabra 
            if (counts.ContainsKey(element))
            {
                counts[element]++;
            }
            // Si el elemento no estaba en el diccionario añadelo y asignale 1 
            else
            {
                counts.Add(element, 1);
            }
        }

        return counts;
    }

}
