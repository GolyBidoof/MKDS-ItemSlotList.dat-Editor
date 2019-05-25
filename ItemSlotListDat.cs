using LibEndianBinaryIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ItemSlotListDatEditor
{
    public class ItemSlotListDat
    {
        public TableItem<ItemTableRace>[] itemTableRace = new TableItem<ItemTableRace>[0x06];
        public TableItem<ItemTableSpecialItemBox>[] itemTableSpecialItemBox = new TableItem<ItemTableSpecialItemBox>[0x01];
        public TableItem<ItemTableBalloon>[] itemTableBalloon = new TableItem<ItemTableBalloon>[0x02];
        public TableItem<ItemTableShine>[] itemTableShine = new TableItem<ItemTableShine>[0x02];

        public class TableItem<T> where T : ItemTable, new()
        {
            public byte columnsCount;
            public byte rowsCount;

            public T[] items = new T[0x12];

            public TableItem(EndianBinaryReaderEx er)
            {
                columnsCount = er.ReadByte();
                rowsCount = er.ReadByte();

                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = new T();
                    items[i].Read(er);
                }
            }

            public void Write(EndianBinaryWriterEx ew)
            {
                ew.Write(columnsCount);
                ew.Write(rowsCount);
                for (int i = 0; i < items.Length; i++)
                {
                    items[i].Write(ew);
                }
            }
        }

        public abstract class ItemTable
        {
            public abstract void Read(EndianBinaryReaderEx er);
            public abstract void Write(EndianBinaryWriterEx ew);
        }

        public class ItemTableSpecialItemBox : ItemTable
        {
            public byte[] itemboxes;

            public override void Read(EndianBinaryReaderEx er)
            {
                itemboxes = er.ReadBytes(0x10);
            }

            public override void Write(EndianBinaryWriterEx ew)
            {
                ew.Write(itemboxes, 0, 0x10);
            }
        }

        public class ItemTableRace : ItemTable
        {
            public byte[] positions;

            public override void Read(EndianBinaryReaderEx er)
            {
                positions = er.ReadBytes(0x08);
            }

            public override void Write(EndianBinaryWriterEx ew)
            {
                ew.Write(positions, 0, 0x08);
            }
        }

        public class ItemTableBalloon : ItemTable
        {
            public byte[] balloons;

            public override void Read(EndianBinaryReaderEx er)
            {
                balloons = er.ReadBytes(0x03);
            }

            public override void Write(EndianBinaryWriterEx ew)
            {
                ew.Write(balloons, 0, 0x03);
            }
        }

        public class ItemTableShine : ItemTable
        {
            public byte[] shines;

            public override void Read(EndianBinaryReaderEx er)
            {
                shines = er.ReadBytes(0x03);
            }

            public override void Write(EndianBinaryWriterEx ew)
            {
                ew.Write(shines, 0, 0x03);
            }
        }

        public byte amountsOfTables;

        public ItemSlotListDat(byte[] data)
        {
            var m = new MemoryStream(data);
            var er = new EndianBinaryReaderEx(m, Endianness.LittleEndian);
            amountsOfTables = er.ReadByte();

            for (int i = 0; i < 0x06; i++)
            {
                itemTableRace[i] = new TableItem<ItemTableRace>(er);
            }
            for (int i = 0; i < 0x01; i++)
            {
                itemTableSpecialItemBox[i] = new TableItem<ItemTableSpecialItemBox>(er);
            }
            for (int i = 0; i < 0x02; i++)
            {
                itemTableBalloon[i] = new TableItem<ItemTableBalloon>(er);
            }
            for (int i = 0; i < 0x02; i++)
            {
                itemTableShine[i] = new TableItem<ItemTableShine>(er);
            }
        }


        public byte[] Write()
        {
            var m = new MemoryStream();
            var ew = new EndianBinaryWriterEx(m, Endianness.LittleEndian);

            ew.Write(amountsOfTables);

            foreach (var itemTable in itemTableRace)
                itemTable.Write(ew);
            foreach (var itemTableSpecialItemBox in itemTableSpecialItemBox)
                itemTableSpecialItemBox.Write(ew);
            foreach (var itemTableBalloon in itemTableBalloon)
                itemTableBalloon.Write(ew);
            foreach (var itemTableShine in itemTableShine)
                itemTableShine.Write(ew);

            byte[] result = m.ToArray();
            ew.Close();
            return result;
        }
    }
}
