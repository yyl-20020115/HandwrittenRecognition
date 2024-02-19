using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Connection class

public class NNConnection : IArchiveSerialization
{
    public uint NeuronIndex;
    public uint WeightIndex;
    public NNConnection()
    {
        NeuronIndex = 0xffffffff;
        WeightIndex = 0xffffffff;

    }
    public NNConnection(uint iNeuron, uint iWeight)
    {
        NeuronIndex = iNeuron;
        WeightIndex = iWeight;
    }

    public void Serialize(Archive ar)
    {

    }
}
