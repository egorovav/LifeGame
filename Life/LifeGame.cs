using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Life
{   
    public class Item : ICloneable
    {
        public Item()
        {

        }

        public Item(bool aIsStarted)
        {
            if (aIsStarted)
                this.FAge = 1; 
        }

        int FAge = 0;
        [XmlElement]
        public int Age
        {
            get { return this.FAge; }
            set { this.FAge = value; }
        }

        private bool FIsDie = false;
        [XmlIgnore]
        public bool IsDie
        {
            get { return this.FIsDie; }
            set { this.FIsDie = value; }
        }

        public void IncrementAge()
        {
            this.FAge++;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class Cell
    {
        public Cell()
        {
        }

        public Cell(Item aItem)
        {
            this.FItem = aItem;
        }

        List<Cell> FNeighbour = new List<Cell>();
        [XmlIgnore]
        public List<Cell> Neighbour
        {
            get { return this.FNeighbour; }
        }

        Item FItem;
        public Item CellItem
        {
            get { return this.FItem; }
            set { this.FItem = value; }
        }

        public void Fill(bool aIsStarted)
        {
            this.FItem = new Item(aIsStarted);
        }

        public void Clear()
        {
            this.FItem = null;
        }
    }

    public class LifeGame
    {
        public LifeGame()
        {

        }

        public LifeGame(GameField aGameField)
        {
            this.FGameField = aGameField;
        }

        public LifeGame(GameField aGameField, int aStartItemCount) :
            this(aGameField)
        {
            if (aStartItemCount > 0)
                this.RandomFillGameField(aStartItemCount);
        }

        private int FMaxAge = 0;
        [XmlElement]
        public int MaxAge
        {
            get { return this.FMaxAge; }
            set { this.FMaxAge = value; }
        }

        private int FMaxNeighbours = 4;
        [XmlElement]
        public int MaxNeighbours
        {
            get { return this.FMaxNeighbours; }
            set { this.FMaxNeighbours = value; }
        }

        private int FMinNeighbours = 3;
        [XmlElement]
        public int MinNeighbours
        {
            get { return this.FMinNeighbours; }
            set { this.FMinNeighbours = value; }
        }

        private int FNeighboursForNew = 2;
        [XmlElement]
        public int NeighboursForNew
        {
            get { return this.FNeighboursForNew; }
            set { this.FNeighboursForNew = value; }
        }

        private int FStepCount;
        [XmlIgnore]
        public int StepCount
        {
            get { return this.FStepCount; }
        }

        [XmlIgnore]
        public int ItemCount
        {
            get 
            {
                if (this.GameField == null)
                    return 0;
                return this.GameField.Where(x => x.CellItem != null).Count(); 
            }
        }

        GameField FGameField;
        [XmlIgnore]
        public GameField GameField
        {
            get { return this.FGameField; }
            set { this.FGameField = value; }
        }

        private ItemData[] FStartPosition;
        [XmlIgnore]
        public ItemData[] CurrentPosition
        {
            get
            {
                int _itemCount = this.ItemCount;
                ItemData[] _position = new ItemData[_itemCount];
                int _itemCounter = 0; 
                for(int i = 0; i < this.GameField.Count; i++)
                {
                    Item _item = this.GameField[i].CellItem;
                    if (_item != null && !_item.IsDie)
                    {
                        _position[_itemCounter] = new ItemData(i, (Item)_item.Clone());
                        _itemCounter++;
                    }
                }
                return _position;
            }

            //set
            //{
            //    SetPosition(value);
            //}
        }

        public void Step()
        {
            if (this.FStepCount == 0)
                this.FStartPosition = this.CurrentPosition;

            foreach (Cell _cell in this.FGameField)
            {
                int _neighbourCount = _cell.Neighbour.Where(x => x.CellItem != null && x.CellItem.Age != 0).Count();
                if (_cell.CellItem != null)
                {
                    if (_neighbourCount < this.MinNeighbours || 
                        _neighbourCount > this.MaxNeighbours ||
                        (this.MaxAge != 0 && _cell.CellItem.Age > this.MaxAge))
                        _cell.CellItem.IsDie = true;
                }
                else if (_neighbourCount == this.NeighboursForNew)
                    _cell.Fill(false);    
            }

            foreach (Cell _cell in this.FGameField)
            {
                if (_cell.CellItem != null)
                {
                    if (_cell.CellItem.IsDie)
                        _cell.Clear();
                    else
                        _cell.CellItem.IncrementAge();
                }
            }
            this.FStepCount++;
        }

        private void RandomFillGameField(int aStartItemCount)
        {
            if (this.GameField.Count < aStartItemCount)
                throw new ArgumentException(String.Format(
                    "Can not put {0} items in {1} places. Parameter aItemCount must be less then length of aGameField",
                        aStartItemCount, this.GameField.Count), "aItemCount");

            int _fieldSize = this.GameField.Count;
            for (int i = 0; i < aStartItemCount; i++)
            {
                Random _rnd = new Random(DateTime.Now.Second);
                int _cellIndex = 0;
                do
                {
                    _cellIndex = _rnd.Next(_fieldSize);
                }
                while (this.GameField[_cellIndex].CellItem != null);

                this.InitialCell(_cellIndex);
            }
        }

        public void InitialCell(int aCellIndex)
        {
            this.GameField[aCellIndex].Fill(true);
        }

        public void Reset()
        {
            if (this.FStartPosition != null)
            {
                this.SetPosition(this.FStartPosition);
                this.FStepCount = 0;
            }
        }

        public void SetPosition(ItemData[] aPosition)
        {
            this.GameField.ClearCells();

            foreach (ItemData _item in aPosition)
            {
                if(!_item.Item.IsDie)
                    this.GameField[_item.Index].CellItem = _item.Item;
            }
        }
    }

    public class ItemData
    {
        public ItemData()
        {

        }

        public ItemData(int aIndex, Item aItem)
        {
            this.Index = aIndex;
            this.Item = aItem;
        }

        public int Index { get; set; }
        public Item Item { get; set; }
    }
}
