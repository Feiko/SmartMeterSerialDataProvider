// See https://aka.ms/new-console-template for more information
using System.Text;
using System.IO.Ports;

Console.WriteLine("Hello, World!");
var serialPort = new SerialPort("comxxxxxx");
serialPort.BaudRate = 115200;
serialPort.Parity = Parity.None;
serialPort.StopBits = StopBits.One;
serialPort.Handshake = Handshake.None;

serialPort.ReadTimeout = 500;
serialPort.WriteTimeout = 500;
serialPort.Open();



while (true)
{
    string TestDataRaw = @$"/XMX5LGF0000453094270

1-3:0.2.8(50)
0-0:1.0.0({DateTime.Now.ToString("yyMMddhhmmss")}W)
0-0:96.1.1(4530303531303035333039343237303139)
1-0:1.8.1(001819.387*kWh)
1-0:1.8.2(002093.302*kWh)
1-0:2.8.1(000088.650*kWh)
1-0:2.8.2(000157.206*kWh)
0-0:96.14.0(0002)
1-0:1.7.0(00.288*kW)
1-0:2.7.0(00.000*kW)
0-0:96.7.21(00015)
0-0:96.7.9(00002)
1-0:99.97.0(1)(0-0:96.7.19)(190226161118W)(0000000541*s)
1-0:32.32.0(00019)
1-0:32.36.0(00002)
0-0:96.13.0()
1-0:32.7.0(231.0*V)
1-0:31.7.0(001*A)
1-0:21.7.0(00.288*kW)
1-0:22.7.0(00.000*kW)
0-1:24.1.0(003)
0-1:96.1.0(4730303339303031393231393034393139)
0-1:24.2.1(210304120005W)(01980.598*m3)
";

    string message = TestDataRaw;
    string crc = CreateCrc(Encoding.UTF8.GetBytes(TestDataRaw + "!"));
    Console.WriteLine(message + crc);
    serialPort.Write(message);
    Thread.Sleep(112);
    serialPort.Write(crc);
    Thread.Sleep(880);
      
}




string CreateCrc(byte[] buf)
{

    uint crc = 0;
    // -6 because we are reusing the original byte[] because of memory conservation
    for (int pos = 0; pos < buf.Length; pos++)
    {
        crc ^= buf[pos];    // XOR byte into least sig. byte of crc

        for (int i = 8; i != 0; i--)
        {    // Loop over each bit
            if ((crc & 0x0001) != 0)
            {      // If the LSB is set
                crc >>= 1;                    // Shift right and XOR 0xA001
                crc ^= 0xA001;
            }
            else                            // Else LSB is not set
                crc >>= 1;                    // Just shift right
        }
    }
    
    var crcString = ((UInt16)crc).ToString("X");
    for (int i = crcString.Length; i < 4; i++)
    {
        crcString = "0" + crcString;
    }
    return $"!{crcString}  ";
}


