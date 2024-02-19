using System.Collections.Generic;


namespace NeuralNetworkLibrary;

public class NNNeuronOutputs:List<double>
{
    public NNNeuronOutputs()
    { 
    
    }
    public NNNeuronOutputs(int capacity)
        : base(capacity)
    {
        
    }
    public NNNeuronOutputs(IEnumerable<double> collection)
        : base(collection)
    {
        
    }
     
}
