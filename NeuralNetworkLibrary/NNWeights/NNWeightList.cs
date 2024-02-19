using System.Collections.Generic;
using ArchiveSerialization;
namespace NeuralNetworkLibrary;

public class NNWeightList : List<NNWeight>, IArchiveSerialization 
{
    public NNWeightList()
    { 
        
    }
    public NNWeightList(int capacity)
        : base(capacity)
    {
        
    }
    public NNWeightList(IEnumerable<NNWeight> collection)
        : base(collection)
    {
        
    }
     
    public void Serialize(Archive ar)
    {

    }
}
