using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AlarmManager
{
    public class Alarm
    {
        public string AlarmMessage;
        public string AlarmType;
        public bool Suppressed;
        public DateTime RaiseTime;
        public int DeviceID;

        public Alarm(string message, string type, bool suppr, DateTime time, int ID) 
        {
            this.AlarmMessage = message;
            this.AlarmType = type;
            this.Suppressed = suppr;
            this.RaiseTime = time;
            this.DeviceID = ID;
        }

        public Alarm(string message, string type, DateTime time, int ID)
        {
            this.AlarmMessage = message;
            this.AlarmType = type;
            this.RaiseTime = time;
            this.DeviceID = ID;
            this.Suppressed = false;
        }

        public static bool operator ==(Alarm A1, Alarm A2)
        {
            if (A1.DeviceID == A2.DeviceID)
            {
                if (A1.AlarmType == A2.AlarmType && A1.DeviceID == A2.DeviceID)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool operator !=(Alarm A1, Alarm A2)
        {
            if (A1.DeviceID == A2.DeviceID)
            {
                if (A1.AlarmType != A2.AlarmType || A1.DeviceID != A2.DeviceID)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }


        public override string ToString()
        {
            string line = $"Alarm {AlarmType.PadRight(20)}|    {AlarmMessage.PadRight(20)} |    ID= {DeviceID.ToString().PadRight(10)} |    At: {RaiseTime.ToString("hh:mm:ss").PadRight(20)}";
            return line;
        }

    }
}
