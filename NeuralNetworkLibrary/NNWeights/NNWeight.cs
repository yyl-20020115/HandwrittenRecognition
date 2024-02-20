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
        Label = string.Empty;
        Value = 0.0;
        DiagHessian = 0.0;
    }
    public NNWeight(string label, double val = 0.0)
    {
        Label = label;
        Value = val;
        DiagHessian = 0.0;
    }

    public virtual void Serialize(Archive ar)
    {

    }
}
