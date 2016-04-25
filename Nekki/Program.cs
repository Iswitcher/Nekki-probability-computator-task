using System;
using System.Collections.Generic;
using System.Linq;


namespace Nekki
{
    class Program
    {
        static string[] colors = new string[] { "Ire", "Red", "Ora", "Yel", "Gre", "Blu", "Vio", "Uvi" };
        static string[] oppositeColors = new string[] { "Uvi", "Vio", "Blu", "Gre", "Yel", "Ora", "Red", "Ire" };
        static double[] gateProbs = new double[] { 0.09, 0.10, 0.11, 0.12, 0.13, 0.14, 0.15, 0.16 };
        static double[] ballProbs = new double[] { 0.8, 0.75, 0.7, 0.65, 0.8, 0.75, 0.7, 0.65 };
        static double i = 0;
        

        static void Main(string[] args)
        {
            FillOriginalGates();
            FillOriginalBalls();

            ComputeOriginalProbability(Lists.ballsOriginal);
            ComputeGeneralProbability(Lists.ballsOriginal);

            Console.WriteLine("===================");
            PrintBalls(Lists.ballsOriginal);

            Lists.ballsComputed = Lists.ballsOriginal;
            while (true == RecomputeProbability())
            { RecomputeProbability(); }


            Console.WriteLine("====Balls probabilities computed====");
            PrintBalls(Lists.ballsComputed);

            Console.WriteLine("Number of iterations to compute: " + i);

            Console.WriteLine("Press Enter to exit the app.");
            Console.ReadLine();
        }

        static void FillOriginalGates()
        {
            for (int i = 0; i < colors.Count(); i++)
            {
                Gate gate = new Gate();
                gate.color = colors[i];
                gate.oppositeColor = oppositeColors[i];
                gate.probability = gateProbs[i];

                Lists.gatesOriginal.Add(gate);

                Console.WriteLine(gate.ToString());
            }
        }

        static void FillOriginalBalls()
        {
            for (int i = 0; i < colors.Count(); i++)
            {
                Ball ball = new Ball();

                ball.color = colors[i];
                ball.nativeColor = colors[i];
                ball.oppositeColor = oppositeColors[i];
                ball.probability = ballProbs[i];
                ball.oppositeProbability = 1 - ballProbs[i];

                Lists.ballsOriginal.Add(ball);

                Console.WriteLine(ball.ToString());
            }
        }

        static void ComputeOriginalProbability(IList<Ball> input)
        {
            foreach (var ball in input)
            {
                var mainBallProbability = input.Where(w => w.color == ball.color).Select(s => s.probability).FirstOrDefault();
                var mainGateProbability = Lists.gatesOriginal.Where(w => w.color == ball.color).Select(s => s.probability).FirstOrDefault();

                var secondaryBallProbability = input.Where(w => w.oppositeColor == ball.color).Select(s => s.oppositeProbability).FirstOrDefault();
                var secondaryGateProbability = Lists.gatesOriginal.Where(w => w.oppositeColor == ball.color).Select(s => s.probability).FirstOrDefault();

                ball.originalprobability = mainBallProbability * mainGateProbability + secondaryBallProbability * secondaryGateProbability;
            }
        }

        static void ComputeGeneralProbability(IList<Ball> input)
        {
            foreach (var ball in input)
            {
                double probability = 0;

                foreach (var row in Lists.ballsOriginal)
                {
                    if (ball.color == row.color)
                    {
                        probability += Lists.gatesOriginal.Where(w => w.color == row.color).Select(s => s.probability).FirstOrDefault() *
                                       input.Where(w => w.color == row.color).Select(s => s.probability).FirstOrDefault();
                    }
                    else
                    {
                        probability += Lists.gatesOriginal.Where(w => w.color == row.color).Select(s => s.probability).FirstOrDefault() *
                                       input.Where(w => w.color == row.color).Select(s => s.oppositeProbability).FirstOrDefault() /
                                       (input.Count - 1 );
                    }
                }

                ball.generalProbability = probability;
            }
        }

        static void PrintBalls(IList<Ball> input)
        {
            foreach (var ball in input)
            {
                Console.WriteLine(ball.ToString());
            }
        }

        static bool RecomputeProbability()
        {
            double minValue = 0;
            double maxValue = 1;
            i++;
            double validFluctuation = 0.0001;

            foreach (var ball in Lists.ballsComputed)
            {
                if (ball.originalprobability > ball.generalProbability && ball.probability < maxValue)
                {
                    ball.probability += validFluctuation;
                    ball.oppositeProbability -= validFluctuation;
                }
                else if (ball.originalprobability < ball.generalProbability && ball.probability > minValue )
                {
                    ball.probability -= validFluctuation;
                    ball.oppositeProbability += validFluctuation;
                }

                //i++;
                ComputeGeneralProbability(Lists.ballsComputed);
            }

            var fluctuation = Lists.ballsComputed.Max(m => Math.Abs(m.originalprobability - m.generalProbability));
            if (fluctuation < validFluctuation)
            {
                return true;
            }
            else
            {
                RecomputeProbability(); return false;
            }
        }

    }

    class Lists
    {
        public static List<Gate> gatesOriginal = new List<Gate>();
        public static List<Ball> ballsOriginal = new List<Ball>();

        public static List<Ball> ballsComputed = new List<Ball>();

    }

    class Ball
    {
        public string color;
        public string nativeColor;
        public string oppositeColor;
        public double probability;
        public double oppositeProbability;
        public double originalprobability;
        public double generalProbability;

        public override string ToString()
        {
            return "BallColor:"+color+ //" | NativeColor:" + nativeColor + " | OppositeColor:" + oppositeColor + 
                   " | Probability:" + probability + " | OppositeProbability:" + oppositeProbability + " | OriginalProbability:" + originalprobability + " | GenetalProbability:" + generalProbability;
        }
    }

    class Gate
    {
        public string color { get; set; }
        public string oppositeColor { get; set; }
        public double probability { get; set; }


        public override string ToString()
        {
            return "GateColor:" + color + " | NativeColor:" + oppositeColor + " | Probability:" + probability;
        }
    }

}
