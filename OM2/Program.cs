using OM2;
using System.Xml.Linq;

Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

List<IFunc> funcs = new List<IFunc>();
//funcs.Add(new QuadraticFunc());
//funcs.Add(new RosenbrokeFunc());
funcs.Add(new VariantFunc());


foreach (var f in funcs)
{
   //var test = new MSG(1e-3, 100, f, new Vector(4, -7), Methods1D.Fibonacci);
   //var test = new MSG(1e-3, 100, f, new Vector(4, -7), Methods1D.QuadraticSearch);
   var test = new Broyden(1e-3, 100, f, new Vector(-3, 4), Methods1D.Fibonacci);
   //var test = new Broyden(1e-3, 100, f, new Vector(-3, 4), Methods1D.QuadraticSearch);
   Console.WriteLine(test.Compute().ToString());
}

//StreamWriter sw = new("Issledovanie.csv");
//sw.WriteLine($"Method name; X0; eps; Iters; Func calculation; Xi; f(Xi)");
//sw.Close();

//double[] eps = new double[] { 1e-3, 1e-4, 1e-5, 1e-6, 1e-7 };
//MethodND test;
//foreach (var acc in eps)
//{
//   test = new MSG(acc, 100, new QuadraticFunc(), new Vector(-3, 4), Methods1D.Fibonacci);
//   Console.WriteLine(test.Compute().ToString());
//   test = new MSG(acc, 100, new QuadraticFunc(), new Vector(4, -7), Methods1D.Fibonacci);
//   Console.WriteLine(test.Compute().ToString());

//   test = new MSG(acc, 100, new QuadraticFunc(), new Vector(-3, 4), Methods1D.QuadraticSearch);
//   Console.WriteLine(test.Compute().ToString());
//   test = new MSG(acc, 100, new QuadraticFunc(), new Vector(4, -7), Methods1D.QuadraticSearch);
//   Console.WriteLine(test.Compute().ToString());

//   test = new Broyden(acc, 100, new QuadraticFunc(), new Vector(-3, 4), Methods1D.Fibonacci);
//   Console.WriteLine(test.Compute().ToString());
//   test = new Broyden(acc, 100, new QuadraticFunc(), new Vector(4, -7), Methods1D.Fibonacci);
//   Console.WriteLine(test.Compute().ToString());

//   test = new Broyden(acc, 100, new QuadraticFunc(), new Vector(-3, 4), Methods1D.QuadraticSearch);
//   Console.WriteLine(test.Compute().ToString());
//   test = new Broyden(acc, 100, new QuadraticFunc(), new Vector(4, -7), Methods1D.QuadraticSearch);
//   Console.WriteLine(test.Compute().ToString());
//}