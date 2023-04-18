namespace OM2;

public interface IFunc
{
   public double Func(Vector x);
}

public class QuadraticFunc : IFunc
{
   public double Func(Vector x)
      => 100 * Math.Pow(x[1] - x[0], 2) + Math.Pow(1 - x[0], 2);
}

public class RosenbrokeFunc : IFunc
{
   public double Func(Vector x)
      => 100 * Math.Pow(x[1] - Math.Pow(x[0], 2), 2) + Math.Pow(1 - x[0], 2);
}

public class VariantFunc : IFunc
{
   public double Func(Vector x)
      => -(3 / (1 + Math.Pow((x[0] - 3) / 2, 2) + Math.Pow((x[1] - 2) / 1, 2)) + 1 / (1 + Math.Pow((x[0] - 3) / 1, 2) + Math.Pow((x[1] - 1) / 3, 2)));
}