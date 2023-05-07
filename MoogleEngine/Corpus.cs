namespace MoogleEngine;
using System.IO;

public class Corpus
{
    //Query recibida
    public string Query { get; private set; }

    //Longitud de la direccion del archivo
    public int DirectionLen { get; private set; }

    //Direcciones de cada documento
    public string[] Paths { get; private set; }

    //Cuerpo de documentos 
    public List<Document> DocumentCorpus;

    // Tamaño del cuerpo
    public int sizecorpus { get; private set; }

    // Cantidad de veces que aparece cada palabra de la query
    public Dictionary<string, int> queryTimesforWord { get; private set; }

    //Cantidad de operadores por palabra 
    public Dictionary<string, int> OperWordsCant { get; private set; }

    //Lista donde cada diccionario es un documento que contiene sus palabras y su tfidf
    public List<Dictionary<string, double>> TfIdfByDoc;

    //Constructor de la clase 
    public Corpus(string Ruta)
    {
        this.DirectionLen = Ruta.Length + 1;

        // Se revisa en la carpeta todos los archivos txt de la carpeta
        this.Paths = Directory.GetFiles(Ruta, "*.txt");

        this.DocumentCorpus = new List<Document>();
        this.sizecorpus = DocumentCorpus.Count;

        //Se le pasa el título, el texto y la posicion que ocupa en el corpus al constructor de la clase document 
        for (int i = 0; i < Paths.Length; i++)
        {
            DocumentCorpus.Add(new Document(GetTitle(Paths[i], DirectionLen), File.ReadAllText(Paths[i]), i));
        }
        this.TfIdfByDoc = new List<Dictionary<string, double>>();

        //Calculamos los tf idf
        for (int i = 0; i < DocumentCorpus.Count; i++)
        {
            this.TfIdfByDoc.Add(CalcTfIdf(DocumentCorpus, i));
        }

        this.Query = "";
        this.queryTimesforWord = new();
        this.OperWordsCant = new();

    }
    //Funcion que añade a mi document corpus la query y la coloca en la ultima posicion
    public void MakeQuery(string query, Corpus corpus)
    {
        corpus.Query = query;
        corpus.DocumentCorpus.Add(new Document("QUERY", query, corpus.DocumentCorpus.Count));
        corpus.queryTimesforWord = DocumentCorpus[DocumentCorpus.Count - 1].TimesforWord;
        corpus.OperWordsCant = OperConvert(query);
        corpus.TfIdfByDoc.Add(CalcTfIdf(DocumentCorpus, DocumentCorpus.Count - 1));

    }

    // Funcion que recorta la ruta del archivo y devuelve su titulo
    private string GetTitle(string str, int dir)
    {
        string ttl = "";
        int pos = str.IndexOf(".txt") - dir;
        ttl = str.Substring(dir, pos);
        return ttl;
    }


    #region Tf IDF

    //Funcion que nos devolvera un diccionario con el Tf idf de cada palabra en el documento
    public Dictionary<string, double> CalcTfIdf(List<Document> DocCorpus, int i)
    {
        Dictionary<string, double> tfidf = new Dictionary<string, double>();

        foreach (string pal in DocCorpus[i].TimesforWord.Keys)
        {
            tfidf.Add(pal, (TF(pal, DocCorpus[i].NumberofWords, DocCorpus[i].TimesforWord) * IDF(pal, DocCorpus)));
        }

        return tfidf;
    }

    // Esta funcion me devuelve la cantidad de documentos que contienen mi palabra
    public static int CantdocWrd(string targetword, List<Document> alldoc)
    {
        int counter = 0;
        for (int i = 0; i < alldoc.Count; i++)
        {
            if (alldoc[i].TimesforWord.ContainsKey(targetword))
            {
                counter++;
            }

        }
        return counter;
    }
    //Calcula el TF
    public double TF(string keyword, int tamn, Dictionary<string, int> fapal)
    {
        double tf = 0;
        double cant = tamn;
        double repeats = fapal[keyword];

        //Se divide la cantidad de veces que se repite la palabra entre la cantidad de palabras del documeto
        tf = repeats / cant;
        return tf;
    }

    // Calcula el IDf
    public double IDF(string searchword, List<Document> corpus)
    {
        double amountofdoc = corpus.Count;
        double wordindoc = CantdocWrd(searchword, corpus);

        // Calcula el logaritmo en base e de la cantidad de documentos entre la cantidad de documentos en los que aparece la palabra
        double idf = Math.Log(amountofdoc/ wordindoc);
        return idf;
    }

    // Se le dara la relevancia a cada doc
    public static void CosRelevancia(Corpus biblioteca)
    {
        // primero se revisa  si tiene ** en la busqueda para aumentar el tfidf de esa palabra 
        OperatorsChecker1(biblioteca.TfIdfByDoc, biblioteca.OperWordsCant);

        for (int i = 0; i < biblioteca.TfIdfByDoc.Count - 1; i++)// el menos 1 es para que ignore la query que esta en el ultimo elemento.
        {
            double sumatotal = 0;

            // tf idf de la palabra en la query  
            double palquery = 0;
            //suma del cuadrado del tfidf de las palabras de la query
            double sumaquery = 0;
            //tfidf de la palabra en este documento
            double paldoc1 = 0;
            //suma de los cuadrados del tfidf de las palbras en el documento
            double sumadoc1 = 0;


            //se recorre todas las palabras del documento
            foreach (string pal in biblioteca.TfIdfByDoc[i].Keys)
            {
                paldoc1 = biblioteca.TfIdfByDoc[i][pal];
                sumadoc1 = sumadoc1 + Math.Pow(paldoc1, 2);

                //si la palabra está en la query se multiplica el valor del tfidf de la palabra en el documento por el de la query y se va acumulando en sumatotal
                if (biblioteca.TfIdfByDoc[biblioteca.TfIdfByDoc.Count - 1].ContainsKey(pal))
                {
                    palquery = biblioteca.TfIdfByDoc[biblioteca.TfIdfByDoc.Count - 1][pal];

                    sumatotal = sumatotal + (paldoc1 * palquery);
                }
            }
            //se recorre todas las palabras de las query
            foreach (string pal in biblioteca.TfIdfByDoc[biblioteca.TfIdfByDoc.Count - 1].Keys)
            {
                sumaquery = sumaquery + Math.Pow(biblioteca.TfIdfByDoc[biblioteca.TfIdfByDoc.Count - 1][pal], 2);
            }

            //Modificamos el valor de la relevancia
            biblioteca.DocumentCorpus[i].Relevance = sumatotal / Math.Sqrt(sumadoc1 * sumaquery);

        }
        //Se le devuelve el valor original a los tfidf que fueron modificados por el operador **
        Restore(biblioteca.TfIdfByDoc, biblioteca.OperWordsCant);
        //Si existe operadores de cercania se revisara y aumentara el valor de la relevancia
        OperatorsChecker3(biblioteca);
        //Si existe operadores de inclusion y exclusion se revisara y se reevaluara la relevancia
        OperatorsChecker2(biblioteca);
    }
    #endregion


    #region Final Part
    // funcion que nos devuelve una parte del documento de 20 palabras siempre que sea posible
    public static string GetSnipet(Corpus corpus, int index)
    {
        //empezmos siempre con un string de palabras de 20 elementos 
        int size = 20;
        //aqui guardaremos la mayor cantidad de palabras que han aparecido en un sector de 20
        int maxCount = 0;
        //posicion a partir de la que iniciamos a tomar nuestras palabras
        int initialpos = 0;
        // string que devolveremos
        string maxSection = "";
        //aqui llevamos la cuenta de el sector actual de 20 palabras 
        int count = 0;
        while (corpus.DocumentCorpus[index].Words.Length < size)
        {
            size--;
        }

        for (int i = 0; i < corpus.DocumentCorpus[index].Words.Length - size - 1; i++)
        {//a partir de que ya tengamos 20 palabras cada vez que nos movamos en el array si la primera palabra del sector era de las relevantas se disminuye 1 a count
            if (i >= size && corpus.queryTimesforWord.ContainsKey(corpus.DocumentCorpus[index].Words[i - size]))
            {
                count--;
            }
            // si la palabra en la que estamos actualmente es de la query suma 1 al contador
            if (corpus.queryTimesforWord.ContainsKey(corpus.DocumentCorpus[index].Words[i]))
            {
                count++;
            }
            if (i >= size - 1 && count > maxCount)
            {
                maxCount = count;
                initialpos = i;
            }
        }
        // añadimos todas las palabras a nuestro snipet
        for (int k = initialpos; k < initialpos + size; k++)
        {
            maxSection = maxSection + corpus.DocumentCorpus[index].Words[k] + " ";
        }

        return maxSection;
    }


    // funcion que ordena mi documet corpus por relevancia y crea un trio titulo snipet relevancia
    public static List<(string, string, double)> Biggest(Corpus corpus)
    {// se elimina mi query pues no la voy a usar mas y no queremos que afecte futuras busquedas
        corpus.DocumentCorpus.RemoveAt(corpus.DocumentCorpus.Count - 1);
        corpus.TfIdfByDoc.RemoveAt(corpus.TfIdfByDoc.Count - 1);
        // se ordena de acuerdo  a la relevancia
        corpus.DocumentCorpus.Sort((o1, o2) => o1.Relevance.CompareTo(o2.Relevance));

        var l = new List<(string, string, double)>();

        int ind = corpus.DocumentCorpus.Count - 1;
        // solo se tienen en cuenta documentos con relevancia distinta a 0 y se empieza de mayor a menor
        while (corpus.DocumentCorpus[ind].Relevance != 0)
        {
            l.Add((corpus.DocumentCorpus[ind].Title, GetSnipet(corpus, ind), corpus.DocumentCorpus[ind].Relevance));
            ind--;
            if (ind == -1) break;
        }
        // se vuelve a ordenar mi cuerpo de acuerdo a la posicion original de los documentos
        corpus.DocumentCorpus.Sort((o1, o2) => o1.Originalpos.CompareTo(o2.Originalpos));
        return l;
    }

    //cuando es llamada devolvera un string conformado por palabras que devuelvan mas de 3 resultados de busuqeda 
    public static string Recomendation(Corpus corpus)
    {
        string Recomendation = "";

        foreach (string word in corpus.queryTimesforWord.Keys)
        {//se revisa si hay alguna palabra que contenga un substring de esta palabra de mis busqueda
            for (int i = word.Length; i > 0; i--)
            {

                foreach (Document item in corpus.DocumentCorpus)
                {
                    if (item.Title == "QUERY")
                    {
                        break;
                    }
                    foreach (string element in item.TimesforWord.Keys)
                    {
                        for (int j = 0; j + i < word.Length; j++)
                        {
                            if (element.Contains(word.Substring(j, i)) && CantdocWrd(element, corpus.DocumentCorpus) > 3)
                            {
                                Recomendation = Recomendation + " " + element;
                                // doy este salto para dar una recomendacion por palabra
                                goto endloop;
                            }
                        }
                    }
                }

            }
        endloop:;
        }

        return Recomendation;
    }
    #endregion


    #region Operators

    //recibe la query  y la divide en palabras y cuenta cuantos operadores tiene cada palabra
    public Dictionary<string, int> OperConvert(string query)
    {
        Dictionary<string, int> OperConverted = new Dictionary<string, int>();
        foreach (string item in SpliterMod(query))
        {
            OperConverted.Add(item, CountOper(item));

        }
        return OperConverted;
    }
    //cuenta cuantos operadores estan modificando a esta palabra
    public static int CountOper(string word)
    {
        List<char> opers = new List<char> { '!', '^', '*' };
        int initial = 0;
        for (int i = 0; i < word.Length; i++)
        {
            if (opers.Contains(word[i]))
            {
                initial++;
            }
            else
            {
                break;
            }
        }
        return initial;
    }

    //divide la query en palabras pero solo se queda con las que estan siendo modificadas
    public static List<string> SpliterMod(string inputString)
    {
        inputString = inputString.ToLower();
        char[] delimiters = { ' ', ',', '.', ':', ';', '?', '-', '_', '"', '(', ')', '[', ']', '¿', '¡', '»', '«' }; // Add any other delimiters you want to ignore
        string[] words = inputString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        List<string> newwords = new List<string>();

        for (int i = 0; i < words.Length; i++)
        {
            if (words[i][0] == '!' || words[i][0] == '*' || words[i][0] == '^') // Check if the first character of the word is a punctuation mark
            {
                newwords.Add(words[i]);
                //System.Console.WriteLine(newwords[0]);
            }
        }

        return newwords;
    }
    //Aumenta el tfidf de una palabra en base a la cant de **
    public static void OperatorsChecker1(List<Dictionary<string, double>> tfidf, Dictionary<string, int> operwords) // Funcionalidad provisional
    {

        foreach (string word in operwords.Keys)
        {
            string cutword = (word.Substring(operwords[word], word.Length - operwords[word]));

            if (word.Contains('*'))
            {
                //si la palabra tiene * el counter lleva la cuentade cuantos
                int counter = 0;
                for (int i = 0; i < operwords[word]; i++)
                {
                    if (word[i] == '*')
                    { counter++; }
                }
                foreach (Dictionary<string, double> item in tfidf)
                {//se va documento por documento y se aumenta el tf idf de esta palabra tantas veces como * haya
                    if (item.ContainsKey(cutword))
                    {
                        item[cutword] = item[cutword] * counter+1;
                    }
                }

            }
        }
    }
    // devuelve el tf idf de una palabra a su estado original luego de ser aumentado por el operador *
    public static void Restore(List<Dictionary<string, double>> tfidf, Dictionary<string, int> operwords)
    {
        foreach (string word in operwords.Keys)
        {
            string cutword = (word.Substring(operwords[word], word.Length - operwords[word]));

            if (word.Contains('*'))
            {
                int counter = 0;
                for (int i = 0; i < operwords[word]; i++)
                {
                    if (word[i] == '*')
                    { counter++; }
                }
                foreach (Dictionary<string, double> item in tfidf)
                {
                    if (item.ContainsKey(cutword))
                    {
                        item[cutword] = item[cutword] / counter;
                    }
                }
            }
        }
    }
    // Modifica la relevancia de acuerdo a si la palabra tiene ! o ^
    public static void OperatorsChecker2(Corpus corpus)
    {
        foreach (string word in corpus.OperWordsCant.Keys)
        {//si mi palabra posee al inicio un ! la relevancia de los documentos que la contengan debe ser 0 
            for (int i = 0; i < corpus.OperWordsCant[word]; i++)
            {


                if (word[i] == '!')
                {
                    foreach (Document item in corpus.DocumentCorpus)
                    {
                        if (item.TimesforWord.ContainsKey(word.Substring(corpus.OperWordsCant[word], word.Length - corpus.OperWordsCant[word])))
                        {
                            item.Relevance = 0;
                        }
                    }
                }
            }
            //si mi palabra posee al inicio un ! la relevancia de los documentos que no la contengan debe ser 0 
            for (int i = 0; i < corpus.OperWordsCant[word]; i++)
            {
                if (word[i] == '^')
                    foreach (Document item in corpus.DocumentCorpus)
                    {
                        if (!(item.TimesforWord.ContainsKey(word.Substring(corpus.OperWordsCant[word], word.Length - corpus.OperWordsCant[word]))))
                        {
                            item.Relevance = 0;
                        }
                    }
            }
        }
    }

    // mientras mayor sea la cercania de las 2 palabras entre las que se encuentra el operador ~ mayor sera la relevancia
    public static void OperatorsChecker3(Corpus corpus)
    {
        string[] querywords = corpus.DocumentCorpus[corpus.DocumentCorpus.Count - 1].Words;
        for (int i = 1; i < querywords.Length - 1; i++)
        {
            if (querywords[i] == "~")
            {
                // busco en que documento esta 
                for (int j = 0; j < corpus.DocumentCorpus.Count - 1; j++)
                {
                    // si el documento tiene ambas palabras que encierran al operador comenzamos a revisar el doc
                    if (corpus.DocumentCorpus[j].TimesforWord.ContainsKey(querywords[i - 1]) && corpus.DocumentCorpus[j].TimesforWord.ContainsKey(querywords[i + 1]))
                    {
                        int pal1 = -1;
                        int pal2 = -1;
                        int difmin = int.MaxValue;
                        // vamos revisando cadaa palabra del documento y si es una de mis 2 palabras calculo la dist entre mis 2 palabras al final me quedare con la menor dist posible
                        for (int p = 0; p < corpus.DocumentCorpus[j].Words.Length; p++)
                        {
                            if (corpus.DocumentCorpus[j].Words[p] == querywords[i - 1])
                            {
                                pal1 = p;

                            }
                            if (corpus.DocumentCorpus[j].Words[p] == querywords[i + 1])
                            {
                                pal2 = p;
                            }
                            // esta condicion se realiza porwue debo tener una aparicion de ambas palabras para empezar a medir la dist
                            if (pal2 != -1 && pal1 != -1)
                            {
                                if (difmin > Math.Abs(pal1 - pal2))
                                {
                                    difmin = Math.Abs(pal1 - pal2);
                                }
                            }
                        }

                        //Modifico mi relevancia con una funcion inversa que mientras mayor sea la distancia menor sera la bonificacion

                        double a = corpus.DocumentCorpus[j].Relevance;
                        corpus.DocumentCorpus[j].Relevance = a + (10 * (1 / (double)difmin));




                    }

                }

            }
        }
    }


    #endregion
}
