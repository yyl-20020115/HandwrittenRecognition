using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Weight class
public class NNWeight : IArchiveSerialization
{
    public readonly string Label;
    public double Value;
    public double DiagHessian;
    public NNWeight()
    {
        Label = "";
        Value = 0.0;
        DiagHessian = 0.0;
    }
    public NNWeight(string str, double val = 0.0)
    {
        Label = str;
        Value = val;
        DiagHessian = 0.0;
    }

    public virtual void Serialize(Archive ar)
    {

    }
}
