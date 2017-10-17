using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer.Distributions;

namespace ZeroInflatedPoisson
{
    class Program
    {
        static void Main(string[] args)
        {
            int N = 1000;
            double pi = 0.3;
            double lambda = 2.0;

            int[] data = SampleData(N, pi, lambda);

            Variable<int> numPoints = Variable.Observed<int>(N);
            Range n = new Range(numPoints);

            Beta cPrior = new Beta(1, 1);
            Gamma pPrior = new Gamma(1, 4);

            var probTrue = Variable.Random<double>(cPrior);
            var mean = Variable.Random<double>(pPrior);

            var x = Variable.Array<int>(n);

            using (Variable.ForEach(n))
            {
                var c = Variable.Bernoulli(probTrue);
                using (Variable.If(c))
                    x[n] = 0;
                using (Variable.IfNot(c))
                    x[n] = Variable.Poisson(mean);
            }

            x.ObservedValue = data;

            InferenceEngine engine = new InferenceEngine();

            Beta probTrueMarginal = engine.Infer<Beta>(probTrue);
            Gamma meanMarginal = engine.Infer<Gamma>(mean);

            Console.WriteLine("E(pi) = " + probTrueMarginal.GetMean());
            Console.WriteLine("E(lambda) = " + meanMarginal.GetMean());

            Console.WriteLine("Press any key ...");
            Console.ReadKey();
        }

        public static int[] SampleData(int N, double pi, double lambda)
        {
            int[] data = new int[N];
            Bernoulli coin = new Bernoulli(pi);
            Poisson poisson = new Poisson(lambda);
            for (int i = 0; i < N; i++)
            {
                bool coin_value = coin.Sample();
                if (coin_value)
                    data[i] = 0;
                else
                    data[i] = poisson.Sample();
            }
            return data;
        }
    }
}
