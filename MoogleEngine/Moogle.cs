using System.Diagnostics;
namespace MoogleEngine;
public static class Moogle
{
    static Corpus? cuerpo;
    public static SearchResult Query(string query)
    {

        //Añadir la query a mi corpus
        cuerpo!.MakeQuery(query, cuerpo);

        //Le damos su relevancia a cada documento
        Corpus.CosRelevancia(cuerpo);

        string suggestion ="";

        //Se crea una lista que contiene Titulo,snipest, y relevancia
        List<(string, string, double)> order = Corpus.Biggest(cuerpo);

        SearchItem[] items = new SearchItem[order.Count];

        //Si nuestra busqueda devuelve menos  de 3 resultados llama a la funcion que modifica nuestra recomendacion
        if (order.Count < 3)
        {
            suggestion = Corpus.Recomendation(cuerpo);
        }

//Crea search items de cada documento que tenga alguna relevancia
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new SearchItem(order[i].Item1, order[i].Item2, order[i].Item3);

        }


//Devolvemos la relevancia a 0
        foreach (var item in cuerpo.DocumentCorpus)
        {
            item.Relevance = 0;
        }
        System.Console.WriteLine("Finish");
        return new SearchResult(items, suggestion);

    }



    //Al iniciar el Moogle server se llama a esta funcion que crea nuestro cuerpo
    public static void CreateCorpus()
    {
        // en caso de cambiar la direccion de nuestros documentos se debe modificar este valor
        string direction = "../Content";


        Moogle.cuerpo = new Corpus(direction);
    }

}