using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LifeConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            int _checkWidth = 3;
            int _checkHeight = 3;
            int _stepCount = 50;
            Check(_checkWidth, _checkHeight, _stepCount);
            //CheckPeriod(_checkWidth, _checkHeight, _stepCount);


            //Life.LifeGame _lg = Life.GameCreator.RectangleTorGame(100, 100, 1000);
            //_lg.MaxAge = 7;
            //_lg.MaxNeighbours = 5;
            //_lg.MinNeighbours = 3;
            //_lg.NeighboursForNew = 3;
            //_lg.MaxAge = 7;

            //DateTime _start = DateTime.Now;
            //for(int i = 1; i < 10; i++)
            //{
            //    _lg.Step();
            //    if(_lg.ItemCount == 0)
            //    {
            //        Console.WriteLine("Field is empty.");
            //    }
            //}
            //TimeSpan _ts = DateTime.Now - _start;
            //Console.WriteLine(_ts.TotalMilliseconds);

            Console.ReadLine();
        }

        private static void Check(int aCheckWidth, int aCheckHeight, int aStepCount)
        {
            int _checkSize = aCheckWidth * aCheckHeight;

            int _fieldHeight = 50;
            int _fieldWidth = 50;

            int _maxNeighbourCount = 5;
            int _minNeighbourCount = 3;
            int _neighbourCount = 3;

            //int _stepsCount = 50;

            long _checkLength = (long)Math.Pow(2, _checkSize);
            long _gameIndex = (long)Math.Pow(2, _checkSize - 4);

			Life.LifeGame _game = Life.GameCreator.RectangleTorGame(_fieldHeight, _fieldWidth, 0);
			//Life.LifeGame _game = Life.GameCreator.HexagonalTorGame(_fieldHeight, _fieldWidth, 0);
            _game.MaxNeighbours = _maxNeighbourCount;
            _game.MinNeighbours = _minNeighbourCount;
            _game.NeighboursForNew = _neighbourCount;
            FileStream _fs = File.OpenWrite("output.txt");
            StreamWriter _sw = new StreamWriter(_fs);
            _sw.WriteLine("GameIndex, MaxAge, StepNum, StartItemCount, ItemCount");
            for (; _gameIndex < _checkLength; _gameIndex++)
            {
                for (int _maxAge = 1; _maxAge < 21; _maxAge++)
                {
                    _game.MaxAge = _maxAge;

                    int _stepNum = aStepCount;
                    int _itemCount = FillGameField(aCheckWidth, aCheckHeight, _fieldWidth, _game, _gameIndex);
                    for (int _step = 0; _step < aStepCount; _step++)
                    {
                        _game.Step();
                        if (_game.ItemCount <= 0)
                        {
                            _stepNum = _step;
                            break;
                        }
                    }
                    if (_game.ItemCount > 0)
                        PrintLog(_gameIndex, _maxAge, _stepNum, _itemCount, _game.ItemCount, _sw);
                }

                Console.WriteLine(_gameIndex);
            }

            _sw.Close();
            _fs.Close();
        }

        private static void CheckPeriod(int aCheckWidth, int aCheckHeight, int aStepCount)
        {
            int _checkSize = aCheckWidth * aCheckHeight;

            int _fieldHeight = 50;
            int _fieldWidth = 50;

            int _maxNeighbourCount = 5;
            int _minNeighbourCount = 3;
            int _neighbourCount = 3;

            //int _stepsCount = 50;

            long _checkLength = (long)Math.Pow(2, _checkSize);

			Life.LifeGame _game = Life.GameCreator.RectangleTorGame(_fieldHeight, _fieldWidth, 0);
			//Life.LifeGame _game = Life.GameCreator.HexagonalTorGame(_fieldHeight, _fieldWidth, 0);
            _game.MaxNeighbours = _maxNeighbourCount;
            _game.MinNeighbours = _minNeighbourCount;
            _game.NeighboursForNew = _neighbourCount;
            FileStream _fs = File.OpenWrite("output.txt");
            StreamWriter _sw = new StreamWriter(_fs);
            _sw.WriteLine("GameIndex, MaxAge, StepNum, Period, ItemCount");
            for (long _gameIndex = 0; _gameIndex < _checkLength; _gameIndex++)
            {
                for (int _maxAge = 1; _maxAge < 11; _maxAge++)
                {
                    _game.MaxAge = _maxAge;

                    int _stepNum = aStepCount;
                    int _itemCount = FillGameField(aCheckWidth, aCheckHeight, _fieldWidth, _game, _gameIndex);
                    List<int> _itemsCount = new List<int>();
                    bool _isPeriodFounded = false;
                    for (int _step = 0; _step < aStepCount && !_isPeriodFounded && _game.ItemCount > 0; _step++)
                    {
                        _game.Step();
                        if (_itemsCount.Count > 2)
                        {
                            int _index1 = -1;
                            int _index2 = -1;
                            for (int i = _itemsCount.Count - 2; i >= 0 && !_isPeriodFounded; i--)
                            {
                                if (_itemsCount[i] == _game.ItemCount)
                                {
                                    if (_index1 == -1 && _game.ItemCount - i != 1)
                                    {
                                        _index1 = i;
                                    }
                                    else if (_index2 == -1)
                                    {
                                        if (_index1 - i == _itemsCount.Count - _index1)
                                            _index2 = i;
                                    }
                                    else
                                    {
                                        if (_index2 - i == _itemsCount.Count - _index1)
                                        {
                                            PrintPeriodLog(_gameIndex, _maxAge, _step, _index2 - i, _game.ItemCount, _sw);
                                            _isPeriodFounded = true;
                                        }
                                    }
                                }
                            }
                        }

                        _itemsCount.Add(_game.ItemCount);
                    }
                }

                Console.WriteLine(_gameIndex);
            }

            _sw.Close();
            _fs.Close();
        }

        private static void PrintPeriodLog(long aGameIndex, int aMaxAge, int aStepNum, int aPeriod, int aItemCount, StreamWriter aSw)
        {
            aSw.WriteLine("{0}, {1}, {2}, {3}, {4}", aGameIndex, aMaxAge, aStepNum, aPeriod, aItemCount);
        }

        private static void PrintLog(long aGameIndex, int aMaxAge, int aStepNum, int aStartItemCount, int aItemCount, StreamWriter aSw)
        {

            aSw.WriteLine("{0}, {1}, {2}, {3}, {4}", aGameIndex, aMaxAge, aStepNum, aStartItemCount, aItemCount);

        }

        private static int FillGameField(
            int aCheckWidth, int aCheckHeight, int aFieldWidth, Life.LifeGame aGame, long aGameIndex)
        {
            if (aGame.ItemCount > 0)
                foreach (Life.Cell _cell in aGame.GameField)
                    _cell.Clear();

            int _itemCount = 0;
            for (int w = 0; w < aCheckWidth; w++)
            {
                for (int h = 0; h < aCheckHeight; h++)
                {
                    int _checkIndex = h * aCheckWidth + w;
                    long _mask = 1L << _checkIndex;
                    int _fieldIndex = h * aFieldWidth + w;

                    if ((aGameIndex & _mask) > 0)
                    {
                        aGame.InitialCell(_fieldIndex);
                        _itemCount++;
                    }
                }
            }
            return _itemCount;

            //return aGame.GameField.Count(x => x.CellItem != null);
        }
    }
}
