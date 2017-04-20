using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Life
{
    public class GameField : IList<Cell>, IXmlSerializable
    {
        public GameField()
        {

        }

        public GameField(object aData)
        {
            int _count = (int)aData;
            this.FCells = new Cell[_count];
        }

        protected Cell[] FCells;
        protected virtual Cell[] Cells
        {
            get { return this.FCells; }
        }

        public virtual void CreateField()
        {
            for (int i = 0; i < this.Cells.Length; i++)
            {
                this.Cells[i] = new Cell();
            }
        }

        public void ClearCells()
        {
            foreach (Cell _cell in this.Cells)
                _cell.Clear();
        }

        public virtual object GetData()
        {
            return this.Count;
        }

        #region = IList =

        public int IndexOf(Cell item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Cell item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Cell this[int index]
        {
            get { return this.Cells[index]; }
            set { this.Cells[index] = value; }
        }

        public void Add(Cell item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            this.FCells = new Cell[this.Cells.Length];
        }

        public bool Contains(Cell item)
        {
            return this.Cells.Contains(item);
        }

        public void CopyTo(Cell[] array, int arrayIndex)
        {
            this.Cells.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get 
            {
                if (this.Cells == null)
                    return 0;

                return this.Cells.Length; 
            }
        }

        public bool IsReadOnly
        {
            get { return this.Cells.IsReadOnly; }
        }

        public bool Remove(Cell item)
        {
            int _itemIndex = -1;
            for (int i = 0; i < this.Cells.Length; i++)
                if (this.Cells[i] == item)
                    _itemIndex = i;

            if (_itemIndex <= 0)
            {
                this.Cells[_itemIndex] = null;
                return true;
            }
            else
                return false;
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return new CellEnumerator(this.Cells);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Cells.GetEnumerator();
        }

        public class CellEnumerator : IEnumerator<Cell>
        {
            public CellEnumerator(Cell[] aCells)
            {
                this.FCellsEnumerator = aCells.GetEnumerator();
            }

            private IEnumerator FCellsEnumerator;

            public Cell Current
            {
                get { return (Cell)this.FCellsEnumerator.Current; }
            }

            public void Dispose()
            {
               
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.FCellsEnumerator.Current; }
            }

            public bool MoveNext()
            {
                return this.FCellsEnumerator.MoveNext();
            }

            public void Reset()
            {
                this.FCellsEnumerator.Reset();
            }
        }

        #endregion

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
        }
    }

    public class RectangleGameField : GameField
    {
        public RectangleGameField()
        {

        }

        public RectangleGameField(int aWidth, int aHeight)
        {
            this.FGameWidth = aWidth;
            this.FGameHeight = aHeight;
            this.FCells = new Cell[this.FGameWidth * this.FGameHeight];
            this.CreateField();
        }

        public RectangleGameField(RectangleGameFieldData aData)
            :this(aData.Width, aData.Height)
        {

        }

        private int FGameHeight = 74;
        public int GameHeight
        {
            get { return this.FGameHeight; }
            set { this.FGameHeight = value; }
        }

        private int FGameWidth = 74;
        public int GameWidth
        {
            get { return this.FGameWidth; }
            set { this.FGameWidth = value; }
        }

        protected override Cell[] Cells
        {
            get
            {
                if (this.FCells == null)
                    this.FCells = new Cell[this.FGameWidth * this.FGameHeight];

                return this.FCells;
            }
        }

		public virtual void SetSize(int width, int height)
		{
			this.FGameWidth = width;
			this.FGameHeight = height;
			this.FCells = null;

			// if some optimisation well be needede check this 
			this.CreateField();
		}

        protected int IncrementCoordinate(int aCoordinate, int aSize)
        {
            return (aCoordinate + 1) % aSize;
        }

        protected int DecrementCoordinate(int aCoordinate, int aSize)
        {
            return (aCoordinate + aSize - 1) % aSize;
        }

        public override object GetData()
        {
            return new RectangleGameFieldData() { Width = this.GameWidth, Height = this.GameHeight };
        }

        public override void ReadXml(XmlReader reader)
        {
            this.FGameWidth = reader.ReadContentAsInt();
            this.FGameHeight = reader.ReadContentAsInt();
            this.CreateField();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("GameWidth", this.GameWidth.ToString());
            writer.WriteElementString("GameHeight", this.GameHeight.ToString());
            writer.WriteElementString("FieldType", this.GetType().ToString());
        }
    }

    public class RectangleGameFieldData
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class SquareCellTorGameField : RectangleGameField
    {
        public SquareCellTorGameField()
        {

        }

        public SquareCellTorGameField(int aWidth, int aHeight) :
            base(aWidth, aHeight)
        {
        }

        public SquareCellTorGameField(RectangleGameFieldData aData)
            : base(aData)
        {
        }

        public override void CreateField()
        {
            base.CreateField();

            for (int i = 0; i < this.Cells.Length; i++)
            {
                Cell _cell = this.Cells[i];
                int _x = i % this.GameWidth;
                int _y = i / this.GameWidth;
                int _incrX = IncrementCoordinate(_x, this.GameWidth);
                int _decrX = DecrementCoordinate(_x, this.GameWidth);
                int _incrY = IncrementCoordinate(_y, this.GameHeight);
                int _decrY = DecrementCoordinate(_y, this.GameHeight);
                _cell.Neighbour.Add(this[_y * this.GameWidth + _incrX]);
                _cell.Neighbour.Add(this[_y * this.GameWidth + _decrX]);
                _cell.Neighbour.Add(this[_incrY * this.GameWidth + _x]);
                _cell.Neighbour.Add(this[_decrY * this.GameWidth + _x]);

                _cell.Neighbour.Add(this[_incrY * this.GameWidth + _incrX]);
                _cell.Neighbour.Add(this[_incrY * this.GameWidth + _decrX]);
                _cell.Neighbour.Add(this[_decrY * this.GameWidth + _incrX]);
                _cell.Neighbour.Add(this[_decrY * this.GameWidth + _decrX]);
            }
        }

		public override void SetSize(int width, int height)
		{
			int _oldWidth = this.GameWidth;
			int _oldHeight = this.GameHeight;
			var _oldField = new Cell[_oldWidth * _oldHeight];
			Array.Copy(this.Cells, _oldField, _oldField.Length);

			base.SetSize(width, height);

			int _shift = width - _oldWidth;
			for (int i = 0; i < _oldHeight; i++)
			{
				for (int j = 0; j < _oldWidth; j++)
				{
					int _newIndex = i * width + j;
					if (_newIndex < 0 || _newIndex >= this.Cells.Length)
						continue;

					this.Cells[_newIndex].CellItem = _oldField[i * _oldWidth + j].CellItem; ;
				
				}
			}
		}
	}

    public class HexagonCellTorGameField : RectangleGameField
    {
        public HexagonCellTorGameField()
        {

        }

        public HexagonCellTorGameField(int aWidth, int aHeight) :
            base(aWidth, aHeight)
        {
        }

        public HexagonCellTorGameField(RectangleGameFieldData aData)
            : base(aData)
        {
        }

        [XmlIgnore]
        protected override Cell[] Cells
        {
            get
            {
                return base.Cells;
            }
        }

        public override void CreateField()
        {
            base.CreateField();

            for (int i = 0; i < this.Cells.Length; i++)
            {
                Cell _cell = this.Cells[i];
                int _x = i % this.GameWidth;
                int _y = i / this.GameWidth;

                int _incrX = IncrementCoordinate(_x, this.GameWidth);
                int _decrX = DecrementCoordinate(_x, this.GameWidth);
                int _incrY = IncrementCoordinate(_y, this.GameHeight);
                int _decrY = DecrementCoordinate(_y, this.GameHeight);

                _cell.Neighbour.Add(this[_y * this.GameWidth + _incrX]);
                _cell.Neighbour.Add(this[_y * this.GameWidth + _decrX]);
                _cell.Neighbour.Add(this[_incrY * this.GameWidth + _x]);
                _cell.Neighbour.Add(this[_decrY * this.GameWidth + _x]);

                if (_y % 2 == 0)
                {
                    _cell.Neighbour.Add(this[_incrY * this.GameWidth + _incrX]);
                    _cell.Neighbour.Add(this[_decrY * this.GameWidth + _incrX]);
                }
                else
                {
                    _cell.Neighbour.Add(this[_incrY * this.GameWidth + _decrX]);
                    _cell.Neighbour.Add(this[_decrY * this.GameWidth + _decrX]);
                }
            }
        }
    }

	public class CubicGameFieldData
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public int Length { get; set; }
	}

	public class CubicGameField : GameField
	{
		public CubicGameField()
        {

        }

		public CubicGameField(int aWidth, int aHeight, int aLength)
        {
            this.FGameWidth = aWidth;
            this.FGameHeight = aHeight;
			this.FGameLength = aLength;
            this.FCells = new Cell[this.FGameWidth * this.FGameHeight * this.FGameLength];
            this.CreateField();
        }

        public CubicGameField(CubicGameFieldData aData)
            :this(aData.Width, aData.Height, aData.Length)
        {

        }

        private int FGameHeight = 74;
        public int GameHeight
        {
            get { return this.FGameHeight; }
            set { this.FGameHeight = value; }
        }

        private int FGameWidth = 74;
        public int GameWidth
        {
            get { return this.FGameWidth; }
            set { this.FGameWidth = value; }
        }

		private int FGameLength = 74;
        public int GameLength
        {
            get { return this.FGameLength; }
            set { this.FGameLength = value; }
        }

        protected override Cell[] Cells
        {
            get
            {
                if (this.FCells == null)
                    this.FCells = new Cell[this.FGameWidth * this.FGameHeight * this.FGameLength];

                return this.FCells;
            }
        }

        protected int IncrementCoordinate(int aCoordinate, int aSize)
        {
            return (aCoordinate + 1) % aSize;
        }

        protected int DecrementCoordinate(int aCoordinate, int aSize)
        {
            return (aCoordinate + aSize - 1) % aSize;
        }

        public override object GetData()
        {
            return new CubicGameFieldData() { Width = this.GameWidth, Height = this.GameHeight, Length = this.GameLength };
        }

        public override void ReadXml(XmlReader reader)
        {
            this.FGameWidth = reader.ReadContentAsInt();
            this.FGameHeight = reader.ReadContentAsInt();
			this.FGameLength = reader.ReadContentAsInt();
            this.CreateField();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("GameWidth", this.GameWidth.ToString());
            writer.WriteElementString("GameHeight", this.GameHeight.ToString());
			writer.WriteElementString("GameLength", this.GameLength.ToString());
            writer.WriteElementString("FieldType", this.GetType().ToString());
        }
	}

	public class CubicCellGameField : CubicGameField
	{
		public CubicCellGameField()
        {

        }

        public CubicCellGameField(int aWidth, int aHeight, int aLength) :
            base(aWidth, aHeight, aLength)
        {
        }

		public CubicCellGameField(CubicGameFieldData aData)
            : base(aData)
        {
        }

        public override void CreateField()
        {
            base.CreateField();

			int s = this.GameWidth * this.GameLength;

            for (int i = 0; i < this.Cells.Length; i++)
            {
                Cell _cell = this.Cells[i];
                int _x = (i % s) % this.GameWidth;
                int _y = (i % s) / this.GameWidth;
				int _z = i / s;
                int _incrX = IncrementCoordinate(_x, this.GameWidth);
                int _decrX = DecrementCoordinate(_x, this.GameWidth);
                int _incrY = IncrementCoordinate(_y, this.GameLength);
                int _decrY = DecrementCoordinate(_y, this.GameLength);
				int _incrZ = IncrementCoordinate(_z, this.GameHeight);
				int _decrZ = DecrementCoordinate(_z, this.GameHeight);

				int _zs = _z * s; 
				int _yw = _y * this.GameWidth;
				int _iyw = _incrY * this.GameWidth;
				int _dyw = _decrY * this.GameWidth;
				_cell.Neighbour.Add(this.Cells[_zs + _yw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_zs + _yw + _decrX]);
				_cell.Neighbour.Add(this.Cells[_zs + _iyw + _x]);
				_cell.Neighbour.Add(this.Cells[_zs + _dyw + _x]);

				_cell.Neighbour.Add(this.Cells[_zs + _iyw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_zs + _iyw + _decrX]);
				_cell.Neighbour.Add(this.Cells[_zs + _dyw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_zs + _dyw + _decrX]);

				int _izs = _incrZ * s;
				_cell.Neighbour.Add(this.Cells[_izs + _yw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_izs + _yw + _decrX]);
				_cell.Neighbour.Add(this.Cells[_izs + _iyw + _x]);
				_cell.Neighbour.Add(this.Cells[_izs + _dyw + _x]);

				_cell.Neighbour.Add(this.Cells[_izs + _iyw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_izs + _iyw + _decrX]);
				_cell.Neighbour.Add(this.Cells[_izs + _dyw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_izs + _dyw + _decrX]);

				_cell.Neighbour.Add(this.Cells[_izs + _y + _x]);

				int _dzs = _decrZ * s;
				_cell.Neighbour.Add(this.Cells[_dzs + _yw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_dzs + _yw + _decrX]);
				_cell.Neighbour.Add(this.Cells[_dzs + _iyw + _x]);
				_cell.Neighbour.Add(this.Cells[_dzs + _dyw + _x]);

				_cell.Neighbour.Add(this.Cells[_dzs + _iyw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_dzs + _iyw + _decrX]);
				_cell.Neighbour.Add(this.Cells[_dzs + _dyw + _incrX]);
				_cell.Neighbour.Add(this.Cells[_dzs + _dyw + _decrX]);

				_cell.Neighbour.Add(this.Cells[_dzs + _y + _x]);
            }
        }
	}
}
