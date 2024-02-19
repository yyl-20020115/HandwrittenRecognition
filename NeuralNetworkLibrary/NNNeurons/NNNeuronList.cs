using System.Collections.Generic;
using ArchiveSerialization;
namespace NeuralNetworkLibrary;

public class NNNeuronList : List<NNNeuron>, IArchiveSerialization 
{
    public NNNeuronList()
    { 
    
    }
    public NNNeuronList(int capacity)
        : base(capacity)
    {
        
    }
    public NNNeuronList(IEnumerable<NNNeuron> collection)
        : base(collection)
    {
        
    }
    public void Serialize(Archive ar)
    {

    }
}
