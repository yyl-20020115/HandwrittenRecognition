using System.Collections.Generic;
using ArchiveSerialization;
namespace NeuralNetworkLibrary;

public class NNConnectionList : List<NNConnection>, IArchiveSerialization
{
    public NNConnectionList() { }
    public NNConnectionList(int capacity)
        : base(capacity) { }
    public NNConnectionList(IEnumerable<NNConnection> collection)
        : base(collection) { }

    public void Serialize(Archive ar) { }
}
