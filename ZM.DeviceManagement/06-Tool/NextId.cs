using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZM.Device.Tool
{
    public static class NextId
    {
        private static readonly object _lock = new object ();
        private static long _lastTimestamp = 0;
        private static int _sequence = 0;
        public static long Id13()
        {
            lock (_lock)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (timestamp == _lastTimestamp)
                {
                    _sequence++;
                    if (_sequence >= 1000)
                    {
                        while (timestamp == _lastTimestamp)
                        {
                            System.Threading.Thread.Sleep(1);
                            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        }

                        _sequence = 0;
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                long id = long.Parse($"{timestamp}{_sequence:D3}");
                return id;
            }
        }
    }
}