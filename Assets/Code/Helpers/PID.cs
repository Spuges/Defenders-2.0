using System;
using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    [Serializable]
    public class PIDD
    {
        public double pFactor, iFactor, dFactor;
        double integral;
        double lastError;

        public PIDD(double pFactor, double iFactor, double dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }

        public double Update(double setpoint, double actual, double timeFrame)
        {
            double present = setpoint - actual;
            integral += present * timeFrame;
            double deriv = (present - lastError) / timeFrame;
            lastError = present;
            return present * pFactor + integral * iFactor + deriv * dFactor;
        }
    }

    [Serializable]
    public class PID
    {
        public float pFactor, iFactor, dFactor;
        float integral = 0;
        float lastError = 0;

        public float LastError => lastError;

        public PID(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }

        public float Update(float setpoint, float actual, float timeFrame)
        {
            float present = setpoint - actual;

            float p_abs = Math.Abs(present);
            float e_abs = Math.Abs(lastError);

            if(p_abs > 0 && e_abs > 0 && Mathf.Approximately(p_abs, e_abs))
            {
                present = 0;
                lastError = 0;
            }

            integral += present * timeFrame;
            float deriv = (present - lastError) / timeFrame;
            lastError = present;
            return present * pFactor + integral * iFactor + deriv * dFactor;
        }
    }

    [Serializable]
    public class PIDV2
    {
        public float pFactor, iFactor, dFactor;
        float2 integral = 0;
        float2 lastError = 0;

        public PIDV2(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }

        public float2 Update(float2 setpoint, float2 actual, float timeFrame)
        {
            float2 present = setpoint - actual;
            integral += present * timeFrame;
            float2 deriv = (present - lastError) / timeFrame;
            lastError = present;
            return present * pFactor + integral * iFactor + deriv * dFactor;
        }
    }

    [Serializable]
    public class PIDV3
    {
        public float pFactor, iFactor, dFactor;
        float3 integral = 0;
        float3 lastError = 0;

        public PIDV3(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }

        public float3 Update(float3 setpoint, float3 actual, float timeFrame)
        {
            float3 present = setpoint - actual;
            integral += present * timeFrame;
            float3 deriv = (present - lastError) / timeFrame;
            lastError = present;
            return present * pFactor + integral * iFactor + deriv * dFactor;
        }
    }
}

