using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Weight class
public class NNWeight : IArchiveSerialization
{
    public string label;
    public double value;
    public double diagHessian;
    public NNWeight()
    {
        label = "";
        value = 0.0;
        diagHessian = 0.0;
    }
    public NNWeight(string str, double val = 0.0)
    {
        label = str;
        value = val;
        diagHessian = 0.0;
    }

    public virtual void Serialize(Archive ar)
    {

    }
}
