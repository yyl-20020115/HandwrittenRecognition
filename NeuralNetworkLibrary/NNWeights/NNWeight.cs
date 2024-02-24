using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Weight class
public class NNWeight(string label = "", double val = 0.0) : IArchiveSerialization
{
    public readonly string Label = label;
    public double Value = val;
    public double DiagHessian = 0.0;

    public virtual void Serialize(Archive ar) { }
}
