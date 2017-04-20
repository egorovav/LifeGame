using Life;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LifeWpfClient
{
    public abstract class LifeGameFactory : IXmlSerializable
    {
        protected abstract GameField CreateField();
        private GameField FGameField;
        public GameField GameField
        {
            get
            {
                if (this.FGameField == null)
                    this.FGameField = this.CreateField();

                return this.FGameField;
            }
        }

        protected virtual GameFieldFrameworkElement CreateGameFieldFrameworkElement(GameField aGameField, int itemSize)
        {
            return new GameFieldFrameworkElement(this.GameField, itemSize);
        }

        public GameFieldFrameworkElement GetGameFieldFrameworkElement(int itemSize)
        {
			return CreateGameFieldFrameworkElement(this.GameField, itemSize);
        }

        public virtual LifeGame GetLifeGame(int aStartItemCount)
        {
            this.GameField.ClearCells();
            return new LifeGame(this.GameField, aStartItemCount);
        }


        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(System.Xml.XmlReader reader);

        public abstract void WriteXml(System.Xml.XmlWriter writer);
    }

    public class RectangleLifeGameFactory : LifeGameFactory
    {
        public RectangleLifeGameFactory()
        {

        }

        public RectangleLifeGameFactory(int aGameWidth, int aGameHeight)
        {
            this.FGameWidth = aGameWidth;
            this.FGameHeight = aGameHeight;
        }

        protected int FGameWidth;
        protected int FGameHeight;

        protected override GameField CreateField()
        {
            return new RectangleGameField(this.FGameWidth, this.FGameHeight);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            this.FGameWidth = reader.ReadElementContentAsInt();
            this.FGameHeight = reader.ReadElementContentAsInt();
            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("FGameWidth", this.FGameWidth.ToString());
            writer.WriteElementString("FGameHeight", this.FGameHeight.ToString());
        }
    }


    public class SquareLifeGameFactory : RectangleLifeGameFactory
    {
        public SquareLifeGameFactory(int aGameWidth, int aGameHeight)
            : base(aGameWidth, aGameHeight)
        {
        }

        public SquareLifeGameFactory()
        {
            // TODO: Complete member initialization
        }

        protected override GameFieldFrameworkElement CreateGameFieldFrameworkElement(GameField aGameField, int itemSize)
        {
            return new SquareTorGameFieldFrameworkElement((Life.SquareCellTorGameField)aGameField, itemSize);
        }

        protected override GameField CreateField()
        {
            return new SquareCellTorGameField(this.FGameWidth, this.FGameHeight);
        }
    }


    public class HexagonLifeGameFacrory : RectangleLifeGameFactory
    {
        

        public HexagonLifeGameFacrory(int aGameWidth, int aGameHeight)
            : base(aGameWidth, aGameHeight)
        {
        }

        public HexagonLifeGameFacrory()
        {
            // TODO: Complete member initialization
        }

        protected override GameFieldFrameworkElement CreateGameFieldFrameworkElement(GameField aGameField, int itemSize)
        {
            return new HexagonTorGameFieldFrameworkElement((HexagonCellTorGameField)aGameField, itemSize);
        }

        protected override GameField CreateField()
        {
            return new HexagonCellTorGameField(this.FGameWidth, this.FGameHeight);
        }
    }
}
