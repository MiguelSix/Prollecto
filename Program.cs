﻿using System;

namespace Prollecto
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Lenguaje a = new Lenguaje("C:\\Users\\wachi\\OneDrive\\Escritorio\\Prollecto\\prueba.cpp");

                a.Programa();
                
                /*while(!a.FinArchivo())
                {
                    a.NextToken();
                }*/
                a.cerrar();
            }
            catch (Exception)
            {
                //Console.WriteLine("Fin de compilacion");
                Console.WriteLine("Error de compilacion");
            }
        }
    }
}
