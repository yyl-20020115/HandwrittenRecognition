using System;
using System.Collections.Generic;
using System.Threading;

namespace NeuralNetworkLibrary;

public class NNForwardPropagation
{
    /// <summary>
    /// 
    /// </summary>
    /// 

    // Main thread sets this event to stop worker thread:
    public ManualResetEvent m_EventStop = null;
    // Worker thread sets this event when it is stopped:
    public ManualResetEvent m_EventStopped = null;
    public List<Mutex> m_Mutexs;
    public HiPerfTimer m_HiPerfTime;
    public uint m_nImages;
    public int m_currentPatternIndex;
    public Preferences m_Preferences { get; set; }
    public bool m_bDistortPatterns;
    /// <summary>
    /// 
    /// </summary>
    /// 
    protected bool _bDataReady;
    //backpropagation and training-related members
    protected NeuralNetwork _NN;
    protected double[] m_DispH;  // horiz distortion map array
    protected double[] m_DispV;  // vert distortion map array
    protected int _cCols;  // size of the distortion maps
    protected int _cRows;
    protected int _cCount;
    //double m_GaussianKernel[ GAUSSIAN_FIELD_SIZE ] [ GAUSSIAN_FIELD_SIZE ];
    double[,] _GaussianKernel = new double[DefaultDefinations.GAUSSIAN_FIELD_SIZE, DefaultDefinations.GAUSSIAN_FIELD_SIZE];

    public NeuralNetwork m_NeuralNetwork
    {
        get { return _NN; }
        set
        {
            _NN = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public NNForwardPropagation()
    {
        m_currentPatternIndex = 0;
        _bDataReady = false;
        _NN = null;
        m_EventStop = null;
        m_EventStopped = null;
        m_Mutexs = new List<Mutex>(4);
        m_HiPerfTime = new HiPerfTimer();
        m_nImages = 0;
        // allocate memory to store the distortion maps

        _cCols = 29;
        _cRows = 29;

        _cCount = _cCols * _cRows;

        m_DispH = new double[_cCount];
        m_DispV = new double[_cCount];


    }
    protected void GetGaussianKernel(double _dElasticSigma)
    {
        // create a gaussian kernel, which is constant, for use in generating elastic distortions

        int iiMid = 21 / 2;  // GAUSSIAN_FIELD_SIZE is strictly odd

        double twoSigmaSquared = 2.0 * (_dElasticSigma) * (_dElasticSigma);
        twoSigmaSquared = 1.0 / twoSigmaSquared;
        double twoPiSigma = 1.0 / (_dElasticSigma) * Math.Sqrt(2.0 * 3.1415926535897932384626433832795);

        for (int col = 0; col < 21; ++col)
        {
            for (int row = 0; row < 21; ++row)
            {
                _GaussianKernel[row, col] = twoPiSigma *
                    (Math.Exp(-(((row - iiMid) * (row - iiMid) + (col - iiMid) * (col - iiMid)) * twoSigmaSquared)));
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public double GetCurrentEta()
    {
        if (_NN != null)
        {
            return _NN.m_etaLearningRate;
        }
        else
            return 0.0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public double GetPreviousEta()
    {
        // provided because threads might change the current eta before we are able to read it
        if (_NN != null)
        {
            return _NN.m_etaLearningRatePrevious;
        }
        else
            return 0.0;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bFromRandomizedPatternSequence"></param>
    /// <returns></returns>


    /////////////////////////
    /// <summary>
    /// Get Next Parttern in Parttern List
    /// </summary>
    /// <param name="iSequenceNum"></param>
    /// <param name="bFromRandomizedPatternSequence"></param>
    /// <returns></returns>
    public void CalculateNeuralNet(double[] inputVector, int count,
                               double[] outputVector /* =NULL */, int oCount /* =0 */,
                               NNNeuronOutputsList pNeuronOutputs /* =NULL */,
                               bool bDistort /* =FALSE */ )
    {
        // wrapper function for neural net's Calculate() function, needed because the NN is a protected member
        // waits on the neural net mutex (using the CAutoMutex object, which automatically releases the
        // mutex when it goes out of scope) so as to restrict access to one thread at a time
        m_Mutexs[0].WaitOne();
        {
            if (bDistort != false)
            {
                GenerateDistortionMap(1.0);
                ApplyDistortionMap(inputVector);
            }


            _NN.Calculate(inputVector, count, outputVector, oCount, pNeuronOutputs);
        }
        m_Mutexs[0].ReleaseMutex();

    }
    /// <summary>
    /// Distortion Pattern
    /// </summary>
    /// <param name="inputVector"></param>
    protected void ApplyDistortionMap(double[] inputVector)
    {
        // applies the current distortion map to the input vector

        // For the mapped array, we assume that 0.0 == background, and 1.0 == full intensity information
        // This is different from the input vector, in which +1.0 == background (white), and 
        // -1.0 == information (black), so we must convert one to the other

        List<List<double>> mappedVector = new List<List<double>>(_cRows);
        for (int i = 0; i < _cRows; i++)
        {
            List<double> mVector = new List<double>(_cCols);

            for (int j = 0; j < _cCols; j++)
            {
                mVector.Add(0.0);
            }
            mappedVector.Add(mVector);
        }

        double sourceRow, sourceCol;
        double fracRow, fracCol;
        double w1, w2, w3, w4;
        double sourceValue;
        int row, col;
        int sRow, sCol, sRowp1, sColp1;
        bool bSkipOutOfBounds;

        for (row = 0; row < _cRows; ++row)
        {
            for (col = 0; col < _cCols; ++col)
            {
                // the pixel at sourceRow, sourceCol is an "phantom" pixel that doesn't really exist, and
                // whose value must be manufactured from surrounding real pixels (i.e., since 
                // sourceRow and sourceCol are floating point, not ints, there's not a real pixel there)
                // The idea is that if we can calculate the value of this phantom pixel, then its 
                // displacement will exactly fit into the current pixel at row, col (which are both ints)

                sourceRow = (double)row - m_DispV[row * _cCols + col];
                sourceCol = (double)col - m_DispH[row * _cCols + col];

                // weights for bi-linear interpolation

                fracRow = sourceRow - (int)sourceRow;
                fracCol = sourceCol - (int)sourceCol;


                w1 = (1.0 - fracRow) * (1.0 - fracCol);
                w2 = (1.0 - fracRow) * fracCol;
                w3 = fracRow * (1 - fracCol);
                w4 = fracRow * fracCol;


                // limit indexes

                /*
                            while (sourceRow >= m_cRows ) sourceRow -= m_cRows;
                            while (sourceRow < 0 ) sourceRow += m_cRows;
			
                            while (sourceCol >= m_cCols ) sourceCol -= m_cCols;
                            while (sourceCol < 0 ) sourceCol += m_cCols;
                */
                bSkipOutOfBounds = false;

                if ((sourceRow + 1.0) >= _cRows) bSkipOutOfBounds = true;
                if (sourceRow < 0) bSkipOutOfBounds = true;

                if ((sourceCol + 1.0) >= _cCols) bSkipOutOfBounds = true;
                if (sourceCol < 0) bSkipOutOfBounds = true;

                if (bSkipOutOfBounds == false)
                {
                    // the supporting pixels for the "phantom" source pixel are all within the 
                    // bounds of the character grid.
                    // Manufacture its value by bi-linear interpolation of surrounding pixels

                    sRow = (int)sourceRow;
                    sCol = (int)sourceCol;

                    sRowp1 = sRow + 1;
                    sColp1 = sCol + 1;

                    while (sRowp1 >= _cRows) sRowp1 -= _cRows;
                    while (sRowp1 < 0) sRowp1 += _cRows;

                    while (sColp1 >= _cCols) sColp1 -= _cCols;
                    while (sColp1 < 0) sColp1 += _cCols;

                    // perform bi-linear interpolation

                    sourceValue = w1 * inputVector[sRow * _cCols + sCol] +
                        w2 * w1 * inputVector[sRow * _cCols + sColp1] +
                        w3 * w1 * inputVector[sRowp1 * _cCols + sCol] +
                        w4 * w1 * inputVector[sRowp1 * _cCols + sColp1];
                }
                else
                {
                    // At least one supporting pixel for the "phantom" pixel is outside the
                    // bounds of the character grid. Set its value to "background"

                    sourceValue = 1.0;  // "background" color in the -1 -> +1 range of inputVector
                }

                mappedVector[row][col] = 0.5 * (1.0 - sourceValue);  // conversion to 0->1 range we are using for mappedVector

            }
        }

        // now, invert again while copying back into original vector

        for (row = 0; row < _cRows; ++row)
        {
            for (col = 0; col < _cCols; ++col)
            {
                inputVector[row * _cCols + col] = 1.0 - 2.0 * mappedVector[row][col];
            }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="severityFactor"></param>
    protected void GenerateDistortionMap(double severityFactor /* =1.0 */ )
    {
        // generates distortion maps in each of the horizontal and vertical directions
        // Three distortions are applied: a scaling, a rotation, and an elastic distortion
        // Since these are all linear tranformations, we can simply add them together, after calculation
        // one at a time

        // The input parameter, severityFactor, let's us control the severity of the distortions relative
        // to the default values.  For example, if we only want half as harsh a distortion, set
        // severityFactor == 0.5

        // First, elastic distortion, per Patrice Simard, "Best Practices For Convolutional Neural Networks..."
        // at page 2.
        // Three-step process: seed array with uniform randoms, filter with a gaussian kernel, normalize (scale)

        int row, col;
        double[] uniformH = new double[_cCount];
        double[] uniformV = new double[_cCount];
        Random rdm = new Random();

        for (col = 0; col < _cCols; ++col)
        {
            for (row = 0; row < _cRows; ++row)
            {

                uniformH[row * _cCols + col] = (double)(2.0 * rdm.NextDouble() - 1.0);
                uniformV[row * _cCols + col] = (double)(2.0 * rdm.NextDouble() - 1.0);
            }
        }

        // filter with gaussian

        double fConvolvedH, fConvolvedV;
        double fSampleH, fSampleV;
        double elasticScale = severityFactor * m_Preferences.m_dElasticScaling;
        int xxx, yyy, xxxDisp, yyyDisp;
        int iiMid = 21 / 2;  // GAUSSIAN_FIELD_SIZE (21) is strictly odd

        for (col = 0; col < _cCols; ++col)
        {
            for (row = 0; row < _cRows; ++row)
            {
                fConvolvedH = 0.0;
                fConvolvedV = 0.0;

                for (xxx = 0; xxx < 21; ++xxx)
                {
                    for (yyy = 0; yyy < 21; ++yyy)
                    {
                        xxxDisp = col - iiMid + xxx;
                        yyyDisp = row - iiMid + yyy;

                        if (xxxDisp < 0 || xxxDisp >= _cCols || yyyDisp < 0 || yyyDisp >= _cRows)
                        {
                            fSampleH = 0.0;
                            fSampleV = 0.0;
                        }
                        else
                        {
                            fSampleH = uniformH[yyyDisp * _cCols + xxxDisp];
                            fSampleV = uniformV[yyyDisp * _cCols + xxxDisp];
                        }

                        fConvolvedH += fSampleH * _GaussianKernel[yyy, xxx];
                        fConvolvedV += fSampleV * _GaussianKernel[yyy, xxx];
                    }
                }

                m_DispH[row * _cCols + col] = elasticScale * fConvolvedH;
                m_DispV[row * _cCols + col] = elasticScale * fConvolvedV;
            }
        }

        uniformH = null;
        uniformV = null;

        // next, the scaling of the image by a random scale factor
        // Horizontal and vertical directions are scaled independently

        double dSFHoriz = severityFactor * m_Preferences.m_dMaxScaling / 100.0 * (2.0 * rdm.NextDouble() - 1.0);  // m_dMaxScaling is a percentage
        double dSFVert = severityFactor * m_Preferences.m_dMaxScaling / 100.0 * (2.0 * rdm.NextDouble() - 1.0);  // m_dMaxScaling is a percentage


        int iMid = _cRows / 2;

        for (row = 0; row < _cRows; ++row)
        {
            for (col = 0; col < _cCols; ++col)
            {
                m_DispH[row * _cCols + col] = m_DispH[row * _cCols + col] + dSFHoriz * (col - iMid);
                m_DispV[row * _cCols + col] = m_DispV[row * _cCols + col] - dSFVert * (iMid - row);  // negative because of top-down bitmap
            }
        }


        // finally, apply a rotation

        double angle = severityFactor * m_Preferences.m_dMaxRotation * (2.0 * rdm.NextDouble() - 1.0);
        angle = angle * 3.1415926535897932384626433832795 / 180.0;  // convert from degrees to radians

        double cosAngle = Math.Cos(angle);
        double sinAngle = Math.Sin(angle);

        for (row = 0; row < _cRows; ++row)
        {
            for (col = 0; col < _cCols; ++col)
            {
                m_DispH[row * _cCols + col] = m_DispH[row * _cCols + col] + (col - iMid) * (cosAngle - 1) - (iMid - row) * sinAngle;
                m_DispV[row * _cCols + col] = m_DispV[row * _cCols + col] - (iMid - row) * (cosAngle - 1) + (col - iMid) * sinAngle;  // negative because of top-down bitmap
            }
        }

    }
}
