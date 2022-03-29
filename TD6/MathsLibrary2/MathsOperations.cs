using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MathsLibraryRest
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class MathsOperations : IMathsOperations
    {
        public int Add(int a, int b) { return a + b; }

        public int Substract(int a, int b) { return a - b; }

        public int Multiply(int a, int b) { return a * b; }

        public double Divide(double a, double b)
        {
            if (b == 0)
            {
                return 0;
            }
            return (double)(a / b);
        }

        public int AddXML(int a, int b) { return a + b; }

        public int SubstractXML(int a, int b) { return a - b; }

        public int MultiplyXML(int a, int b) { return a * b; }

        public double DivideXML(double a, double b)
        {
            if (b == 0)
            {
                return 0;
            }
            return (double)(a / b);
        }

        public int AddBody(int a, int b) { return a + b; }

        public int SubstractBody(int a, int b) { return a - b; }

        public int MultiplyBody(int a, int b) { return a * b; }

        public double DivideBody(double a, double b)
        {
            if (b == 0)
            {
                return 0;
            }
            return (double)(a / b);
        }


        public int AddPost(int a, int b) { return a + b; }

        public int SubstractPost(int a, int b) { return a - b; }

        public int MultiplyPost(int a, int b) { return a * b; }

        public double DividePost(double a, double b)
        {
            if (b == 0)
            {
                return 0;
            }
            return (double)(a / b);
        }
    }
}
