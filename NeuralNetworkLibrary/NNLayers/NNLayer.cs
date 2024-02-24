﻿using System;
using ArchiveSerialization;
using System.Threading;
namespace NeuralNetworkLibrary;

// Layer class
public class NNLayer : IArchiveSerialization
{
    public NNWeightList Weights { get; private set; }
    public NNNeuronList Neurons { get; private set; }

    public string Label;
    public NNLayer PreviousLayer;
    private readonly SigmoidFunction Sigmoid;
    bool IsFloatingPointWarning;  // flag for one-time warning (per layer) about potential floating point overflow
    public NNLayer()
    {
        Label = string.Empty;
        PreviousLayer = null;
        Sigmoid = new ();
        Weights = [];
        Neurons = [];
        Initialize();

    }
    public NNLayer(string str, NNLayer pPrev /* =NULL */)
    {
        Label = str;
        PreviousLayer = pPrev;
        Sigmoid = new ();
        Weights = [];
        Neurons = [];
    }
    public void Initialize()
    {
        Weights.Clear();
        Neurons.Clear();
        IsFloatingPointWarning = false;
    }
    public void Calculate()
    {
        var dSum = 0.0;
        foreach (var nit in Neurons)
        {
            foreach (var cit in nit.Connections)
            {
                if (cit == nit.Connections[0])
                {
                    dSum = (Weights[(int)cit.WeightIndex].Value);
                }
                else
                {

                    dSum += (Weights[(int)cit.WeightIndex].Value) *
                        (PreviousLayer.Neurons[(int)cit.NeuronIndex].Output);
                }
            }

            nit.Output = Sigmoid.SIGMOID(dSum);
        }
    }
    /////////////
    public bool Backpropagate(DErrorsList dErr_wrt_dXn /* in */,
                        DErrorsList dErr_wrt_dXnm1 /* out */,
                        NNNeuronOutputs thisLayerOutput,  // memorized values of this layer's output
                        NNNeuronOutputs prevLayerOutput,  // memorized values of previous layer's output
                        double etaLearningRate)
    {
        // nomenclature (repeated from NeuralNetwork class):
        //
        // Err is output error of the entire neural net
        // Xn is the output vector on the n-th layer
        // Xnm1 is the output vector of the previous layer
        // Wn is the vector of weights of the n-th layer
        // Yn is the activation value of the n-th layer, i.e., the weighted sum of inputs BEFORE the squashing function is applied
        // F is the squashing function: Xn = F(Yn)
        // F' is the derivative of the squashing function
        //   Conveniently, for F = tanh, then F'(Yn) = 1 - Xn^2, i.e., the derivative can be calculated from the output, without knowledge of the input
        try
        {
            int ii, jj;
            uint idx;
            int nIndex;
            double output;
            var dErr_wrt_dYn = new DErrorsList(Neurons.Count);
            //
            //	std::vector< double > dErr_wrt_dWn( m_Weights.size(), 0.0 );  // important to initialize to zero
            //////////////////////////////////////////////////
            //
            ///// DESIGN TRADEOFF: REVIEW !!
            // We would prefer (for ease of coding) to use STL vector for the array "dErr_wrt_dWn", which is the 
            // differential of the current pattern's error wrt weights in the layer.  However, for layers with
            // many weights, such as fully-connected layers, there are also many weights.  The STL vector
            // class's allocator is remarkably stupid when allocating large memory chunks, and causes a remarkable 
            // number of page faults, with a consequent slowing of the application's overall execution time.

            // To fix this, I tried using a plain-old C array, by new'ing the needed space from the heap, and 
            // delete[]'ing it at the end of the function.  However, this caused the same number of page-fault
            // errors, and did not improve performance.

            // So I tried a plain-old C array allocated on the stack (i.e., not the heap).  Of course I could not
            // write a statement like 
            //    double dErr_wrt_dWn[ m_Weights.size() ];
            // since the compiler insists upon a compile-time known constant value for the size of the array.  
            // To avoid this requirement, I used the _alloca function, to allocate memory on the stack.
            // The downside of this is excessive stack usage, and there might be stack overflow probelms.  That's why
            // this comment is labeled "REVIEW"
            var dErr_wrt_dWn = new double[Weights.Count];
            for (ii = 0; ii < Weights.Count; ii++)
            {
                dErr_wrt_dWn[ii] = 0.0;
            }

            var bMemorized = (thisLayerOutput != null) && (prevLayerOutput != null);
            // calculate dErr_wrt_dYn = F'(Yn) * dErr_wrt_Xn

            for (ii = 0; ii < Neurons.Count; ii++)
            {
                output = bMemorized ? thisLayerOutput[ii] : Neurons[ii].Output;

                dErr_wrt_dYn.Add(Sigmoid.DSIGMOID(output) * dErr_wrt_dXn[ii]);
            }

            // calculate dErr_wrt_Wn = Xnm1 * dErr_wrt_Yn
            // For each neuron in this layer, go through the list of connections from the prior layer, and
            // update the differential for the corresponding weight

            ii = 0;
            foreach (var nit in Neurons)
            {
                foreach (var cit in nit.Connections)
                {
                    idx = cit.NeuronIndex;
                    if (idx == 0xffffffff)
                    {
                        output = 1.0;  // this is the bias weight
                    }
                    else
                    {
                        if (bMemorized != false)
                        {
                            output = prevLayerOutput[(int)idx];
                        }
                        else
                        {
                            output = PreviousLayer.Neurons[(int)idx].Output;
                        }
                    }
                    dErr_wrt_dWn[cit.WeightIndex] += dErr_wrt_dYn[ii] * output;
                }

                ii++;
            }
            // calculate dErr_wrt_Xnm1 = Wn * dErr_wrt_dYn, which is needed as the input value of
            // dErr_wrt_Xn for backpropagation of the next (i.e., previous) layer
            // For each neuron in this layer

            ii = 0;
            foreach (var nit in Neurons)
            {
                foreach (var cit in nit.Connections)
                {
                    idx = cit.NeuronIndex;
                    if (idx != 0xffffffff)
                    {
                        // we exclude ULONG_MAX, which signifies the phantom bias neuron with
                        // constant output of "1", since we cannot train the bias neuron

                        nIndex = (int)idx;
                        dErr_wrt_dXnm1[nIndex] += dErr_wrt_dYn[ii] * Weights[(int)cit.WeightIndex].Value;
                    }

                }

                ii++;  // ii tracks the neuron iterator

            }
            // finally, update the weights of this layer neuron using dErr_wrt_dW and the learning rate eta
            // Use an atomic compare-and-exchange operation, which means that another thread might be in 
            // the process of backpropagation and the weights might have shifted slightly
            const double dMicron = 0.10;
            double epsilon, divisor;
            double oldValue;
            double newValue;
            for (jj = 0; jj < Weights.Count; ++jj)
            {
                divisor = Weights[jj].DiagHessian + dMicron;

                // the following code has been rendered unnecessary, since the value of the Hessian has been
                // verified when it was created, so as to ensure that it is strictly
                // zero-positve.  Thus, it is impossible for the diagHessian to be less than zero,
                // and it is impossible for the divisor to be less than dMicron
                /*
                if ( divisor < dMicron )  
                {
                // it should not be possible to reach here, since everything in the second derviative equations 
                // is strictly zero-positive, and thus "divisor" should definitely be as large as MICRON.
		
                  ASSERT( divisor >= dMicron );
                  divisor = 1.0 ;  // this will limit the size of the update to the same as the size of gloabal eta
                  }
                */
                epsilon = etaLearningRate / divisor;
                oldValue = Weights[jj].Value;
                newValue = oldValue - epsilon * dErr_wrt_dWn[jj];
                while (oldValue != Interlocked.CompareExchange(
                       ref (Weights[jj].Value),
                        (double)newValue, (double)oldValue))
                {
                    // another thread must have modified the weight.

                    // Obtain its new value, adjust it, and try again

                    oldValue = Weights[jj].Value;
                    newValue = oldValue - epsilon * dErr_wrt_dWn[jj];
                }
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


    public void PeriodicWeightSanityCheck()
    {
        // called periodically by the neural net, to request a check on the "reasonableness" of the 
        // weights.  The warning message is given only once per layer
        foreach (var wit in Weights)
        {

            double val = Math.Abs(wit.Value);

            if ((val > 100.0) && (IsFloatingPointWarning == false))
            {
                // 100.0 is an arbitrary value, that no reasonable weight should ever exceed
                /*
                string strMess = ""; ;
                strMess.Format("Caution: Weights are becoming unboundedly large \n"+
                    "Layer: %s \nWeight: %s \nWeight value = %g \nWeight Hessian = %g\n\n"+
                     "Suggest abandoning this backpropagation and investigating",
                    label, wit.label, wit.value, wit.diagHessian );
                //show message box
                //MessageBox.show( NULL, strMess, _T( "Problem With Weights" ), MB_ICONEXCLAMATION | MB_OK );
                */
                IsFloatingPointWarning = true;
            }
        }
    }


    public void EraseHessianInformation()
    {
        // goes through all the weights associated with this layer, and sets each of their
        // diagHessian value to zero

        foreach (var wit in Weights)
        {
            wit.DiagHessian = 0.0;
        }

    }

    public void DivideHessianInformationBy(double divisor)
    {
        // goes through all the weights associated with this layer, and divides each of their
        // diagHessian value by the indicated divisor
        foreach (var wit in Weights)
        {
            var d = wit.DiagHessian;

            if (d < 0.0)
            {
                // it should not be possible to reach here, since all calculations for the second
                // derviative are strictly zero-positive.  However, there are some early indications 
                // that this check is necessary anyway
                d = 0.0;
            }

            wit.DiagHessian = d / divisor;
        }
    }
    public void BackpropagateSecondDerivatives(DErrorsList d2Err_wrt_dXn /* in */,
                                                DErrorsList d2Err_wrt_dXnm1 /* out */)
    {
        // nomenclature (repeated from NeuralNetwork class)
        // NOTE: even though we are addressing SECOND derivatives ( and not first derivatives),
        // we use nearly the same notation as if there were first derivatives, since otherwise the
        // ASCII look would be confusing.  We add one "2" but not two "2's", such as "d2Err_wrt_dXn",
        // to give a gentle emphasis that we are using second derivatives
        //
        // Err is output error of the entire neural net
        // Xn is the output vector on the n-th layer
        // Xnm1 is the output vector of the previous layer
        // Wn is the vector of weights of the n-th layer
        // Yn is the activation value of the n-th layer, i.e., the weighted sum of inputs BEFORE the squashing function is applied
        // F is the squashing function: Xn = F(Yn)
        // F' is the derivative of the squashing function
        //   Conveniently, for F = tanh, then F'(Yn) = 1 - Xn^2, i.e., the derivative can be calculated from the output, without knowledge of the input 

        int ii, jj;
        uint kk;
        int nIndex;
        double output;
        double dTemp;

        var d2Err_wrt_dYn = new DErrorsList(Neurons.Count);
        //
        // std::vector< double > d2Err_wrt_dWn( m_Weights.size(), 0.0 );  // important to initialize to zero
        //////////////////////////////////////////////////
        //
        ///// DESIGN TRADEOFF: REVIEW !!
        //
        // Note that the reasoning of this comment is identical to that in the NNLayer::Backpropagate() 
        // function, from which the instant BackpropagateSecondDerivatives() function is derived from
        //
        // We would prefer (for ease of coding) to use STL vector for the array "d2Err_wrt_dWn", which is the 
        // second differential of the current pattern's error wrt weights in the layer.  However, for layers with
        // many weights, such as fully-connected layers, there are also many weights.  The STL vector
        // class's allocator is remarkably stupid when allocating large memory chunks, and causes a remarkable 
        // number of page faults, with a consequent slowing of the application's overall execution time.

        // To fix this, I tried using a plain-old C array, by new'ing the needed space from the heap, and 
        // delete[]'ing it at the end of the function.  However, this caused the same number of page-fault
        // errors, and did not improve performance.

        // So I tried a plain-old C array allocated on the stack (i.e., not the heap).  Of course I could not
        // write a statement like 
        //    double d2Err_wrt_dWn[ m_Weights.size() ];
        // since the compiler insists upon a compile-time known constant value for the size of the array.  
        // To avoid this requirement, I used the _alloca function, to allocate memory on the stack.
        // The downside of this is excessive stack usage, and there might be stack overflow probelms.  That's why
        // this comment is labeled "REVIEW"

        double[] d2Err_wrt_dWn = new double[Weights.Count];
        for (ii = 0; ii < Weights.Count; ii++)
        {
            d2Err_wrt_dWn[ii] = 0.0;
        }
        // calculate d2Err_wrt_dYn = ( F'(Yn) )^2 * dErr_wrt_Xn (where dErr_wrt_Xn is actually a second derivative )

        for (ii = 0; ii < Neurons.Count; ii++)
        {

            output = Neurons[ii].Output;
            dTemp = Sigmoid.DSIGMOID(output);
            d2Err_wrt_dYn.Add(d2Err_wrt_dXn[ii] * dTemp * dTemp);
        }
        // calculate d2Err_wrt_Wn = ( Xnm1 )^2 * d2Err_wrt_Yn (where dE2rr_wrt_Yn is actually a second derivative)
        // For each neuron in this layer, go through the list of connections from the prior layer, and
        // update the differential for the corresponding weight

        ii = 0;
        foreach (NNNeuron nit in Neurons)
        {
            foreach (NNConnection cit in nit.Connections)
            {
                try
                {

                    kk = (uint)cit.NeuronIndex;
                    if (kk == 0xffffffff)
                    {
                        output = 1.0;  // this is the bias connection; implied neuron output of "1"
                    }
                    else
                    {
                        output = PreviousLayer.Neurons[(int)kk].Output;
                    }

                    ////////////	ASSERT( (*cit).WeightIndex < d2Err_wrt_dWn.size() );  // since after changing d2Err_wrt_dWn to a C-style array, the size() function this won't work
                    //d2Err_wrt_dWn[cit.WeightIndex] += d2Err_wrt_dYn[ii] * output * output;
                    d2Err_wrt_dWn[cit.WeightIndex] = d2Err_wrt_dYn[ii] * output * output;
                }
                catch (Exception)
                {

                }
            }

            ii++;
        }
        // calculate d2Err_wrt_Xnm1 = ( Wn )^2 * d2Err_wrt_dYn (where d2Err_wrt_dYn is a second derivative not a first).
        // d2Err_wrt_Xnm1 is needed as the input value of
        // d2Err_wrt_Xn for backpropagation of second derivatives for the next (i.e., previous spatially) layer
        // For each neuron in this layer

        ii = 0;
        foreach (var nit in Neurons)
        {
            foreach (var cit in nit.Connections)
            {
                try
                {
                    kk = cit.NeuronIndex;
                    if (kk != 0xffffffff)
                    {
                        // we exclude ULONG_MAX, which signifies the phantom bias neuron with
                        // constant output of "1", since we cannot train the bias neuron

                        nIndex = (int)kk;
                        dTemp = Weights[(int)cit.WeightIndex].Value;
                        d2Err_wrt_dXnm1[nIndex] += d2Err_wrt_dYn[ii] * dTemp * dTemp;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }

            ii++;  // ii tracks the neuron iterator

        }

        // finally, update the diagonal Hessians for the weights of this layer neuron using dErr_wrt_dW.
        // By design, this function (and its iteration over many (approx 500 patterns) is called while a 
        // single thread has locked the nueral network, so there is no possibility that another
        // thread might change the value of the Hessian.  Nevertheless, since it's easy to do, we
        // use an atomic compare-and-exchange operation, which means that another thread might be in 
        // the process of backpropagation of second derivatives and the Hessians might have shifted slightly

        for (jj = 0; jj < Weights.Count; jj++)
        {
            var oldValue = Weights[jj].DiagHessian;
            var newValue = oldValue + d2Err_wrt_dWn[jj];
            Weights[jj].DiagHessian = newValue;
        }
    }
    virtual public void Serialize(Archive ar)
    {
        int ii, jj;

        if (ar.IsStoring)
        {
            // TODO: add storing code here
            // TODO: add storing code here

            ar.Write(Label);
            //ar.WriteString(_T("\r\n"));  // ar.ReadString will look for \r\n when loading from the archive
            ar.Write(Neurons.Count);
            ar.Write(Weights.Count);

            foreach (var nit in Neurons)
            {
                ar.Write(nit.Label);
                ar.Write(nit.Connections.Count);

                foreach (var cit in nit.Connections)
                {
                    ar.Write(cit.NeuronIndex);
                    ar.Write(cit.WeightIndex);
                }
            }

            foreach (var wit in Weights)
            {
                ar.Write(wit.Label);
                ar.Write(wit.Value);
            }
        }
        else
        {
            // TODO: add loading code here

            //Read Layter's label
            ar.Read(out string label);
            this.Label = label;

            NNNeuron pNeuron;
            NNWeight pWeight;

            //Read No of Neuron, Weight
            ar.Read(out int iNumNeurons);
            ar.Read(out int iNumWeights);
            if (iNumNeurons != 0)
            {
                //clear neuron list and weight list.
                Neurons.Clear();
                Neurons = new (iNumNeurons);
                Weights.Clear();
                Weights = new (iNumWeights);

                for (ii = 0; ii < iNumNeurons; ii++)
                {
                    //ar.Read Neuron's label
                    ar.Read(out label);
                    //Read Neuron's Connection number
                    ar.Read(out int iNumConnections);
                    pNeuron = new NNNeuron(label, iNumConnections);
                    //pNeuron.Label = str;
                    Neurons.Add(pNeuron);
                    for (jj = 0; jj < iNumConnections; jj++)
                    {
                        var conn = new NNConnection();
                        ar.Read(out conn.NeuronIndex);
                        ar.Read(out conn.WeightIndex);
                        pNeuron.AddConnection(conn);
                    }
                }

                for (jj = 0; jj < iNumWeights; jj++)
                {
                    ar.Read(out label);
                    ar.Read(out double value);

                    pWeight = new NNWeight(label, value);
                    Weights.Add(pWeight);
                }
            }
        }
    }
}
