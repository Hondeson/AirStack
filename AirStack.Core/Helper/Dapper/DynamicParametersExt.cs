﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Helper.Dapper
{
    public static class DynamicParametersExt
    {
        /// <summary>
        /// jméno typu vytvořeného v DB
        /// </summary>
        const string c_StringList = "dbo.StringList"; 
        public static void AddStringList(this DynamicParameters param, string paramName, IEnumerable<string> stringList)
        {
            var table = new DataTable();
            table.Columns.Add("value", typeof(string));

            foreach (var item in stringList)
                table.Rows.Add(item);

            param.Add(paramName, table.AsTableValuedParameter(c_StringList));
        }
    }
}
