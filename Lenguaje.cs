using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Requerimiento 1.- Grabar la fecha y hora en el log *
//Requerimiento 2.- Programar la produccion For -> for(Asignacion Condicion; Incremento) Bloque de instruccones | Intruccion *
//Requerimiento 3.- Programar la produccion Incremento -> Identificador ++ | --*
//Requerimiento 4.- Programar la produccion Switch -> switch (Expresion) {Lista de casos}
//Requerimiento 5.- Programar la produccion listaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (listaDeCasos)?
//Requerimiento 6.- Incluir en el Switch un default optativo
//Requerimiento 7.- Levantar una excepcion cuando el archivo de prueba.cpp no exista
//Requerimiento 8.- Si el programa a compilar es suma.cpp debera generar un suma.log
//Requerimiento 9.- Es necesario que el error lexico o sintactico indique el numero de liena en el que ocurrio

//Nuevos requerimientos: 
// 1.Eliminar las comillas del printf e interpretar las secuencias de escape de la cadena
// 2.Marcar los errorres sintacticos cuando la variable no exista
// 3.Modificar el valor de la variable en la asignacion
// 4.Obtener el valor de la variable cuando se requiera y programar el metodo getValor
// 5.Modificar el valor de la variable en el scanf

namespace Prollecto
{

    public class Lenguaje: Sintaxis
    {

        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }
        private void secuenciasEscape(string cadena)
        {
            //Eliminamos las comillas de la cadena
            cadena = cadena.Trim('"');
            //Console.WriteLine(cadena);
            cadena = cadena.Replace("\\n", "\n");
            cadena = cadena.Replace("\\t", "\t");
            Console.Write(cadena);
        }

        private void addVariable(string nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }

        private void displayVariables()
        {
            log.WriteLine("\n\nVariables:");
            foreach(Variable v in variables){
                log.WriteLine(v.getNombre() + ", " + v.getTipoDato() + ", " + v.getValor());
            }
        }

        private bool existeVariable(string nombre)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        //Requerimiento 3:
        private void modificaValor(string nombre, float nuevoValor) 
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre)) 
                {
                    v.setValor(nuevoValor);
                }
            }
        }

        //Requerimiento 4:
        private float getValor(string nombreVariable)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable)) 
                {
                    return v.getValor();
                }
            }
            return 0;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            displayVariables();
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

         //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;

                switch(getContenido()){

                    case "int": tipo = Variable.TipoDato.Int; break;

                    case "float": tipo = Variable.TipoDato.Float; break;

                    default: tipo = Variable.TipoDato.Char; break;
                }

                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

         //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador){
                if(!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }else
                {
                    throw new Error("Error de sintaxis, variable duplicada <"+ getContenido() + "> en la linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }    
            match("}"); 
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase()
        {
            Instruccion();
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase();
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "printf")
            {
                Printf();
            }
            else if (getContenido() == "scanf")
            {
                Scanf();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if(getContenido() == "do")
            {
                Do();
            }
            else if(getContenido() == "for")
            {
                For();
            }
            else if(getContenido() == "switch")
            {
                Switch();
            }
            else
            {
                Asignacion();
            }
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion()
        {
            //Requerimiento 2.-
            if(!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
            }

            log.WriteLine();
            string nombreVariable = getContenido();
            match(Tipos.Identificador);
            log.Write(nombreVariable + " = ");
            match(Tipos.Asignacion);
            Expresion();
            match(";");
            float resultado = stack.Pop();
            log.WriteLine("= " + resultado);
            log.WriteLine();
            //Requerimiento 3:
            modificaValor(nombreVariable, resultado);
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{") 
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            } 
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();  
            }
            else
            {
                Instruccion();
            }
        }

        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            //Requerimiento 2.-
            if(!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
            }
            string variable = getContenido();
            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
                modificaValor(variable, getValor(variable) + 1);
                match("++");
            }
            else if(getContenido() == "--")
            {
                modificaValor(variable, getValor(variable) - 1);
                match("--");
            }
            //match(";");
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch()
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos();
            if(getContenido() == "default")
            {
                match("default");
                match(":");
                ListaInstruccionesCase();
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
            }
            match("}");
        } 

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos()
        {
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            ListaInstruccionesCase();
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos();
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private void Condicion()
        {
            Expresion();
            stack.Pop();
            match(Tipos.OperadorRelacional);
            Expresion();
            stack.Pop();
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();  
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }

        //Printf -> printf(cadena || expresion);
        private void Printf()
        {
            match("printf");
            match("(");

            if(getClasificacion() == Tipos.Cadena)
            {
            String aux = getContenido();
            secuenciasEscape(aux);
            match(Tipos.Cadena);

            }
            else
            {
                Expresion();
                Console.Write(stack.Pop());
            }
            match(")");
            match(Tipos.FinSentencia); //(";")
        }

        //Scanf -> scanf(cadena  -> , -> & -> identificador);
        private void Scanf()    
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            //Requerimiento 2.-
            if(!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
            }
            string valor = "" + Console.ReadLine();
            //Requerimiento 5
            modificaValor(getContenido(), float.Parse(valor));
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones();
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "+": stack.Push(n2 + n1); break;
                    case "-": stack.Push(n2 - n1); break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*": stack.Push(n2 * n1); break;
                    case "/": stack.Push(n2 / n1); break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
            log.Write(getContenido() + " ");
            stack.Push(getValor(getContenido()));
            //Requerimiento 2
            if(!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
            }
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}