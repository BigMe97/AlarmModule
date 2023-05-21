using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmManager
{
    public class Device
    {
        public int ID;
        public string deviceName;
        public string unit;
        public double hiLimit;
        public double lowLimit;
        public double hiHiLimit;
        public double lowLowLimit;
        public double value;
        public DateTime time;

        public Device(int id) 
        {
            this.ID = id;
        }
        public Device(int id, string deviceName, string unit, double lo, double lolo, double hi, double hihi)
        {
            this.ID = id;
            this.deviceName = deviceName;
            this.unit = unit;
            this.lowLimit = lo;
            this.lowLowLimit = lolo;
            this.hiLimit = hi;
            this.hiHiLimit = hihi;
        }

        public override string ToString()
        {
            return $"ID: {ID},   Tag: {deviceName},   Unit: {unit},   Value: {value}";
        }
    }
}
