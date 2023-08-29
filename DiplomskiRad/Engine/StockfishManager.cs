using System;
using System.Diagnostics;
using System.IO;

namespace DiplomskiRad.Engine
{
    public class StockfishManager : IDisposable
    {
        private Process _stockfishProcess;
        private StreamWriter _stockfishInput;
        private StreamReader _stockfishOutput;


        public  StockfishManager(int engineStrength)
        {
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

            //SendCommand("setoption name UCI_LimitStrength value true");
            //SendCommand("setoption name Skill Level value -1");

            //ReadResponse();
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

        //public void Close()
        //{
        //    SendCommand("quit");
        //    _stockfishProcess.WaitForExit();
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
            return ReadResponse();
        }

        private string ReadResponse()
        {
            string response = "";
            string line;

            line = _stockfishOutput.ReadLine();
            //while ((line = _stockfishOutput.ReadLine()) != null)
            //{
            //    if (line is "uciok" or "readyok")
            //    {
            //        response += line + Environment.NewLine;
            //        break;
            //    }
            //    response += line + Environment.NewLine;
            //}

            return line;
        }

        public void Dispose()
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
