namespace MoogleEngine;


//esta clase no se usa en la implementacion pero se deja para la evaluación conjunta con la asignatura algebra
public class Matrix
{
    public double[,] rows { get; private set; }

    public Matrix(double[,] arrays)
    {
        this.rows = arrays;
    }
    // Rota la matrix un numero b de veces y la direccion depende de si es positivo(derecha) o negativo(izquierda)
    public static double[,] RotateMatrix(double[,] a, int b)
    {
        int contador = 0;
        if (b < 0)
        {//hayamos la rotacion equivalente al dividir el numero de rotaciones entre 4 
            while (contador != Math.Abs(b) % 4)
            {
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        a[i, j] = a[j, a.GetLength(0) - 1 - i];
                    }
                }
                contador++;
            }
        }
        else
        {
            while (contador != b % 4)
            {
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        a[i, j] = a[a.GetLength(1) - j - 1, i];
                    }
                }
                contador++;
            }
        }
        return a;
    }

    // Metodo que recorre la matriz y nos permite observar el estado actual de los valores
    public static void ShowMatrix(double[,] a)
    {
        System.Console.WriteLine();
        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                System.Console.Write(a[i, j] + " ");
            }
            System.Console.WriteLine();
        }
    }

    //suma 2 matrices yendo valor por valor en la misma posicion de ambas matrices , se adicionan y se asignan a la matriz resultante 
    public static double[,] PlusMatrix(double[,] a, double[,] b)
    {
        double[,] c = (double[,])a.Clone();
        //matrices de dimensiones distintas no se pueden sumar
        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
        {
            System.Console.WriteLine("Imposible sumar 2 matrices de dimensiones de dimensiones distintas");
            for (int i = 0; i < c.GetLength(0); i++)
            {
                for (int j = 0; j < c.GetLength(1); j++)
                {
                    c[i, j] = int.MaxValue;
                }

            }
            return c;
        }


        for (int i = 0; i < c.GetLength(0); i++)
        {
            for (int j = 0; j < c.GetLength(1); j++)
            {
                c[i, j] += b[i, j];
            }

        }
        return c;

    }

    public static double[,] MinusMatrix(double[,] a, double[,] b)
    {
        double[,] c = (double[,])a.Clone();

        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
        {
            System.Console.WriteLine("Imposible restar 2 matrices de dimensiones de dimensiones distintas");
            for (int i = 0; i < c.GetLength(0); i++)
            {
                for (int j = 0; j < c.GetLength(1); j++)
                {
                    c[i, j] = int.MinValue;
                }

            }
            return c;
        }


        for (int i = 0; i < c.GetLength(0); i++)
        {
            for (int j = 0; j < c.GetLength(1); j++)
            {
                c[i, j] -= b[i, j];
            }
        }
        return c;

    }

    public static double[,] EscalarMulti(double[,] a, double b)
    {
        double[,] c = (double[,])a.Clone();

        for (int i = 0; i < c.GetLength(0); i++)
        {
            for (int j = 0; j < c.GetLength(1); j++)
            {
                c[i, j] = c[i, j] * b;
            }

        }
        return c;

    }
    //funcion que recibe 2 matrices y calcula el producto entre ambas 
    public static double[,] MatrixMulti(double[,] a, double[,] b)
    {
        double[,] c = new double[a.GetLength(0), b.GetLength(1)];
        if (a.GetLength(1) != b.GetLength(0))
        {
            System.Console.WriteLine("Imposible multiplicar esas matrices");

            return c;
        }
        //multiplica cada elemento de la fila de la primera matriz por cada elemento de la columna de lla segunda matriz suma los resultados y se lo asigna a la posicion 
        for (int i = 0; i < c.GetLength(0); i++)
        {
            for (int j = 0; j < c.GetLength(1); j++)
            {
                double sumaprod = 0;
                for (int k = 0; k < a.GetLength(1); k++)
                {
                    sumaprod = sumaprod + a[i, k] * b[k, j];
                }
                c[i, j] = sumaprod;
            }

        }
        return c;

    }
    //recibe una matriz y devuelve su transpuesta 
    public static double[,] Traspuesta(double[,] a)
    {
        double[,] c = (double[,])a.Clone();
        double[,] d = new double[c.GetLength(1), c.GetLength(0)];
        // convertimos cada fila en columna
        for (int i = 0; i < d.GetLength(0); i++)
        {
            for (int j = 0; j < d.GetLength(1); j++)
            {
                d[i, j] = c[j, i];
            }

        }
        return d;
    }
    // funcion que dada una matriz cuadrada nos devuelve su determinante
    public static double Determinante(double[,] matriz)
    {
        if (matriz.GetLength(1) != matriz.GetLength(0))
        {
            System.Console.WriteLine("Imposible calcular determinantte de una matriz no cuadrada");

            return int.MinValue;
        }
        int n = matriz.GetLength(0);
        double det = 0;

        if (n == 1)
        {
            // Caso matriz de tamaño 1
            det = matriz[0, 0];
        }
        else if (n == 2)
        {
            // Caso matriz de tamaño 2
            det = matriz[0, 0] * matriz[1, 1] - matriz[0, 1] * matriz[1, 0];
        }
        else
        {
            // Caso recursivo: matriz de tamaño n > 2
            for (int i = 0; i < n; i++)
            {
                double[,] menor = Menor(matriz, 0, i);

                // Calcular el determinante del menor y sumarlo al resultado
                det += matriz[0, i] * Math.Pow(-1, i) * Determinante(menor);
            }
        }

        return det;
    }
    // dada una matriz nos devuelve uno de sus menores eliminando una fila y columna dadas
    public static double[,] Menor(double[,] matriz, int fila, int columna)
    {
        int n = matriz.GetLength(0);
        double[,] menor = new double[n - 1, n - 1];

        int x = 0;
        int y = 0;

        for (int i = 0; i < n; i++)
        {
            if (i == fila)
            {
                continue;
            }

            for (int j = 0; j < n; j++)
            {
                if (j == columna)
                {
                    continue;
                }

                menor[x, y] = matriz[i, j];
                y++;

                if (y >= n - 1)
                {
                    y = 0;
                    x++;
                }
            }
        }

        return menor;
    }
    // metodo que nos devuelve la matriz inversa de una matriz 
    public static double[,] Inverse(double[,] matrix)
    {
        int n = (int)Math.Sqrt(matrix.Length);
        double[,] adjunta = new double[n, n];
        double det = Determinante(matrix);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double[,] minor = Menor(matrix, i, j);
                double cofactor = ((i + j) % 2 == 0 ? 1 : -1) * Determinante(minor);
                adjunta[j, i] = cofactor;
            }
        }

        double[,] inverse = EscalarMulti(adjunta, 1 / det);
        return inverse;
    }
}
