using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace OM2;

public abstract class MethodND
{
   public delegate double LinearSearch((double, double) interval, Vector x, Func<Vector, double> f, Vector direction, ref int funcCalc);

   protected LinearSearch LinearMethod;
   protected double eps;
   protected int maxIter;
   protected Func<Vector, double> func;
   protected IFunc Func;
   protected Vector x0;
   protected int funcCalc;
   protected string path = "results ";

   public MethodND(double eps, int maxIter, IFunc func, Vector x0, LinearSearch method)
   {
      this.eps = eps;
      this.maxIter = maxIter;
      this.func = func.Func;
      this.x0 = x0;
      funcCalc = 0;
      this.LinearMethod = method;
      Func = func;
   }

   public abstract Vector Compute();

   public Vector Gradient(Vector x, Func<Vector, double> func)
   {
      Vector grad = new(2);
      double fx = func(x);
      funcCalc++;
      double h = 1e-14;

      for (int i = 0; i < x.Length; i++)
      {
         grad[i] = (func(x + new Vector(h * ((i + 1) % 2), h * (i % 2))) - fx) / h;
         funcCalc++;
      }

      return grad;
   }

   public static void Issledovanie(List<Vector> coords, List<double> funcs, int iters, int funcCalc, double eps, string name)
   {
      StreamWriter sw = new("Issledovanie.csv", true);
      sw.WriteLine($"{name};{coords[0].ToString()}; {eps}; {iters}; {funcCalc}; {coords[^1].ToString()}; {funcs[^1]}");
      sw.Close();
   }
}


public class MSG : MethodND
{

   public MSG(double eps, int maxIter, IFunc func, Vector x0, LinearSearch method) : base(eps, maxIter, func, x0, method)
   {

   }

   public override Vector Compute()
   {
      List<Vector> coords = new(); // Координаты для графики.
      List<double> funcs = new(); // Значения функции.
      List<Vector> dirs = new(); // Значения направлений поиска.
      List<double> lambdas = new();
      List<double> corner = new(); // Угол между векторами (xi, yi) и (s1, s2)
      List<Vector> gradfs = new();

      Vector S = new(2), gradient = new(2), gradient0;

      int iter;
      double omegaK = 0, lambdaK;
      for (iter = 0; iter < maxIter;)
      {
         coords.Add(1 * x0);
         funcs.Add(func(x0));
         gradient0 = Gradient(x0, func);
         if (iter % 3 == 0) S = -1 * gradient0;
         else S = -1 * gradient + omegaK * S;

         gradfs.Add(gradient0);
         dirs.Add(S);

         if (S.Norm() < eps) break;

         omegaK = 1 / Math.Pow(S.Norm(), 2);

         var interval = Methods1D.FindInterval(x0, func, S, ref funcCalc);
         lambdaK = LinearMethod(interval, x0, func, S, ref funcCalc);

         lambdas.Add(lambdaK);

         var x = x0 + lambdaK * S;

         gradient = Gradient(x, func);

         omegaK *= Math.Pow(gradient.Norm(), 2);

         Vector.Copy(x, x0);

         iter++;
      }

      for (int i = 0; i < iter; i++)
      {
         double crn =
            Math.Acos
            (
             (coords[i][0] * dirs[i][0] + coords[i][1] * dirs[i][1])
             / (coords[i].Norm() * dirs[i].Norm())
            );
         corner.Add(crn);
      }

      //Output(coords, funcs, dirs, corner, iter, funcCalc, lambdas, gradfs);
      Issledovanie(coords, funcs, iter, funcCalc, eps, Name());

      return x0;
   }

   public string Name()
   {
      string name = "MSG";

      if (LinearMethod == Methods1D.Fibonacci)
         name += "(Fibonacci)";
      else
         name += "(Parabols)";

      name += Func.GetType().ToString();

      return name;
   }

   private void Output(List<Vector> coords, List<double> funcs, List<Vector> dirs, List<double> corners, int iters, int funcCalc, List<double> lambdas, List<Vector> gradfs)
   {
      StreamWriter sw = new(path + Name() + ".csv");
      sw.WriteLine("i; Xi; f(Xi); S; Lambda; |Xi - Xi-1|; |Yi - Yi-1|; |fi - fi-1|; Angel above X and S; Gradient");

      for (int i = 0; i <= iters; i++)
      {
         if (i > 0)
            sw.WriteLine($"{i}; {coords[i].ToString()}; {funcs[i]:F6}; {dirs[i].ToString()}; {lambdas[i - 1]:F6}; {Math.Abs(coords[i][0] - coords[i - 1][0]):F6}; " +
               $"{Math.Abs(coords[i][1] - coords[i - 1][1]):F6}; {Math.Abs(funcs[i] - funcs[i - 1]):F6}; {corners[i - 1]:F6}; {gradfs[i].ToString()}");
         if (i == 0)
            sw.WriteLine($"{i}; {coords[i].ToString()}; {funcs[i]:F6}; {dirs[i].ToString()}; ; ; ; ; ; {gradfs[i].ToString()}");


      }

      sw.Close();
   }

}

public class Broyden : MethodND
{
   public Broyden(double eps, int maxIter, IFunc func, Vector x0, LinearSearch method) : base(eps, maxIter, func, x0, method)
   {

   }

   public override Vector Compute()
   {

      List<Vector> coords = new(); // Координаты для графики.
      List<double> funcs = new(); // Значения функции.
      List<Vector> dirs = new(); // Значения направлений поиска.
      List<double> lambdas = new();
      List<double> corner = new(); // Угол между векторами (xi, yi) и (s1, s2)
      List<Vector> gradfs = new();
      List<Matrix> matrices = new();

      int iter;
      double omegaK, lambdaK;
      Matrix H = new(2);
      Matrix deltaH = new(2);

      H[0, 0] = 1;
      H[1, 1] = 1;

      for (iter = 0; iter < maxIter; iter++)
      {
         coords.Add(1 * x0);

         if (iter % 2 == 0 && iter != 0)
         {
            H.Clear();
            H[0, 0] = 1;
            H[1, 1] = 1;
         }

         var gradient0 = Gradient(x0, func);

         funcs.Add(func(x0));
         gradfs.Add(gradient0);
         matrices.Add(H);

         if (gradient0.Norm() < eps)
            break;

         var S = -1 * H * gradient0;

         var interval = Methods1D.FindInterval(x0, func, S, ref funcCalc);
         lambdaK = LinearMethod(interval, x0, func, S, ref funcCalc);

         var x1 = x0 + lambdaK * S;

         dirs.Add(S);
         lambdas.Add(lambdaK);

         var gradient1 = Gradient(x1, func);

         var deltaGrad = gradient1 - gradient0;
         var deltaX = x1 - x0;

         var denominatorAsVec = deltaX - H * deltaGrad;
         var denominator = denominatorAsVec * deltaGrad;

         for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
               deltaH[i, j] = denominatorAsVec[i] * denominatorAsVec[j];

         deltaH = 1 / denominator * deltaH;

         H += deltaH;

         Vector.Copy(x1, x0);
      }

      for (int i = 0; i < iter; i++)
      {
         double crn =
            Math.Acos
            (
             (coords[i][0] * dirs[i][0] + coords[i][1] * dirs[i][1])
             / (coords[i].Norm() * dirs[i].Norm())
            );
         corner.Add(crn);
      }

      //Output(coords, funcs, dirs, corner, iter, funcCalc, lambdas, gradfs, matrices);
      Issledovanie(coords, funcs, iter, funcCalc, eps, Name());

      return x0;
   }


   public string Name()
   {
      string name = "Broyden";

      if (LinearMethod == Methods1D.Fibonacci)
         name += "(Fibonacci)";
      else
         name += "(Parabols)";

      name += Func.GetType().ToString();
      return name;
   }

   private void Output(List<Vector> coords, List<double> funcs, List<Vector> dirs, List<double> corners, int iters, int funcCalc, List<double> lambdas, List<Vector> gradfs, List<Matrix> matrices)
   {
      StreamWriter sw = new(path + Name() + ".csv");
      sw.WriteLine("i; Xi; f(Xi); S; Lambda; |Xi - Xi-1|; |Yi - Yi-1|; |fi - fi-1|; Angel above X and S; Gradient; Eta 1 stroka; Eta 2 stroka");

      for (int i = 0; i <= iters; i++)
      {
         if (i > 0 && i < iters)
            sw.WriteLine($"{i}; {coords[i].ToString()}; {funcs[i]:F6}; {dirs[i].ToString()}; {lambdas[i]:F6}; {Math.Abs(coords[i][0] - coords[i - 1][0]):F6}; " +
               $"{Math.Abs(coords[i][1] - coords[i - 1][1]):F6}; {Math.Abs(funcs[i] - funcs[i - 1]):F6}; {corners[i - 1]:F6}; {gradfs[i].ToString()};" +
               $" {matrices[i][0, 0]:F6} {matrices[i][0, 1]:F6}; {matrices[i][1, 0]:F6} {matrices[i][1, 1]:F6}");
         if (i == 0)
            sw.WriteLine($"{i}; {coords[i].ToString()}; {funcs[i]:F6}; {dirs[i].ToString()}; {lambdas[i]:F6}; ; ; ; ; {gradfs[i].ToString()};" +
               $" {matrices[i][0,0]:F6} {matrices[i][0, 1]:F6}; {matrices[i][1, 0]:F6} {matrices[i][1, 1]:F6}");
         if (i == iters && iters < 100)
            sw.WriteLine($"{i}; {coords[i].ToString()}; {funcs[i]:F6}; ; ; {Math.Abs(coords[i][0] - coords[i - 1][0]):F6}; " +
            $"{Math.Abs(coords[i][1] - coords[i - 1][1]):F6}; {Math.Abs(funcs[i] - funcs[i - 1]):F6}; {corners[i - 1]:F6}; {gradfs[i].ToString()}");


      }

      sw.Close();
   }
}
