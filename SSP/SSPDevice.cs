using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Data.HashFunction.CRC;
using System.IO.Ports;
using System.Linq;
using SSP.Enums;
using SSP.Events;
using SSP.Interfaces;
using SSP.Models;

namespace SSP
{
    public class SSPDevice : ISSPDevice
    {
        private const byte STX = 127;

        private byte _sequence = 128;
        private SerialPort _serialPort;
        private CRCConfig _crcConfig;
        private ICRC _crc;
        private Command _lastCommand;
        private readonly ConcurrentQueue<DeviceCommand> _commands;
        private bool _commandRunning;

        public SSPDevice(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _commands = new ConcurrentQueue<DeviceCommand>();

            _crcConfig = new CRCConfig
            {
                InitialValue = 0xFFFF,
                HashSizeInBits = 16,
                Polynomial = 0x8005
            };

            _crc = CRCFactory.Instance.Create(_crcConfig);

            ConnectToDevice(portName, baudRate, parity, dataBits, stopBits);
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        private void ConnectToDevice(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

            _serialPort.DataReceived += SerialPortOnDataReceived;

            _serialPort.Open();
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var result = new byte[_serialPort.BytesToRead];

            _serialPort.Read(result, 0, _serialPort.BytesToRead);

            var eventArgs = new DataReceivedEventArgs
            {
                ResponseBytes = result,
                Command = _lastCommand
            };

            try
            {
                eventArgs.Response = (Response) result[3];
            }
            catch (Exception)
            {
                // ignored
            }

            if (result.Length > 6 && _lastCommand == Command.Poll)
            {
                try
                {
                    if (Enum.IsDefined(typeof(DeviceEvent), result[4]))
                    {
                        eventArgs.Event = (DeviceEvent)result[4];
                    }
                    else
                    {
                        eventArgs.Event = null;
                    }
                }
                catch (Exception ex)
                {
                    eventArgs.Event = null;
                }
            }

            DataReceived?.Invoke(this, eventArgs);

            if (_commands.IsEmpty)
            {
                _commandRunning = false;
            }
            else
            {
                RunCommand();
            }
        }

        public void Disconnect()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void SendCommand(Command command, byte[] args = null)
        {
            _commands.Enqueue(new DeviceCommand
            {
                Command = command,
                Args = args
            });

            if (!_commandRunning)
            {
                RunCommand();
            }
        }

        private void RunCommand()
        {
            if (_commands.IsEmpty)
            {
                return;
            }

            var dequeueResult = _commands.TryDequeue(out DeviceCommand deviceCommand);

            if (!dequeueResult)
            {
                return;
            }

            if (_serialPort == null || !_serialPort.IsOpen)
            {
                return;
            }

            IHashValue hashValue;

            var commandList = new List<byte>
            {
                STX, _sequence
            };

            //if (deviceCommand.Command == Command.Sync || deviceCommand.Command == Command.HostProtocolVersion)
            //{
            //    commandList.Add(0x80);
            //    _sequence = 0x80;
            //}
            //else
            //{
            //    commandList.Add(_sequence);
            //}

            switch (deviceCommand.Command)
            {
                case Command.Sync:
                case Command.SetupRequest:
                case Command.Reset:
                case Command.SetInhibits:
                case Command.HostProtocolVersion:
                case Command.Enable:
                case Command.Disable:
                case Command.GetSerialNumber:
                case Command.GetFirmwareVersion:
                case Command.GetDataSetVersion:
                case Command.DisplayOn:
                case Command.DisplayOff:
                case Command.Reject:
                case Command.UnitData:
                case Command.ChannelValueData:
                case Command.ChannelSecurityData:
                case Command.LastRejectCode:
                case Command.ConfigureBezel:
                case Command.PollWithAck:
                case Command.EventAck:
                case Command.Poll:

                    if (deviceCommand.Args != null)
                    {
                        commandList.Add((byte)(1 + deviceCommand.Args.Length));
                    }
                    else
                    {
                        commandList.Add(1);
                    }
                    
                    commandList.Add((byte)deviceCommand.Command);

                    if (deviceCommand.Args != null && deviceCommand.Args.Length > 0)
                    {
                        commandList.AddRange(deviceCommand.Args);
                    }

                    hashValue = _crc.ComputeHash(commandList.Skip(1).ToArray());

                    commandList.AddRange(hashValue.Hash);

                    _commandRunning = true;

                    _serialPort.Write(commandList.ToArray(), 0, commandList.Count);
                    _lastCommand = deviceCommand.Command;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceCommand.Command), deviceCommand.Command, null);
            }

            _sequence ^= (1 << 7);
        }
    }


}
