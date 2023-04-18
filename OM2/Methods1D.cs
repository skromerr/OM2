namespace OM2;

public static class Methods1D
{
   public static (double, double) FindInterval(Vector x, Func<Vector, double> f, Vector direction, ref int funcCalc)
   {
      double delta = 1e-8;
      double a = 0, h;

      double f1 = f(x), f2 = f(x + delta * direction);
      funcCalc += 2;

      if (f1 == f2) return (a, a + delta);

      if (f1 > f2) h = delta;
      else h = -delta;

      f2 = f(x + (a + h) * direction);
      funcCalc++;

      while (f1 > f2)
      {
         a += h;

         h *= 2;

         f1 = f2;
         f2 = f(x + (a + h) * direction);
         funcCalc++;
      }

      return (Math.Min(a, a + h), Math.Max(a, a + h));
   }

   public static double Fibonacci((double, double) interval, Vector x, Func<Vector, double> f, Vector direction, ref int funcCalc)
   {
      double eps = 1e-7;
      double a, b;
      (a, b) = interval;
      int n = 1;
      while ((b - a) / eps > Fibonacci(n + 2))
         n++;

      double x1, x2, f1, f2;
      x1 = a + Fibonacci(n) / Fibonacci(n + 2) * (b - a);
      x2 = a + Fibonacci(n + 1) / Fibonacci(n + 2) * (b - a);
      f1 = f(x + x1 * direction);
      f2 = f(x + x2 * direction);
      funcCalc += 2;

      for (int iter = 1; iter < n; iter++)
      {
         if (f1 == f2) break;

         if (f1 > f2)
         {
            a = x1;
            x1 = x2;
            x2 = a + Fibonacci(n - iter + 2) / Fibonacci(n - iter + 3) * (b - a);
            f1 = f2;
            f2 = f(x + x2 * direction);
         }

         else
         {
            b = x2;
            x2 = x1;
            x1 = a + Fibonacci(n - iter + 1) / Fibonacci(n - iter + 3) * (b - a);
            f2 = f1;
            f1 = f(x + x1 * direction);
         }

         funcCalc++;
      }

      return (a + b) / 2;
   }

   public static double Fibonacci(int i)
   {
      return (Math.Pow((1 + Math.Sqrt(5)) / 2, i) - Math.Pow((1 - Math.Sqrt(5)) / 2, i)) / Math.Sqrt(5);
   }

   public static double QuadraticSearch((double, double) interval, Vector x, Func<Vector, double> f, Vector direction, ref int funcCalc)
   {
      double eps = 1e-7;
      double a, b;
      (a, b) = interval;

      double lmbd1 = a, lmbd2 = (a + b) / 2, lmbd3 = b;
      double lmbd = 0, lmbdPrev;

      double f1 = f(x + lmbd1 * direction), f2 = f(x + lmbd2 * direction), f3 = f(x + lmbd3 * direction);
      funcCalc += 3;

      while (true)
      {
         var c2 = (f2 - f1) / (lmbd2 - lmbd1);
         var c3 = ((f3 - f1) / (lmbd3 - lmbd1) - (f2 - f1) / (lmbd2 - lmbd1)) / (lmbd3 - lmbd2);

         lmbdPrev = lmbd;
         lmbd = (lmbd1 + lmbd2 - c2 / c3) / 2;

         var fx = f(x + lmbd * direction);
         funcCalc++;

         if (Math.Abs(lmbd - lmbdPrev) < eps) break;

         if (lmbd > lmbd2)
         {
            if (fx > f2)
            {
               lmbd3 = lmbd;
               f3 = fx;
            }
            else
            {
               lmbd1 = lmbd2;
               f1 = f2;
               lmbd2 = lmbd;
               f2 = fx;
            }
         }
         else
         {
            if (fx < f2)
            {
               lmbd3 = lmbd2;
               f3 = f2;
               lmbd2 = lmbd;
               f2 = fx;
            }
            else
            {
               lmbd1 = lmbd;
               f1 = fx;
            }
         }
      }

      return lmbd;
   }
}


