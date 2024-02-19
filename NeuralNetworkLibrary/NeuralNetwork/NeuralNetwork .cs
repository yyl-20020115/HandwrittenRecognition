using System.Collections.Generic;
using System.Linq;
using ArchiveSerialization;
namespace NeuralNetworkLibrary;

// Neural Network class
public class NeuralNetwork : IArchiveSerialization
{
    	public double m_etaLearningRatePrevious;
    	public double m_etaLearningRate;
        public uint m_cBackprops;  // counter used in connection with Weight sanity check
        public NNLayerList m_Layers;
        public NeuralNetwork()
        {
            m_etaLearningRate = .001;  // arbitrary, so that brand-new NNs can be serialized with a non-ridiculous number
            m_cBackprops = 0;
            m_Layers = new NNLayerList();
        }
    
        public void Calculate(double[] inputVector, int iCount, 
							      double[] outputVector /* =NULL */, int oCount /* =0 */,
							      NNNeuronOutputsList pNeuronOutputs /* =NULL */ )
        {
            var lit = m_Layers.First();
             // first layer is imput layer: directly set outputs of all of its neurons
            // to the input vector
            if (m_Layers.Count > 1)
            {
                
                int count = 0;
                if (iCount != lit.m_Neurons.Count)
                {
                    return;
                }
                foreach (var nit in lit.m_Neurons) 
                {
                    if (count < iCount)
                    {
                        nit.output = inputVector[count];
                        count++;
                    }
                }

            }
            //caculate output of next layers
            for (int i = 1;i<m_Layers.Count; i++)
            {
                m_Layers[i].Calculate();
            }

            // load up output vector with results

            if (outputVector != null)
            {
                lit = m_Layers[m_Layers.Count - 1];

                for (int ii = 0; ii < oCount; ii++)
                {
                    outputVector[ii] = lit.m_Neurons[ii].output;
                }
            }

            // load up neuron output values with results
            if (pNeuronOutputs != null)
            {
                // check for first time use (re-use is expected)
                    pNeuronOutputs.Clear();
                    // it's empty, so allocate memory for its use
                    pNeuronOutputs.Capacity=m_Layers.Count;
                    foreach (NNLayer nnlit in m_Layers)
                    {
                        var layerOut = new NNNeuronOutputs(nnlit.m_Neurons.Count);
                        for (int ii = 0; ii < nnlit.m_Neurons.Count; ++ii)
                        {
                            layerOut.Add(nnlit.m_Neurons[ii].output);
                        }
                        pNeuronOutputs.Add(layerOut);
                    }
          
                
            }
        }
        public void Backpropagate(double[] actualOutput, double[] desiredOutput, int count, NNNeuronOutputsList pMemorizedNeuronOutputs)
        { 
                    // backpropagates through the neural net
	
	            if(( m_Layers.Count >= 2 )==false) // there must be at least two layers in the net
            {
                return;           
            }
	            if ( ( actualOutput == null ) || ( desiredOutput == null ) || ( count >= 256 ) )
		            return;
	
	
	            // check if it's time for a weight sanity check
	
	            m_cBackprops++;
	
	            if ( (m_cBackprops % 10000) == 0 )
	            {
		            // every 10000 backprops
		
		            PeriodicWeightSanityCheck();
	            }
	
	            
	            // proceed from the last layer to the first, iteratively
	            // We calculate the last layer separately, and first, since it provides the needed derviative
	            // (i.e., dErr_wrt_dXnm1) for the previous layers
	
	            // nomenclature:
	            //
	            // Err is output error of the entire neural net
	            // Xn is the output vector on the n-th layer
	            // Xnm1 is the output vector of the previous layer
	            // Wn is the vector of weights of the n-th layer
	            // Yn is the activation value of the n-th layer, i.e., the weighted sum of inputs BEFORE the squashing function is applied
	            // F is the squashing function: Xn = F(Yn)
	            // F' is the derivative of the squashing function
	            //   Conveniently, for F = tanh, then F'(Yn) = 1 - Xn^2, i.e., the derivative can be calculated from the output, without knowledge of the input

           int iSize = m_Layers.Count;
           var dErr_wrt_dXlast = new DErrorsList(m_Layers[m_Layers.Count - 1].m_Neurons.Count);
           var differentials = new List<DErrorsList>(iSize);

           int ii;
	
	            // start the process by calculating dErr_wrt_dXn for the last layer.
	            // for the standard MSE Err function (i.e., 0.5*sumof( (actual-target)^2 ), this differential is simply
	            // the difference between the target and the actual

            for (ii = 0; ii < m_Layers[m_Layers.Count - 1].m_Neurons.Count; ++ii)
	            {
		            dErr_wrt_dXlast.Add(actualOutput[ ii ] - desiredOutput[ ii ]);
	            }
	
	
	            // store Xlast and reserve memory for the remaining vectors stored in differentials
	
	            
	           
	            for ( ii=0; ii<iSize-1; ii++ )
	            {
                var m_differential = new DErrorsList(m_Layers[ii].m_Neurons.Count);
                for (int kk = 0; kk < m_Layers[ii].m_Neurons.Count; kk++)
                {
                    m_differential.Add(0.0);
                }
                differentials.Add(m_differential);
            }
	            differentials.Add(dErr_wrt_dXlast);  // last one
	            // now iterate through all layers including the last but excluding the first, and ask each of
	            // them to backpropagate error and adjust their weights, and to return the differential
	            // dErr_wrt_dXnm1 for use as the input value of dErr_wrt_dXn for the next iterated layer
	
	            bool bMemorized = ( pMemorizedNeuronOutputs != null );
	            for ( int jj=iSize-1; jj>0;jj--)
	            {
		            if ( bMemorized != false )
		            {
			           m_Layers[jj].Backpropagate( differentials[ jj ], differentials[ jj - 1 ], 
				            pMemorizedNeuronOutputs[jj], pMemorizedNeuronOutputs[ jj - 1 ], m_etaLearningRate );
		            }
		            else
		            {
                    m_Layers[jj].Backpropagate(differentials[jj], differentials[jj - 1], 
				            null, null, m_etaLearningRate );
		            }
		
		        
	            }


            differentials.Clear();
        }
        public void EraseHessianInformation()
        {
            foreach (var lit in m_Layers)
            {
                lit.EraseHessianInformation();
            }
        }
        public void DivideHessianInformationBy(double divisor)
        {

            // controls each layer to divide its current diagonal Hessian info by a common divisor. 
            // A check is also made to ensure that each Hessian is strictly zero-positive

            foreach (var lit in m_Layers)
            {
                lit.DivideHessianInformationBy(divisor);
            }
	
        }
        public void BackpropagateSecondDervatives(double[] actualOutputVector, double[] targetOutputVector, uint count)
        { 
            // calculates the second dervatives (for diagonal Hessian) and backpropagates
	            // them through neural net
	
		
	            if( m_Layers.Count< 2 ){return;};  // there must be at least two layers in the net

            if ((actualOutputVector == null) || (targetOutputVector == null) || (count >= 256))
            {
                return;
            }
           
	            // we use nearly the same nomenclature as above (e.g., "dErr_wrt_dXnm1") even though everything here
	            // is actually second derivatives and not first derivatives, since otherwise the ASCII would 
	            // become too confusing.  To emphasize that these are second derivatives, we insert a "2"
	            // such as "d2Err_wrt_dXnm1".  We don't insert the second "2" that's conventional for designating
	            // second derivatives"

            int iSize = m_Layers.Count;
            int neuronCount = m_Layers[m_Layers.Count - 1].m_Neurons.Count;
            var d2Err_wrt_dXlast = new DErrorsList(neuronCount);
            var differentials = new List<DErrorsList>(iSize);
	           
	
	            // start the process by calculating the second derivative dErr_wrt_dXn for the last layer.
	            // for the standard MSE Err function (i.e., 0.5*sumof( (actual-target)^2 ), this differential is 
	            // exactly one

            var lit = m_Layers.Last();  // point to last layer
	
	            for ( int ii=0; ii<lit.m_Neurons.Count; ii++ )
	            {
		            d2Err_wrt_dXlast.Add(1.0);
	            }	
	
	            // store Xlast and reserve memory for the remaining vectors stored in differentials
	
	
	            for ( int ii=0; ii<iSize-1; ii++ )
	            {
                var m_differential = new DErrorsList(m_Layers[ii].m_Neurons.Count);
                for (int kk = 0; kk < m_Layers[ii].m_Neurons.Count; kk++)
                {
                    m_differential.Add(0.0);
                }
                differentials.Add(m_differential);
               
	            }
	
	            differentials.Add(d2Err_wrt_dXlast);  // last one
	
	            // now iterate through all layers including the last but excluding the first, starting from
	            // the last, and ask each of
	            // them to backpropagate the second derviative and accumulate the diagonal Hessian, and also to
	            // return the second dervative
	            // d2Err_wrt_dXnm1 for use as the input value of dErr_wrt_dXn for the next iterated layer (which
	            // is the previous layer spatially)
	         
	            for ( int ii = iSize - 1; ii>0; ii--)
	            {
		            m_Layers[ii].BackpropagateSecondDerivatives( differentials[ ii ], differentials[ ii - 1 ] );
		        }
	
	            differentials.Clear();
        }
        void PeriodicWeightSanityCheck()
        {
            // fucntion that simply goes through all weights, and tests them against an arbitrary
            // "reasonable" upper limit.  If the upper limit is exceeded, a warning is displayed

            foreach (var lit in m_Layers)
            {
                lit.PeriodicWeightSanityCheck();
            }
        }
        virtual public void Serialize(Archive ar)
        { 
            if (ar.IsStoring())
	            {
                
		            // TODO: add storing code here
                ar.Write(m_etaLearningRate);
                ar.Write(m_Layers.Count);
                foreach (var lit in m_Layers)
		            {
			            lit.Serialize( ar );
		            }
		 
	            }
	            else
	            {
		            // TODO: add loading code here
		
		            double eta; 
		            ar.Read(out eta);
		            m_etaLearningRate = eta;  // two-step storage is needed since m_etaLearningRate is "volatile"
		
		            int nLayers;
                var pLayer = (NNLayer)null;

                ar.Read(out nLayers);
                m_Layers.Clear();
                m_Layers = new NNLayerList(nLayers);
		            for ( int ii=0; ii<nLayers; ii++ )
		            {
			            pLayer = new NNLayer( "", pLayer );
			
			            m_Layers.Add(pLayer);
			            pLayer.Serialize( ar );
		            }
		
	            }
        }
}
