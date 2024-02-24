using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Neuron class
public class NNNeuron : IArchiveSerialization
{
    public readonly string Label;
    public double Output;
    public readonly NNConnectionList Connections;
    public NNNeuron(string str = "")
    {
        Label = str;
        Output = 0.0;
        Connections = [];
        Initialize();
    }
    public NNNeuron(string str,int icount)
    {
        Label = str;
        Output = 0.0;
        Connections = new (icount);
        Initialize();
    }
    public void AddConnection(uint iNeuron, uint iWeight)
        => AddConnection(new(iNeuron, iWeight));
    public void AddConnection(NNConnection conn)
        => Connections.Add(conn);
    private void Initialize()
    {

    }
    public virtual void Serialize(Archive ar)
    {

    }
}
