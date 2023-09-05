using System;
using System.Diagnostics;
using System.IO;

namespace DiplomskiRad.Engine
{
    public class StockfishManager/* : IDisposable*/
    {
        private readonly Process _stockfishProcess;
        private readonly StreamWriter _stockfishInput;
        private readonly StreamReader _stockfishOutput;

        private string _position;

        public  StockfishManager(int engineStrength)
        {
            _position = "position startpos moves ";

            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Engine");

            _stockfishProcess = new Process();
            _stockfishProcess.StartInfo.FileName = Path.Combine(targetFolder, "stockfish.exe");
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
            SendCommand("setoption name Skill Level value 1");
        }

        //public string GetBestMove(string fenPosition, int depth = 10)
        //{
        //    SendCommand($"position fen {fenPosition}");
        //    SendCommand($"go depth {depth}");
        //    string analysisResponse = ReadResponse();

        //    // Parsiranje analize i izdvajanje najboljeg poteza
        //    // Implementacija parsiranja zavisi od formata odgovora Stockfish-a

        //    return "bestMove";
        //}



        //private void SendCommand(string command)
        //{
        //    _inputWriter.WriteLine(command);
        //    _inputWriter.Flush();
        //}

        //private string ReadResponse()
        //{
        //    return _outputReader.ReadLine();
        //}

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
            var bestMove = GetMove("go depth 1");
            _position += $"{bestMove} ";

            return bestMove;
        }

        private string GetMove(string command)
        {
            _stockfishInput.WriteLine(command);
            _stockfishInput.Flush();

            string response = "";
            string line;

            while ((line = _stockfishOutput.ReadLine()) != null)
            {
                if (line.StartsWith("bestmove")) return line.Split(' ')[1];
            }

            return "";
        }

        //public void Dispose()
        //{
        //    if (_stockfishProcess != null && !_stockfishProcess.HasExited)
        //    {
        //        SendCommand("quit");
        //        _stockfishProcess.WaitForExit();
        //        _stockfishProcess.Close();
        //    }
        //    _stockfishInput.Close();
        //    _stockfishOutput.Close();
        //}

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
