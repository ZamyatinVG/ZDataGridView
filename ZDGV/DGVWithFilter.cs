using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

namespace ZDGV
{
    public class DGVWithFilter : DataGridView
    {
        List<FilterStatus> Filter = new List<FilterStatus>();
        TextBox SearchTB = new TextBox();
        DateTimePicker DTP = new DateTimePicker();
        CheckedListBox CheckLB = new CheckedListBox();
        Button ApplyButton = new Button();
        Button ClearAllButton = new Button();
        Button ClearButton = new Button();
        ToolStripDropDown popup = new ToolStripDropDown();

        ContextMenuStrip context = new ContextMenuStrip();
        ToolStripMenuItem miFilter = new ToolStripMenuItem("Фильтр");
        ToolStripMenuItem miClearFilter = new ToolStripMenuItem(ClearButtonText);
        ToolStripSeparator miSeparator = new ToolStripSeparator();
        ToolStripMenuItem miFilterByValue = new ToolStripMenuItem("Фильтр по значению");
        //ToolStripMenuItem miFilterByColor = new ToolStripMenuItem("Фильтр по цвету");

        const string SearchTBText = "<Поиск>";
        const string ButtonCtrlText = "Применить";
        const string ClearAllButtonText = "Очистить все фильтры";
        const string CheckLBAllText = "<Выбрать всё>";
        const string ClearButtonText = "Очистить фильтр";
        public string SpaceText = "<Пустые>";
        Color CheckedColor = Color.Gold;
        Color UnCheckedColor = SystemColors.Control;
        bool checkMark = true;

        // Добавление столбца
        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            this.EnableHeadersVisualStyles = false;
            this.AllowUserToAddRows = false;

            this.miFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.miClearFilter, this.miSeparator, this.miFilterByValue/*, this.miFilterByColor*/ });
            this.context.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.miFilter });
            this.ContextMenuStrip = this.context;

            var header = new DGVFilterHeader();
            header.FilterButtonClicked += new EventHandler<ColumnFilterClickedEventArg>(Header_FilterButtonClicked);
            e.Column.HeaderCell = header;
            base.OnColumnAdded(e);
        }
        
        // Отключение сортировки
        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            //base.OnColumnHeaderMouseClick(e);
        }

        // Обнуление фильтров при смене источника данных
        protected override void OnDataSourceChanged(EventArgs e)
        {
            Filter.Clear();
            base.OnDataSourceChanged(e);
        }

        // Выделение ячейки
        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            this.ColumnIndex = e.ColumnIndex;
            if (e.RowIndex != -1 && e.Button == MouseButtons.Right)
            {
                if (!this.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                    this.ClearSelection();
                //this.cellColor = this.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor.Name;
                this.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;

                this.miClearFilter.Click -= ClearButton_Click;
                this.miClearFilter.Click += ClearButton_Click;
                this.miFilterByValue.Click -= new EventHandler(MiFilterByValue_Click);
                this.miFilterByValue.Click += new EventHandler(MiFilterByValue_Click);
                //this.miFilterByColor.Click -= new EventHandler(miFilterByColor_Click);
                //this.miFilterByColor.Click += new EventHandler(miFilterByColor_Click);
            }
            base.OnCellMouseDown(e);
        }

        // Скролл после сортировки
        public override void Sort(DataGridViewColumn dataGridViewColumn, System.ComponentModel.ListSortDirection direction)
        {
            int scrl = this.HorizontalScrollBar.Value;
            int scrlOffset = this.HorizontalScrollingOffset;
            base.Sort(dataGridViewColumn, direction);
            this.HorizontalScrollBar.Value = scrl;
            this.HorizontalScrollingOffset = scrlOffset;
        }

        // Текущий индекс столбца
        private int ColumnIndex { get; set; }

        // Цвет выделенной йчейки
        //private string cellColor { get; set; }

        // Событие кнопки фильтрации
        private void Header_FilterButtonClicked(object sender, ColumnFilterClickedEventArg e)
        {
            int widthTool = this.Columns[e.ColumnIndex].Width + 50;
            if (widthTool < 200) widthTool = 200;
            int heightTool = this.Height - 200;
            if (heightTool < 200) heightTool = 200;

            ColumnIndex = e.ColumnIndex;

            SearchTB.Clear();
            CheckLB.Items.Clear();

            SearchTB.Size = new System.Drawing.Size(widthTool, 30);
            SearchTB.Text = SearchTBText;
            SearchTB.ForeColor = Color.Gray;
            SearchTB.MouseDown -= SearchTB_MouseDown;
            SearchTB.MouseDown += SearchTB_MouseDown;
            SearchTB.TextChanged -= SearchTB_TextChanged;
            SearchTB.TextChanged += SearchTB_TextChanged;

            DTP.Size = new System.Drawing.Size(widthTool, 30);
            DTP.Format = DateTimePickerFormat.Custom;
            DTP.CustomFormat = "dd.MM.yyyy";
            DTP.TextChanged -= DTP_TextChanged;
            DTP.TextChanged += DTP_TextChanged;

            CheckLB.ItemCheck -= CheckLB_ItemCheck;
            CheckLB.ItemCheck += CheckLB_ItemCheck;
            CheckLB.CheckOnClick = true;

            GetChkFilter();

            CheckLB.MaximumSize = new System.Drawing.Size(widthTool, heightTool);
            CheckLB.Size = new System.Drawing.Size(widthTool, (CheckLB.Items.Count + 1) * 18);

            ApplyButton.Text = ButtonCtrlText;
            ApplyButton.Size = new System.Drawing.Size(widthTool, 30);
            ApplyButton.Click -= ApplyButton_Click;
            ApplyButton.Click += ApplyButton_Click;

            ClearAllButton.Text = ClearAllButtonText;
            ClearAllButton.Size = new System.Drawing.Size(widthTool, 30);
            ClearAllButton.Click -= ClearAllButton_Click;
            ClearAllButton.Click += ClearAllButton_Click;

            ClearButton.Text = ClearButtonText;
            ClearButton.Size = new System.Drawing.Size(widthTool, 30);
            ClearButton.Click -= ClearButton_Click;
            ClearButton.Click += ClearButton_Click;

            popup.Items.Clear();
            popup.AutoSize = true;
            popup.Margin = Padding.Empty;
            popup.Padding = Padding.Empty;

            ToolStripControlHost host1 = new ToolStripControlHost(SearchTB)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = SearchTB.Size
            };
            ToolStripControlHost host2 = new ToolStripControlHost(CheckLB)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = CheckLB.Size
            };
            ToolStripControlHost host3 = new ToolStripControlHost(ApplyButton)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = ApplyButton.Size
            };
            ToolStripControlHost host4 = new ToolStripControlHost(ClearAllButton)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = ClearAllButton.Size
            };
            ToolStripControlHost host5 = new ToolStripControlHost(DTP)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = DTP.Size
            };
            ToolStripControlHost host6 = new ToolStripControlHost(ClearButton)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = ClearButton.Size
            };

            switch (this.Columns[ColumnIndex].ValueType.ToString())
            {
                case "System.DateTime":
                    popup.Items.Add(host5);
                    break;
                default:
                    popup.Items.Add(host1);
                    break;
            }
            popup.Items.Add(host2);
            popup.Items.Add(host3);
            popup.Items.Add(host6);
            popup.Items.Add(host4);

            popup.Show(this, e.ButtonRectangle.X, e.ButtonRectangle.Bottom);
            host2.Focus();
        }

        // Выбор значений CheckLB
        private void CheckLB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == 0)
            {
                if (checkMark)
                    if (e.NewValue == CheckState.Checked)
                        for (int i = 1; i < CheckLB.Items.Count; i++)
                            CheckLB.SetItemChecked(i, true);
                    else
                        for (int i = 1; i < CheckLB.Items.Count; i++)
                            CheckLB.SetItemChecked(i, false);
            }
            else
                if (e.NewValue == CheckState.Unchecked)
                {
                    checkMark = false;
                    CheckLB.SetItemChecked(0, false);
                    checkMark = true;
                }
                else
                {
                    int checkCount = 0;
                    for (int i = 0; i < CheckLB.Items.Count; i++)
                        if (CheckLB.GetItemCheckState(i) == CheckState.Checked)
                            checkCount++;
                    if (checkCount == CheckLB.Items.Count - 2)
                    {
                        checkMark = false;
                        CheckLB.SetItemChecked(0, true);
                        checkMark = true;
                    }
                }
        }

        // Удаление подсказки поиска
        private void SearchTB_MouseDown(object sender, EventArgs e)
        {
            SearchTB.Text = null;
            SearchTB.ForeColor = Color.Black;
        }

        // Событие при изменении текста в TextBox
        private void SearchTB_TextChanged(object sender, EventArgs e)
        {
            //(this.DataSource as DataTable).DefaultView.RowFilter = string.Format("convert([" + this.Columns[columnIndex].Name + "], 'System.String') LIKE '%{0}%'", SearchTB.Text);
            if (SearchTB.Text != "")
                for (int i = 0; i < CheckLB.Items.Count; i++)
                    if (!CheckLB.Items[i].ToString().ToLower().Contains(SearchTB.Text.ToLower()))
                        CheckLB.SetItemChecked(i, false);
                    else CheckLB.SetItemChecked(i, true);
        }

        // Событие при изменении даты
        private void DTP_TextChanged(object sender, EventArgs e)
        {
            FilterStatusComparerDateTime fsc = new FilterStatusComparerDateTime();
            for (int i = 0; i < CheckLB.Items.Count; i++)
                if (CheckLB.Items[i].ToString() == SpaceText || CheckLB.Items[i].ToString() == CheckLBAllText || fsc.ToDate(CheckLB.Items[i].ToString()) != fsc.ToDate(DTP.Text))
                    CheckLB.SetItemChecked(i, false);
                else CheckLB.SetItemChecked(i, true);
        }

        // Очистить все фильтры
        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            Filter.Clear();
            ApllyFilter();
            popup.Close();
            for (int i = 0; i < this.Columns.Count; i++) 
                this.Columns[i].HeaderCell.Style.BackColor = UnCheckedColor;
        }

        // Очистить фильтр
        private void ClearButton_Click(object sender, EventArgs e)
        {
            Filter.RemoveAll(x => x.columnName == this.Columns[ColumnIndex].Name);
            ApllyFilter();
            popup.Close();
            this.Columns[ColumnIndex].HeaderCell.Style.BackColor = UnCheckedColor;
        }

        // Событие кнопки применить
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (CheckLB.CheckedItems.Count != 0)
            {
                if (CheckLB.GetItemCheckState(0) == CheckState.Checked)
                {
                    Filter.RemoveAll(x => x.columnName == this.Columns[ColumnIndex].Name);
                    this.Columns[ColumnIndex].HeaderCell.Style.BackColor = UnCheckedColor;
                }
                else
                    SaveChkFilter();
                ApllyFilter();
                popup.Close();
            }
        }

        // Получаем данные из выбранной колонки 
        private List<string> GetDataColumns(int e)
        {
            List<string> ValueCellList = new List<string>();
            string Value;
            // Поиск данных в столбце, исключая повторения
            foreach (DataGridViewRow row in this.Rows)
            {
                Value = row.Cells[e].Value.ToString();
                if (Value == "") Value = SpaceText;

                if (!ValueCellList.Contains(Value))
                    ValueCellList.Add(Value);
            }
            return ValueCellList;
        }

        // Запомнить чекбоксы фильтра
        private void SaveChkFilter()
        {
            string col = this.Columns[ColumnIndex].Name;
            string itemChk;
            bool statChk;

            Filter.RemoveAll(x => x.columnName == col);
            Filter.RemoveAll(x => x.columnName != col && !x.check);

            if (CheckLB.CheckedItems.Count < CheckLB.Items.Count)
                this.Columns[ColumnIndex].HeaderCell.Style.BackColor = CheckedColor;
            else
                this.Columns[ColumnIndex].HeaderCell.Style.BackColor = UnCheckedColor;

            for (int i = 1; i < CheckLB.Items.Count; i++)
            {
                itemChk = CheckLB.Items[i].ToString();
                statChk = CheckLB.GetItemChecked(i);
                Filter.Add(new FilterStatus() { columnName = col, valueString = itemChk, check = statChk });
            }
        }

        // Загрузить чекбоксы
        private void GetChkFilter()
        {
            List<FilterStatus> CheckList = new List<FilterStatus>();
            List<string> ColumnList = new List<string>();

            // Поиск данных в таблице
            foreach (string ValueCell in GetDataColumns(this.ColumnIndex))
            {
                int index = CheckList.FindIndex(item => item.valueString == ValueCell);
                if (index == -1)
                    CheckList.Add(new FilterStatus { valueString = ValueCell, check = true });
            }

            // Поиск сохранённых данных
            foreach (FilterStatus val in Filter)
            {
                if (val.columnName == this.Columns[ColumnIndex].Name)
                {
                    if (val.valueString == "") val.valueString = SpaceText;
                    int index = CheckList.FindIndex(item => item.valueString == val.valueString);
                    if (index == -1 && !val.check)
                        CheckList.Add(new FilterStatus() { columnName = "", valueString = val.valueString, check = val.check });
                }
                else
                    if (!ColumnList.Contains(val.columnName))
                        ColumnList.Add(val.columnName);
            }

            // Поиск кросс-данных
            for (int i = 0; i < (this.DataSource as DataTable).Rows.Count; i++)
            {
                int counter = 0;
                foreach (string columnName in ColumnList)
                    foreach (FilterStatus val in Filter)
                        if (val.columnName == columnName && val.check && val.valueString == ((this.DataSource as DataTable).Rows[i] as DataRow)[columnName].ToString())
                            counter++;
                if (counter == ColumnList.Count)
                {
                    string newValue = ((this.DataSource as DataTable).Rows[i] as DataRow)[this.ColumnIndex].ToString();
                    if (newValue == "") newValue = SpaceText;
                    int index = CheckList.FindIndex(item => item.valueString == newValue);
                    if (index == -1)
                        CheckList.Add(new FilterStatus { valueString = newValue, check = false });
                }
            }

            CheckLB.Items.Add(CheckLBAllText, CheckState.Indeterminate);
            // Сортировка
            switch (this.Columns[ColumnIndex].ValueType.ToString())
            {
                case "System.Int32":
                    CheckList.Sort(new FilterStatusComparerInt());
                    break;
                case "System.DateTime":
                    CheckList.Sort(new FilterStatusComparerDateTime());
                    break;
                default:
                    CheckList.Sort(new FilterStatusComparer());
                    break;
            }
            // Заполнение
            foreach (FilterStatus val in CheckList)
            {
                if (val.check == true)
                    CheckLB.Items.Add(val.valueString, CheckState.Checked);
                else
                    CheckLB.Items.Add(val.valueString, CheckState.Unchecked);
            }
        }

        // Применить фильтр
        private void ApllyFilter()
        {
            string StrFilter = "";
            string prev_column = "";
            try
            {
                if (Filter.Count != 0)
                {
                    foreach (FilterStatus val in Filter)
                    {
                        if (val.check)
                        {
                            string valueFilter;
                            if (val.valueString == SpaceText)
                                valueFilter = "is null";
                            else
                                switch (this.Columns[val.columnName].ValueType.ToString())
                                {
                                    case "System.String":
                                        valueFilter = "= '" + val.valueString.Replace("'", "''") + "' ";
                                        break;
                                    case "System.Boolean":
                                        if (val.valueString.ToLower() == "true")
                                            valueFilter = "= 1";
                                        else
                                            valueFilter = "= 0";
                                        break;
                                    default:
                                        valueFilter = "= '" + val.valueString + "' ";
                                        break;
                                }

                            if (StrFilter.Length == 0)
                                StrFilter += "([" + val.columnName + "] " + valueFilter;
                            else
                                if (val.columnName == prev_column)
                                    StrFilter += " OR [" + val.columnName + "] " + valueFilter;
                                else
                                    StrFilter += ") AND ([" + val.columnName + "] " + valueFilter;
                            prev_column = val.columnName;
                        }
                        if (StrFilter.Length > 10000)
                            throw new Exception();
                    }
                    if (StrFilter != "")
                        StrFilter += ")";
                    else StrFilter = "1=2";
                }
                (this.DataSource as DataTable).DefaultView.RowFilter = StrFilter;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Слишком много фильтров! Упростите поиск!" + ex.Message);
                Filter.Clear();
                StrFilter = "";
                (this.DataSource as DataTable).DefaultView.RowFilter = StrFilter;
                for (int i = 0; i < this.Columns.Count; i++)
                    this.Columns[i].HeaderCell.Style.BackColor = UnCheckedColor;
            }
        }

        // Контекстная фильтрация по значению
        private void MiFilterByValue_Click(object sender, EventArgs e)
        {
            bool mark = true;
            Filter.RemoveAll(x => x.columnName == this.Columns[ColumnIndex].Name);
            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (cell.ColumnIndex != this.ColumnIndex)
                {
                    MessageBox.Show("Операция неприемлима для ячеек разных столбцов!", "");
                    mark = false;
                    break;
                }
                Filter.Add(new FilterStatus() { columnName = this.Columns[ColumnIndex].Name, valueString = cell.Value.ToString(), check = true });
            }
            if (mark)
            {
                this.Columns[ColumnIndex].HeaderCell.Style.BackColor = CheckedColor;
                ApllyFilter();
                if (this.Rows.Count > 0)
                {
                    this.ClearSelection();
                    this.Rows[0].Cells[ColumnIndex].Selected = true;
                }
            }
        }

        // Контекстная фильтрация по цвету
        //private void miFilterByColor_Click(object sender, EventArgs e)
        //{
        //    Filter.RemoveAll(x => x.columnName == this.Columns[columnIndex].Name);
        //    ApllyFilter();
        //    foreach (DataGridViewRow row in this.Rows)
        //    {
        //        if (row.Cells[columnIndex].Style.BackColor.Name == this.cellColor)
        //        {
        //            string newValue = row.Cells[columnIndex].Value.ToString();
        //            int index = Filter.FindIndex(item => item.valueString == newValue);
        //            if (index == -1)
        //                Filter.Add(new FilterStatus { columnName = this.Columns[columnIndex].Name, valueString = newValue, check = true });
        //        }
        //    }
        //    this.Columns[columnIndex].HeaderCell.Style.BackColor = CheckedColor;
        //    ApllyFilter();
        //    if (this.Rows.Count > 0)
        //    {
        //        this.ClearSelection();
        //        this.Rows[0].Cells[columnIndex].Selected = true;
        //    }
        //}
    }
}