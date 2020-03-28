using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace GUI_RegexLexer
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        RegexLexer csLexer = new RegexLexer();
        bool load;
        List<string> palabrasReservadas;
        List<string> palabrasRevervadasObjetoVariables;

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            using (StreamReader sr = new StreamReader(@"..\..\RegexLexer.cs"))
            {
                //tbxCode.Text = sr.ReadToEnd();

                csLexer.AddTokenRule(@"\s+", "ESPACIO", true);           
                csLexer.AddTokenRule(@"\b[_a-zA-Z][\w]*\b", "IDENTIFICADOR");
                csLexer.AddTokenRule("\".*?\"", "CADENA");
                csLexer.AddTokenRule(@"'\\.'|'[^\\]'", "CARACTER");
                csLexer.AddTokenRule("//[^\r\n]*", "COMENTARIO1");
                csLexer.AddTokenRule("/[*].*?[*]/", "COMENTARIO2");
                csLexer.AddTokenRule(@"\d*\.?\d+", "NUMERO");             
                csLexer.AddTokenRule(@"[\(\)\{\}\[\];,]", "DELIMITADOR");
                csLexer.AddTokenRule(@"[\.=\+\-/*%]","OPERADOR");
                csLexer.AddTokenRule(@">|<|==|>=|<=|!", "COMPARADOR");

                palabrasReservadas = new List<string>() { "abstract", "as", "async", "await",
                "checked", "const", "continue", "default", "delegate", "base", "break", "case", 
                "do", "else", "enum", "event", "explicit", "extern", "false", "finally",
                "fixed", "for", "foreach", "goto", "if", "implicit", "in", "interface",
                "internal", "is", "lock", "new", "null", "operator","catch",
                "out", "override", "params", "private", "protected", "public", "readonly",
                "ref", "return", "sealed", "sizeof", "stackalloc", "static",
                "switch", "this", "throw", "true", "try", "typeof", "namespace",
                "unchecked", "unsafe", "virtual", "void", "while", "float", "int", "long", "object",
                "get", "set", "new", "partial", "yield", "add", "remove", "value", "alias", "ascending",
                "descending", "from", "group", "into", "orderby", "select", "where",
                "join", "equals", "using","bool", "byte", "char", "decimal", "double", "dynamic",
                "sbyte", "short", "string", "uint", "ulong", "ushort", "var", "class", "struct" };

                palabrasRevervadasObjetoVariables = new List<string>() { "bool", "byte", "char",
                "decimal", "double", "dynamic", "sbyte", "short", "string", "uint", "ulong", "ushort",
                "var", "class", "struct", "params", "float", "float", "int", "long", "object"};

                csLexer.Compile(RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

                load = true;
                AnalizeCode();
                tbxCode.Focus();
            }
        }

        private void AnalizeCode()
        {
            lvToken.Items.Clear();

            int n = 0, e = 0;

            foreach (var tk in csLexer.GetTokens(tbxCode.Text))
            {
                if (tk.Name == "ERROR") e++;

                if (tk.Name == "IDENTIFICADOR")
                {
                    if (palabrasRevervadasObjetoVariables.Contains(((GUI_RegexLexer.Token)lvToken.Items[n - 1]).Lexema))
                    {
                        tk.Name = "VARIABLE";
                    }

                    if (palabrasReservadas.Contains(tk.Lexema))
                    {
                        tk.Name = "RESERVADO";
                    }
                }

                lvToken.Items.Add(tk);
                n++;
            }

            Title = string.Format("Analizador Lexico - {0} tokens {1} errores", n, e);
        }

        private void CodeChanged(object sender, TextChangedEventArgs e)
        {
            if (load)
            {
                AnalizeCode();
            }
        }
    }
}
