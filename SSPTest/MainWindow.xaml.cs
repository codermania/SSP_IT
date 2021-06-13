using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SSP;
using SSP.Enums;
using SSP.Events;
using SSPTest.Models;

namespace SSPTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SSPDevice _device;
        private bool _enablePoll;
        private List<ChannelInfo> _channelInfo;

        private uint _sum = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _device = new SSPDevice("COM3", 9600, Parity.None, 8, StopBits.Two);
            _device.DataReceived += DeviceOnDataReceived;
        }

        private void DeviceOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            //if (e.Event == null)
            //{
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        ResultTextBox.Text += $"{e.Command} {e.Response} {string.Join(" ", e.ResponseBytes)}\n";
            //    });
            //} else
            //{
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        ResultTextBox.Text += $"{e.Command} {e.Response} {e.Event} {string.Join(" ", e.ResponseBytes)}\n";
            //    });
            //}

            if (e.Event == DeviceEvent.NoteCredit)
            {
                var index = e.ResponseBytes[5];

                var channelInfo = _channelInfo.FirstOrDefault(i => i.Id == index);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ResultTextBox.Text += $"{e.Command} {e.Response} {e.Event} {string.Join(" ", e.ResponseBytes)}\n";

                    if (channelInfo != null)
                    {
                        ResultTextBox.Text += $"Принял {channelInfo.Value} {channelInfo.Name}\n";
                    }

                    _sum += channelInfo.Value;

                    ResultTextBox.Text += $"Всего {_sum}\n\n";

                });
            }

            if (e.Command == Command.Enable)
            {
                _sum = 0;
                _enablePoll = true;
            }

            if (e.Command == Command.Disable)
            {
                _enablePoll = false;
            }

            if (e.Command == Command.ChannelValueData && e.ResponseBytes.Length > 6)
            {
                _channelInfo = new List<ChannelInfo>();

                var channelCount = e.ResponseBytes.ElementAt(4);

                var length = 5 + (1 * channelCount) + (3 * channelCount) + (4 * channelCount) + 2;

                if (e.ResponseBytes.Length == length)
                {
                    for (var i = 0; i < channelCount; i++)
                    {
                        var startCurrencyPosition = 5 + (1 * channelCount) + (i * 3);

                        var currencyNameByte = e.ResponseBytes[new Range(startCurrencyPosition, startCurrencyPosition + 3)];

                        var currencyName = Encoding.ASCII.GetString(currencyNameByte);

                        var startCurrencyValue = 5 + (1 * channelCount) + (3 * channelCount) + (i * 4);

                        var currencyValueBytes = e.ResponseBytes[new Range(startCurrencyValue, startCurrencyValue + 4)];

                        var currencyValue = BitConverter.ToUInt32(currencyValueBytes, 0);

                        _channelInfo.Add(new ChannelInfo
                        {
                            Id = i + 1,
                            Name = currencyName,
                            Value = currencyValue
                        });
                    }
                }
            }

            if (_enablePoll)
            {
                _device.SendCommand(Command.Poll);
            }
        }

        private void DisconnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.Disconnect();
        }

        private void SyncButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.SendCommand(Command.Sync);
        }

        private void SerialButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.SendCommand(Command.GetSerialNumber);
        }

        private async void EnableButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.SendCommand(Command.Enable);
        }

        private void PollButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.SendCommand(Command.Poll);
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.SendCommand(Command.Reset);
        }

        private void DisableButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.SendCommand(Command.Disable);
        }

        private void ChannelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _device?.SendCommand(Command.ChannelValueData);
        }
    }
}
