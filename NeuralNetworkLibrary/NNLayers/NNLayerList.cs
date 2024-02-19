using System.Collections.Generic;
using ArchiveSerialization;
namespace NeuralNetworkLibrary;

public class NNLayerList : List<NNLayer>, IArchiveSerialization 
{
    public NNLayerList()
    { 
    
    }
    public NNLayerList(int capacity)
        : base(capacity)
    {
        
    }
    public NNLayerList(IEnumerable<NNLayer> collection)
        : base(collection)
    {
        
    }
    public void Serialize(Archive ar)
    {

    }
}
