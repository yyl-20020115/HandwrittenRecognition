using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Neuron class
public class NNNeuron : IArchiveSerialization
{
    public string label;
    public double output;
    public NNConnectionList m_Connections;
    public NNNeuron()
    {
        Initialize();
        label = "";
        output = 0.0;
        m_Connections = [];
    }
    public NNNeuron(string str)
    {
        label = str;
        output = 0.0;
        m_Connections = [];
        Initialize();
    }
    public NNNeuron(string str,int icount)
    {
        label = str;
        output = 0.0;
        m_Connections = new NNConnectionList(icount);
        Initialize();
    }
    public void AddConnection(uint iNeuron, uint iWeight)
    {
        NNConnection conn = new NNConnection(iNeuron, iWeight);
        m_Connections.Add(conn);

    }
    public void AddConnection(NNConnection conn)
    {
        m_Connections.Add(conn);
    }
    private void Initialize()
    {

    }
    public virtual void Serialize(Archive ar)
    {

    }
}
