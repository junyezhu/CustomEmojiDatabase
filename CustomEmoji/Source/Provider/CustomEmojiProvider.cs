namespace CustomEmoji.Source.Provider
{
    using CustomEmoji.Source.CustomEmojiInterface;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class CustomEmojiProvider : ICustomEmojiProvider
    {
        private const string databaseName = "CustomEmojiDatabase";
        private const string tableName = "CustomEmojiEntryTable";
        private static string xmlFilepath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\Source\\EmojiDatabase.xml";
        private readonly PropertyInfo[] customEmojiEntryProperties = typeof(CustomEmojiEntry).GetProperties();
        private DataSet dataSet;
        private DataTable emojiEntriesTable;

        public CustomEmojiProvider()
        {
            InitializeDataSet();
            LoadDataFromXmlIntoDataSet();
        }

        ///<inheritdoc/>
        public bool TryAddCustomEmojiEntry(CustomEmojiEntry entry)
        {
            if (string.IsNullOrEmpty(entry?.Id))
            {
                // Log input entry Id can't be empty.
                return false;
            }

            if (emojiEntriesTable.AsEnumerable().Any(row => string.Equals(row.Field<String>("Id"), entry.Id, StringComparison.OrdinalIgnoreCase)))
            {
                // Log entry with same Id already exists.
                return false;
            }

            try
            {
                AddCustomEmojiEntryIntoTable(entry);

                emojiEntriesTable.AcceptChanges();
                dataSet.AcceptChanges();
                dataSet.WriteXml(xmlFilepath);
                return true;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }

        ///<inheritdoc/>
        public bool TryAddOrUpdateCustomEmojiEntry(CustomEmojiEntry entry)
        {
            if (string.IsNullOrEmpty(entry?.Id))
            {
                // Log input entry Id can't be empty.
                return false;
            }

            try
            {
                var row = emojiEntriesTable.AsEnumerable().FirstOrDefault(row => string.Equals(row.Field<String>("Id"), entry.Id, StringComparison.OrdinalIgnoreCase));
                if (row == null)
                {
                    AddCustomEmojiEntryIntoTable(entry);
                }
                else
                {
                    UpdateCustomEmojiEntry(row, entry);
                }

                emojiEntriesTable.AcceptChanges();
                dataSet.AcceptChanges();
                dataSet.WriteXml(xmlFilepath);
                return true;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }

        ///<inheritdoc/>
        public IList<CustomEmojiEntry> GetPublicCustomEmojiEntries()
        {
            return emojiEntriesTable?.AsEnumerable()?.Select(row => GetCustomEmojiEntry(row)).Where(entry => entry.State == CustomEmojiEntryState.Public).ToList();
        }

        ///<inheritdoc/>
        public IList<CustomEmojiEntry> GetCustomEmojiEntriesByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception($"Input user object id can't be empty.");
            }

            return emojiEntriesTable?.AsEnumerable()?.Select(row => GetCustomEmojiEntry(row))
                .Where(entry => entry.State == CustomEmojiEntryState.Public || (entry.State == CustomEmojiEntryState.Private && string.Equals(entry.AuthorUserObjectId, userId, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        ///<inheritdoc/>
        public bool TryInactiveExistingCustomEmojiEntry(string id, string userId)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Log input emoji entry Id can't be empty.
                return false;
            }

            var entry = emojiEntriesTable.AsEnumerable().FirstOrDefault(row => string.Equals(row.Field<String>("Id"), id, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                // Log unable to find exisiting record with Id.
                return false;
            }
            else if(!string.Equals((string)entry["AuthorUserObjectId"], userId,StringComparison.OrdinalIgnoreCase))
            {
                // Provided user doesn't have permission to inactive this custom emoji.
                return false;
            }

            try
            {
                entry.BeginEdit();
                entry["State"] = CustomEmojiEntryState.Inactive;
                entry.EndEdit();

                emojiEntriesTable.AcceptChanges();
                dataSet.AcceptChanges();
                dataSet.WriteXml(xmlFilepath);
                return true;
            }
            catch (Exception)
            {
                // Log exception
                return false;
            }
        }

        /// <summary>
        /// Load custom emoji entry data from Xml
        /// </summary>
        private void LoadDataFromXmlIntoDataSet()
        {
            if (!File.Exists(xmlFilepath))
            {
                // Log Failed to read xml file using path xmlFilepath, will create new xml file.
                return;
            }

            dataSet.ReadXml(xmlFilepath);
            emojiEntriesTable = dataSet.Tables[tableName];
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        private void InitializeDataSet()
        {
            dataSet = new DataSet(databaseName);
            emojiEntriesTable = new DataTable(tableName);
            foreach (PropertyInfo info in customEmojiEntryProperties)
            {
                emojiEntriesTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            dataSet.Tables.Add(emojiEntriesTable);
            dataSet.AcceptChanges();
        }

        /// <summary>
        /// Insert new CustomEmojiEntry entry into emojiEntriesTable
        /// </summary>
        /// <param name="entry">CustomEmojiEntry entry to be inserted</param>
        private void AddCustomEmojiEntryIntoTable(CustomEmojiEntry entry)
        {
            object[] values = new object[customEmojiEntryProperties.Length];
            for (int i = 0; i < customEmojiEntryProperties.Length; i++)
            {
                values[i] = customEmojiEntryProperties[i].GetValue(entry);
            }

            emojiEntriesTable.Rows.Add(values);
        }

        /// <summary>
        /// Convert DataRow in emojiEntriesTable into CustomEmojiEntry object
        /// </summary>
        /// <param name="dr">DataRow in table emojiEntriesTable</param>
        /// <returns>CustomEmojiEntry object</returns>
        private CustomEmojiEntry GetCustomEmojiEntry(DataRow dr)
        {
            CustomEmojiEntry entry = new CustomEmojiEntry();

            foreach (DataColumn column in dr.Table.Columns)
            {
                PropertyInfo property = customEmojiEntryProperties.FirstOrDefault(pro => string.Equals(pro.Name, column.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                    property.SetValue(entry, dr[column.ColumnName], null);
            }

            return entry;
        }

        /// <summary>
        /// Update exisiting DataRowin emojiEntriesTable with input CustomEmojiEntry object
        /// </summary>
        /// <param name="dr">DataRow in table emojiEntriesTable</param>
        /// <param name="entry">CustomEmojiEntry object</param>
        private void UpdateCustomEmojiEntry(DataRow dr, CustomEmojiEntry entry)
        {
            dr.BeginEdit();
            foreach (DataColumn column in dr.Table.Columns)
            {
                PropertyInfo property = customEmojiEntryProperties.FirstOrDefault(pro => string.Equals(pro.Name, column.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    dr[column.ColumnName] = property.GetValue(entry);
                }
            }

            dr.EndEdit();
        }
    }
}
