using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Connection class

public class NNConnection(uint iNeuron = 0xffffffff, uint iWeight = 0xffffffff) : IArchiveSerialization
{
    public uint NeuronIndex = iNeuron;
    public uint WeightIndex = iWeight;

    public void Serialize(Archive ar) { }
}
