using System;
using System.Diagnostics;
using System.IO;

namespace DiplomskiRad.Engine
{
    public class StockfishManager
    {
        private readonly Process _stockfishProcess;
        private readonly StreamWriter _stockfishInput;
        private readonly StreamReader _stockfishOutput;

        private string _position;
        private readonly int _engineStrength;

        public  StockfishManager(int engineStrength)
        {
            _position = "position startpos moves ";
            _engineStrength = engineStrength;
            string chosenEngine = _engineStrength > 1250 ? "Stronger" : "Weaker";

            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Engine");

            _stockfishProcess = new Process();
            _stockfishProcess.StartInfo.FileName = Path.Combine(targetFolder, StockfishSetting.Engine[chosenEngine]);
            _stockfishProcess.StartInfo.UseShellExecute = false;
            _stockfishProcess.StartInfo.RedirectStandardInput = true;
            _stockfishProcess.StartInfo.RedirectStandardOutput = true;
            _stockfishProcess.StartInfo.CreateNoWindow = true;

            _stockfishProcess.Start();
            _stockfishInput = _stockfishProcess.StandardInput;
            _stockfishOutput = _stockfishProcess.StandardOutput;

            SendCommand("uci");
            SendCommand("isready");

            SendCommand("setoption name UCI_LimitStrength value true");
            SendCommand(StockfishSetting.Setting[_engineStrength][0]);
        }

        public string SendCommand(string command)
        {
            _stockfishInput.WriteLine(command);
            _stockfishInput.Flush();
            return ReadResponse(command);
        }

        private string ReadResponse(string command)
        {
            if (command.Equals("uci")) return LongResponse();
            if (command.StartsWith("setoption") || command.StartsWith("position")) return "";
            return ShortResponse();
        }

        private string LongResponse()
        {
            string response = "";
            string line;

            while ((line = _stockfishOutput.ReadLine()) != null)
            {
                if (line is "uciok" or "readyok")
                {
                    response += line + Environment.NewLine;
                    break;
                }
                response += line + Environment.NewLine;
            }

            return response;
        }

        private string ShortResponse() => _stockfishOutput.ReadLine();

        public string GetBestMove(string move = "")
        {
            _position += move;
            SendCommand(_position);
            var bestMove = GetMove($"go {StockfishSetting.Setting[_engineStrength][1]} {StockfishSetting.Setting[_engineStrength][2]}");
            _position += $"{bestMove} ";

            return bestMove;
        }

        private string GetMove(string command)
        {
            _stockfishInput.WriteLine(command);
            _stockfishInput.Flush();

            string line;
            while ((line = _stockfishOutput.ReadLine()) != null)
            {
                if (line.StartsWith("bestmove")) return line.Split(' ')[1];
            }

            return "";
        }

        public void Close()
        {
            if (_stockfishProcess != null && !_stockfishProcess.HasExited)
            {
                SendCommand("quit");
                _stockfishProcess.WaitForExit();
                _stockfishProcess.Close();
            }
            _stockfishInput.Close();
            _stockfishOutput.Close();
        }
    }
}
