﻿using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Neuron class
public class NNNeuron : IArchiveSerialization
{
    public readonly string Label;
    public double Output;
    public readonly NNConnectionList Connections;
    public NNNeuron()
    {
        Initialize();
        Label = "";
        Output = 0.0;
        Connections = [];
    }
    public NNNeuron(string str)
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
        Connections = new NNConnectionList(icount);
        Initialize();
    }
    public void AddConnection(uint iNeuron, uint iWeight)
    {
        NNConnection conn = new(iNeuron, iWeight);
        Connections.Add(conn);

    }
    public void AddConnection(NNConnection conn)
    {
        Connections.Add(conn);
    }
    private void Initialize()
    {

    }
    public virtual void Serialize(Archive ar)
    {

    }
}
