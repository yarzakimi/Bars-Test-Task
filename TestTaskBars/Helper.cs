﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskBars
{
    public static class Helper
    {
        public static string GetCellNumber(int row, int column)
        {
            var columnSheet = new StringBuilder();

            while (column > 0)
            {
                var cm = column % 26;

                if (cm == 0)
                {
                    column--;
                    columnSheet.Insert(0, 'Z');
                }
                else
                {
                    columnSheet.Insert(0, (char)(cm + 'A' - 1));
                }

                column /= 26;
            }

            return $"{columnSheet}{row}";
        }

        public static SheetCellNumeric GetRowAndColumnNumber(string sheetCellNumber)
        {
            var i = 0;

            for (; i < sheetCellNumber.Length; i++)
            {
                if (char.IsDigit(sheetCellNumber[i]))
                {
                    break;
                }
            }

            var column = 0;
            var row = sheetCellNumber.Substring(i);

            var cs = sheetCellNumber.Substring(0, i);
            var csl = cs.Length - 1;

            for (var j = 0; j < cs.Length; j++)
            {
                var cc = cs[j] - 'A' + 1;

                var multi = (int)Math.Pow(26, csl--);

                column += cc * multi;
            }

            return new SheetCellNumeric { RowNumber = int.Parse(row), ColumnNumber = column };
        }

        public static IList<IList<Object>> GenerateData(IList<IList<Object>> table)
        {
            List<IList<Object>> objNewRecords = new List<IList<Object>>();

            IList<Object> obj = new List<Object>();

            obj.Add("Сервер");
            obj.Add("База данных");
            obj.Add("Размер в байтах");
            obj.Add("Размер");
            obj.Add("Дата обновления");

            objNewRecords.Add(obj);

            objNewRecords.AddRange(table);

            return objNewRecords;
        }
        public class SheetCellNumeric
        {
            public int ColumnNumber;

            public int RowNumber;
        }
    }
}