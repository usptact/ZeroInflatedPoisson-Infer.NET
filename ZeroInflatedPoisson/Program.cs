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
            int N = 1000;           // number of data points
            double pi = 0.3;        // bias of the coin: prob. producing 0's
            double lambda = 2.0;    // fixed parameter for Poisson distrib.

            // sample data
            int[] data = SampleData(N, pi, lambda);

            // variable for the number of data points (can be skipped)
            Variable<int> numPoints = Variable.Observed<int>(N);

            // range over data points
            Range n = new Range(numPoints);

            // pi and lambda parameter priors
            Beta cPrior = new Beta(1, 1);
            Gamma pPrior = new Gamma(1, 4);

            // RVs for pi and lambda: attach prior distribution
            var probTrue = Variable.Random<double>(cPrior);
            var mean = Variable.Random<double>(pPrior);

            // array of integer RVs
            var x = Variable.Array<int>(n);

            // loop over every data point
            using (Variable.ForEach(n))
            {
                // toss the coin
                var c = Variable.Bernoulli(probTrue);

                // set x[n] = 0 if heads
                using (Variable.If(c))
                    x[n] = 0;

                // set x[n] to Poisson if tails
                using (Variable.IfNot(c))
                    x[n] = Variable.Poisson(mean);
            }

            // hook up the observations
            x.ObservedValue = data;

            // inference engine instance (can pick different algos; default EP)
            InferenceEngine engine = new InferenceEngine();

            // infer posteriors for pi and lambda
            Beta probTrueMarginal = engine.Infer<Beta>(probTrue);
            Gamma meanMarginal = engine.Infer<Gamma>(mean);

            // print means of the pi and lambda distributions
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
