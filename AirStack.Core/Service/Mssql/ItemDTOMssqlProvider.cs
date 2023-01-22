using AirStack.Core.Connection;
using AirStack.Core.Helper.Dapper;
using AirStack.Core.Model;
using AirStack.Core.Model.API;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace AirStack.Core.Service.Mssql
{
    public class ItemDTOMssqlProvider : IItemDTOProvider
    {
        readonly ISqlAdapter _sql;
        public ItemDTOMssqlProvider(ISqlAdapter sqlAdapter)
        {
            _sql = sqlAdapter;
        }

        const string c_GetItemDTOProc = "[dbo].[GetItemDTO]";
        public List<GetItemDTO> Get(
            long offset, long fetch,
            List<StatusEnum> statuses,
            string? codeLike, string? parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo)
        {
            using (var con = _sql.Connect())
            {
                DynamicParameters p = GetParams(
                    statuses,
                    codeLike, parentCodeLike,
                    productionFrom, productionTo,
                    dispatchedFrom, dispatchedTo,
                    testsFrom, testsTo,
                    complaintFrom, complaintTo,
                    complaintSuplFrom, complaintSuplTo);

                p.Add("@Offset", offset);
                p.Add("@Fetch", fetch);

                return con.Query<GetItemDTO>(c_GetItemDTOProc, p, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        const string c_GetItemsCountProc = "[dbo].[GetItemCount]";
        public long GetCount(
            List<StatusEnum> statuses,
            string? codeLike, string? parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo)
        {
            using (var con = _sql.Connect())
            {
                DynamicParameters p = GetParams(
                    statuses,
                    codeLike, parentCodeLike,
                    productionFrom, productionTo,
                    dispatchedFrom, dispatchedTo,
                    testsFrom, testsTo,
                    complaintFrom, complaintTo,
                    complaintSuplFrom, complaintSuplTo);

                return con.ExecuteScalar<long>(c_GetItemsCountProc, p, commandType: CommandType.StoredProcedure);
            }
        }

        static DynamicParameters GetParams(
            List<StatusEnum> statuses,
            string codeLike, string parentCodeLike,
            DateTimeOffset? productionFrom, DateTimeOffset? productionTo,
            DateTimeOffset? dispatchedFrom, DateTimeOffset? dispatchedTo,
            DateTimeOffset? testsFrom, DateTimeOffset? testsTo,
            DateTimeOffset? complaintFrom, DateTimeOffset? complaintTo,
            DateTimeOffset? complaintSuplFrom, DateTimeOffset? complaintSuplTo)
        {
            var p = new DynamicParameters();

            p.AddStringList("@Statuses", statuses.Select(x => x.ToString()).ToList());
            p.Add("@CodeLike", codeLike);
            p.Add("@ParentCodeLike", parentCodeLike);

            p.Add("@ProductionFrom", productionFrom);
            p.Add("@ProductionTo", productionTo);

            p.Add("@DispatchedFrom", dispatchedFrom);
            p.Add("@DispatchedTo", dispatchedTo);

            p.Add("@TestsFrom", testsFrom);
            p.Add("@TestsTo", testsTo);

            p.Add("@ComplaintFrom", complaintFrom);
            p.Add("@ComplaintTo", complaintTo);

            p.Add("@ComplaintSuplFrom", complaintSuplFrom);
            p.Add("@ComplaintSuplTo", complaintSuplTo);

            return p;
        }
    }
}
